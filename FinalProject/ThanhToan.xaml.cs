﻿using FinalProject.Models;
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
            string nhanVien = UserSession.UserName; // hoặc bạn có thể thay bằng người đăng nhập

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                using var tran = conn.BeginTransaction();

                // 1. Cập nhật trạng thái đơn hàng là đã thanh toán
                string updateQuery = @"
            UPDATE orders 
            SET paid = TRUE, status = 'Đã thanh toán'
            WHERE orderid = @orderid";

                using (var cmdUpdate = new NpgsqlCommand(updateQuery, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@orderid", selected.OrderID);
                    cmdUpdate.ExecuteNonQuery();
                }

                // 2. Tính toán các phần cần thiết
                decimal total = selected.TongTien;
                decimal tax = total * 0.1m;
                decimal finalAmount = total + tax;
                DateTime issueDate = DateTime.Now;

                // 3. Thêm hóa đơn vào bảng bills
                string insertQuery = @"
            INSERT INTO bills (orderid, issuedate, total, tax, finalamount, paymentmethod, ""user"")
            VALUES (@orderid, @issuedate, @total, @tax, @finalamount, @paymentmethod, @user)";

                using (var cmdInsert = new NpgsqlCommand(insertQuery, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@orderid", selected.OrderID);
                    cmdInsert.Parameters.AddWithValue("@issuedate", issueDate);
                    cmdInsert.Parameters.AddWithValue("@total", total);
                    cmdInsert.Parameters.AddWithValue("@tax", tax);
                    cmdInsert.Parameters.AddWithValue("@finalamount", finalAmount);
                    cmdInsert.Parameters.AddWithValue("@paymentmethod", phuongThuc);
                    cmdInsert.Parameters.AddWithValue("@user", nhanVien);

                    cmdInsert.ExecuteNonQuery();
                }

                tran.Commit();

                MessageBox.Show("Thanh toán thành công và đã lưu hóa đơn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Cập nhật lại danh sách đơn hàng chưa thanh toán
                LoadDonHangCho();
                txtMaDonHang.Text = "";
                txtTongTien.Text = "";
                cbBanAn.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thanh toán: " + ex.Message);
            }
        }

    }
}