using FinalProject.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FinalProject
{
    public partial class QlyDonHang : Page
    {
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        private List<ChiTietDonHang> danhSachTamThoi = new List<ChiTietDonHang>();

        public QlyDonHang()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                comboTrangThai.SelectionChanged += comboTrangThai_SelectionChanged;
                LoadBanVaMon();
                LoadDonHang();
            };
        }

        public void LoadDonHang(string trangThai = "")
        {
            List<DonHang> dsDon = new List<DonHang>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                SELECT o.orderid, o.status, t.tableid, o.ordertime, o.totalamount
                FROM orders o
                LEFT JOIN tables t ON o.tableid = t.tableid";

                if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả")
                {
                    query += " WHERE o.status = @trangThai";
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
                                DonHangID = reader.GetInt32(reader.GetOrdinal("orderid")),
                                TrangThai = reader.GetString(reader.GetOrdinal("status")),
                                TenBan = reader.IsDBNull(reader.GetOrdinal("tableid")) ? "(Không có)" : $"Bàn {reader.GetInt32(reader.GetOrdinal("tableid"))}",
                                GioDat = reader.GetDateTime(reader.GetOrdinal("ordertime")),
                                TongTien = reader.IsDBNull(reader.GetOrdinal("totalamount")) ? 0 : reader.GetDecimal(reader.GetOrdinal("totalamount"))
                            });
                        }
                    }
                }
            }

            listViewDonHang.ItemsSource = dsDon;
        }

        public void LoadChiTietDon(int donHangID)
        {
            List<ChiTietDonHang> chiTiet = new List<ChiTietDonHang>();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                SELECT m.itemname, d.quantity, d.itemprice, d.note
                FROM orderdetails d
                JOIN menuitems m ON d.itemid = m.itemid
                WHERE d.orderid = @id";

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

        public void TaoDonHang(int banID, List<ChiTietDonHang> danhSachMon, string paymentMethod)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        string insertDon = @"
                            INSERT INTO orders (tableid, status, ordertime, paymentmethod, paid)
                            VALUES (@banID, 'Chờ xử lý', CURRENT_TIMESTAMP, @payment, FALSE)
                            RETURNING orderid";

                        int donHangID;
                        using (var cmd = new NpgsqlCommand(insertDon, conn))
                        {
                            cmd.Parameters.AddWithValue("@banID", banID);
                            cmd.Parameters.AddWithValue("@payment", paymentMethod);
                            donHangID = (int)cmd.ExecuteScalar();
                        }

                        foreach (var item in danhSachMon)
                        {
                            string insertChiTiet = @"
                                INSERT INTO orderdetails 
                                (orderid, itemid, quantity, itemprice, note) 
                                VALUES (
                                    @orderID,
                                    (SELECT itemid FROM menuitems WHERE itemname = @itemName),
                                    @quantity, @price, @note
                                )";

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

            LoadDonHang();
        }

        private void btnThemMon_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxMon.SelectedItem is MonAn mon)
            {
                if (!int.TryParse(textBoxSoLuong.Text, out int soLuong) || soLuong <= 0)
                {
                    MessageBox.Show("Số lượng phải là số nguyên dương.");
                    return;
                }

                try
                {
                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"SELECT price FROM menuitems WHERE itemid = @id";
                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", mon.ItemID);
                            var result = cmd.ExecuteScalar();

                            if (result != null && decimal.TryParse(result.ToString(), out decimal gia))
                            {
                                var item = new ChiTietDonHang
                                {
                                    TenMon = mon.ItemName,
                                    SoLuong = soLuong,
                                    Gia = gia,
                                    GhiChu = textBoxGhiChu.Text.Trim()
                                };

                                danhSachTamThoi.Add(item);
                                listViewChiTiet.ItemsSource = null;
                                listViewChiTiet.ItemsSource = danhSachTamThoi;

                                // Xóa nội dung ghi chú và reset số lượng sau khi thêm
                                textBoxGhiChu.Clear();
                                textBoxSoLuong.Text = "1";
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy giá món ăn.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi truy vấn giá món ăn: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn món ăn.");
            }
        }

        private void btnTaoDonHang_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxBan.SelectedItem is BanAn ban)
            {
                if (danhSachTamThoi.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất một món.");
                    return;
                }

                TaoDonHang(ban.TableID, danhSachTamThoi, "Tiền mặt");
                danhSachTamThoi.Clear();
                listViewChiTiet.ItemsSource = null;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn bàn.");
            }
        }

        private void comboTrangThai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedStatus = (comboTrangThai.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            LoadDonHang(selectedStatus);
        }

        private void listViewDonHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewDonHang.SelectedItem is DonHang selected)
            {
                LoadChiTietDon(selected.DonHangID);
            }
        }

        private async void LoadBanVaMon()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    var listBan = new List<BanAn>();
                    using (var cmdBan = new NpgsqlCommand(@"SELECT tableid, tablenumber FROM tables ORDER BY tableid", conn))
                    using (var reader = await cmdBan.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            listBan.Add(new BanAn
                            {
                                TableID = reader.GetInt32(0),
                                TableNumber = reader.GetString(1)
                            });
                        }
                    }
                    comboBoxBan.ItemsSource = listBan;
                    comboBoxBan.DisplayMemberPath = "TableNumber";
                    comboBoxBan.SelectedValuePath = "TableID";

                    var listMon = new List<MonAn>();
                    using (var cmdMon = new NpgsqlCommand(@"SELECT itemid, itemname FROM menuitems ORDER BY itemname", conn))
                    using (var reader = await cmdMon.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            listMon.Add(new MonAn
                            {
                                ItemID = reader.GetInt32(0),
                                ItemName = reader.GetString(1)
                            });
                        }
                    }
                    comboBoxMon.ItemsSource = listMon;
                    comboBoxMon.DisplayMemberPath = "ItemName";
                    comboBoxMon.SelectedValuePath = "ItemID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách bàn/món: " + ex.Message);
            }
        }

        private void BtnChapNhan_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int donHangId)
            {
                CapNhatTrangThaiDonHang(donHangId, "Đang phục vụ");
            }
        }

        private void BtnHuy_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int donHangId)
            {
                var result = MessageBox.Show("Bạn có chắc muốn hủy đơn hàng này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    CapNhatTrangThaiDonHang(donHangId, "Đã hủy");
                }
            }
        }

        private void CapNhatTrangThaiDonHang(int donHangId, string trangThaiMoi)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE orders SET status = @status WHERE orderid = @id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", trangThaiMoi);
                        cmd.Parameters.AddWithValue("@id", donHangId);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show($"Cập nhật trạng thái đơn hàng #{donHangId} thành '{trangThaiMoi}' thành công.");
                            LoadDonHang(); // Hàm bạn đang dùng để load lại ListView
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái đơn hàng: " + ex.Message);
            }
        }

        private void btnChonBan_Click(object sender, RoutedEventArgs e)
        {
            QlyBanAn.OnBanDuocChon = (banChon) =>
            {
                comboBoxBan.SelectedItem = comboBoxBan.Items
                    .OfType<BanAn>()
                    .FirstOrDefault(b => b.TableNumber == banChon.TableNumber);
            };

            NavigationService.Navigate(new QlyBanAn());
        }


        private void btnChonMon_Click(object sender, RoutedEventArgs e)
        {
            QlyThucDon.OnMonAnDuocChon = (dsMonChon) =>
            {
                foreach (var mon in dsMonChon)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        TenMon = mon.ItemName,
                        SoLuong = 1, // Hoặc để người dùng chỉnh sau
                        Gia = mon.Gia,
                        GhiChu = ""
                    };

                    danhSachTamThoi.Add(chiTiet);
                }

                listViewChiTiet.ItemsSource = null;
                listViewChiTiet.ItemsSource = danhSachTamThoi;
            };

            NavigationService.Navigate(new QlyThucDon());
        }



    }
}
