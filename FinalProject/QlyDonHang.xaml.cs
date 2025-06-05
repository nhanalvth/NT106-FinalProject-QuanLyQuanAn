using FinalProject.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for QlyDonHang.xaml
    /// </summary>
    public partial class QlyDonHang : Page
    {
        public QlyDonHang()
        {
            InitializeComponent();
            Loaded += (s, e) => LoadBanVaMon();
            Loaded += (s, e) =>
            {
                LoadBanVaMon();
                LoadDonHang();
            };
        }

        // Chuỗi kết nối đến cơ sở dữ liệu PostgreSQL
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        // Load danh sách đơn hàng
        public void LoadDonHang(string trangThai = "")
        {
            List<DonHang> dsDon = new List<DonHang>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT ""OrderID"", ""Status"", ""PaymentMethod"" 
                         FROM ""Orders""";

                if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả")
                {
                    query += " WHERE \"Status\" = @trangThai";
                }

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả")
                        cmd.Parameters.AddWithValue("@trangThai", trangThai);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dsDon.Add(new DonHang
                            {
                                DonHangID = reader.GetInt32(0),
                                TrangThai = reader.GetString(1),
                                TenKhach = reader.IsDBNull(2) ? "(Không có)" : reader.GetString(2) // dùng PaymentMethod làm ví dụ
                            });
                        }
                    }
                }
            }

            listViewDonHang.ItemsSource = dsDon;
        }

        // Load chi tiết đơn hàng
        public void LoadChiTietDon(int donHangID)
        {
            List<ChiTietDonHang> chiTiet = new List<ChiTietDonHang>();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT m.""Name"", d.""Quantity"", d.""ItemPrice"", d.""Note""
                         FROM ""OrderDetails"" d
                         INNER JOIN ""MenuItems"" m ON d.""ItemID"" = m.""ItemID""
                         WHERE d.""OrderID"" = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", donHangID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chiTiet.Add(new ChiTietDonHang
                            {
                                TenMon = reader.GetString(0),
                                SoLuong = reader.GetInt32(1),
                                Gia = reader.GetDecimal(2),
                                GhiChu = reader.IsDBNull(3) ? "" : reader.GetString(3)
                            });
                        }
                    }
                }
            }

            listViewChiTiet.ItemsSource = chiTiet;
        }

        // Tạo đơn hàng mới
        public void TaoDonHang(int banID, List<ChiTietDonHang> danhSachMon, string tenKhach)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        string insertDon = @"INSERT INTO ""Orders"" 
                    (""TableID"", ""Status"", ""OrderTime"", ""PaymentMethod"", ""Paid"") 
                    VALUES (@banID, 'Chờ xử lý', CURRENT_TIMESTAMP, @payment, FALSE)
                    RETURNING ""OrderID""";
                        int donHangID;

                        using (var cmd = new NpgsqlCommand(insertDon, conn))
                        {
                            cmd.Parameters.AddWithValue("@banID", banID);
                            cmd.Parameters.AddWithValue("@payment", tenKhach); // tạm dùng làm phương thức thanh toán
                            donHangID = (int)cmd.ExecuteScalar();
                        }

                        foreach (var item in danhSachMon)
                        {
                            string insertChiTiet = @"INSERT INTO ""OrderDetails"" 
                        (""OrderID"", ""ItemID"", ""Quantity"", ""ItemPrice"", ""Note"") 
                        VALUES (@orderID, (SELECT ""ItemID"" FROM ""MenuItems"" WHERE ""Name"" = @itemName), @quantity, @price, @note)";
                            using (var cmd = new NpgsqlCommand(insertChiTiet, conn))
                            {
                                cmd.Parameters.AddWithValue("@orderID", donHangID);
                                cmd.Parameters.AddWithValue("@itemName", item.TenMon);
                                cmd.Parameters.AddWithValue("@quantity", item.SoLuong);
                                cmd.Parameters.AddWithValue("@price", item.Gia);
                                cmd.Parameters.AddWithValue("@note", item.GhiChu ?? "");
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        MessageBox.Show("Tạo đơn hàng thành công!");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        MessageBox.Show("Lỗi khi tạo đơn: " + ex.Message);
                    }
                }
            }

            LoadDonHang(); // Reload sau khi thêm
        }

        private void comboTrangThai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedStatus = (comboTrangThai.SelectedItem as ComboBoxItem)?.Content.ToString();
            LoadDonHang(selectedStatus);
        }

        private async void LoadBanVaMon()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    // --- Lấy danh sách bàn ---
                    var cmdBan = new NpgsqlCommand(@"SELECT ""TableID"", ""TableNumber"" FROM ""Tables"" ORDER BY ""TableID""", conn);
                    using (var reader = await cmdBan.ExecuteReaderAsync())
                    {
                        var listBan = new List<BanAn>();
                        while (await reader.ReadAsync())
                        {
                            listBan.Add(new BanAn
                            {
                                TableID = reader.GetInt32(0),
                                TableNumber = reader.GetString(1)
                            });
                        }
                        comboBoxBan.ItemsSource = listBan;
                        comboBoxBan.DisplayMemberPath = "TableNumber";
                        comboBoxBan.SelectedValuePath = "TableID";
                    }

                    // --- Lấy danh sách món ---
                    var cmdMon = new NpgsqlCommand(@"SELECT ""ItemID"", ""ItemName"" FROM ""MenuItems"" ORDER BY ""ItemName""", conn);
                    using (var reader = await cmdMon.ExecuteReaderAsync())
                    {
                        var listMon = new List<MonAn>();
                        while (await reader.ReadAsync())
                        {
                            listMon.Add(new MonAn
                            {
                                ItemID = reader.GetInt32(0),
                                ItemName = reader.GetString(1)
                            });
                        }
                        comboBoxMon.ItemsSource = listMon;
                        comboBoxMon.DisplayMemberPath = "ItemName";
                        comboBoxMon.SelectedValuePath = "ItemID";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách bàn/món: " + ex.Message);
            }
        }
    }
}
