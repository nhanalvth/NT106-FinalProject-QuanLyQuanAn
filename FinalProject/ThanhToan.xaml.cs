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
using System.Windows.Shapes;

namespace FinalProject
{
    public partial class ThanhToan : Window
    {
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        public ThanhToan()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                LoadDonHangCho();
                cbPhuongThuc.ItemsSource = new List<string> { "Tiền mặt", "ViettinBank", "MoMo", "ZaloPay" };
                cbPhuongThuc.SelectedIndex = 0;
                cbHoaDon.SelectionChanged += cbHoaDon_SelectionChanged;
                cbPhuongThuc.SelectionChanged += cbPhuongThuc_SelectionChanged;
            };
        }

        public ThanhToan(string maDonHang, decimal tongTien) : this()
        {
            txtMaDonHang.Text = maDonHang;
            txtTongTien.Text = tongTien.ToString("N0");
        }

        public class DonHangTam
        {
            public int OrderID { get; set; }
            public string Ban { get; set; }
            public decimal TongTien { get; set; }
        }

        private void LoadDonHangCho()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                    SELECT o.orderid, t.tablenumber, o.totalamount
                    FROM orders o
                    JOIN tables t ON o.tableid = t.tableid
                    WHERE o.paid = FALSE AND (o.status = 'Chờ xử lý' OR o.status = 'Đang phục vụ')
                    ORDER BY o.ordertime DESC";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var ds = new List<DonHangTam>();

                        while (reader.Read())
                        {
                            ds.Add(new DonHangTam
                            {
                                OrderID = reader.GetInt32(0),
                                Ban = reader.GetString(1),
                                TongTien = reader.GetDecimal(2)
                            });
                        }

                        cbHoaDon.ItemsSource = ds;
                        cbHoaDon.DisplayMemberPath = "OrderID";
                        cbHoaDon.SelectedValuePath = "OrderID"; // nếu cần truy xuất ID
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải đơn hàng: " + ex.Message);
            }
        }

        private void cbHoaDon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbHoaDon.SelectedItem is DonHangTam don)
            {
                txtMaDonHang.Text = $"HD{don.OrderID}";
                txtTongTien.Text = don.TongTien.ToString("N0");
                cbBanAn.Text = $"Bàn {don.Ban}";
            }
        }

        private void cbPhuongThuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string phuongThuc = cbPhuongThuc.SelectedItem?.ToString();
            string imagePath = phuongThuc switch
            {
                "MoMo" => "Images/Logo/MoMo.png",
                "ViettinBank" => "Images/Logo/ViettinBank.png",
                "ZaloPay" => "Images/Logo/ZaloPay.png",
                _ => ""
            };
            imgQRCode.Source = string.IsNullOrEmpty(imagePath) ? null : new BitmapImage(new Uri(imagePath, UriKind.Relative));
        }

        private void Btn_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            if (cbHoaDon.SelectedItem is not DonHangTam selected)
            {
                MessageBox.Show("Vui lòng chọn đơn hàng cần thanh toán.", "Lưu ý", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string phuongThuc = cbPhuongThuc.Text;
            using var conn = new NpgsqlConnection(connectionString);

        }
    }
}