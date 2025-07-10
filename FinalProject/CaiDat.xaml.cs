using FinalProject.Models;
using Microsoft.Win32;
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
    /// Interaction logic for CaiDat.xaml
    /// </summary>
    public partial class CaiDat : Page
    {
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        public CaiDat()
        {
            InitializeComponent();
            txtUsername.Text = UserSession.UserName;
            LoadThongTinQuan(); // ✅ Tải thông tin quán khi khởi tạo trang
        }

        private void BtnChonLogo_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show($"Logo đã chọn: {dialog.FileName}", "Thông báo");
            }
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra vai trò
            if (UserSession.Role != "Admin")
            {
                MessageBox.Show("Chỉ tài khoản Admin mới được phép thay đổi cài đặt.", "Quyền hạn bị từ chối", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string tenQuan = txtTenQuan.Text;
            string diaChi = txtDiaChi.Text;
            string sdt = txtSDT.Text;
            string vatText = txtVAT.Text;
            bool notifyEmail = chkNotifyEmail.IsChecked ?? false;
            bool notifySMS = chkNotifySMS.IsChecked ?? false;

            if (!decimal.TryParse(vatText, out decimal vat))
            {
                MessageBox.Show("VAT phải là số hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var conn = new Npgsql.NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                INSERT INTO settings (storename, address, phone, vatrate, notifybyemail, notifybysms)
                VALUES (@tenQuan, @diaChi, @sdt, @vat, @email, @sms)
                ON CONFLICT (settingid) DO UPDATE SET
                    storename = EXCLUDED.storename,
                    address = EXCLUDED.address,
                    phone = EXCLUDED.phone,
                    vatrate = EXCLUDED.vatrate,
                    notifybyemail = EXCLUDED.notifybyemail,
                    notifybysms = EXCLUDED.notifybysms;";

                    using (var cmd = new Npgsql.NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@tenQuan", tenQuan);
                        cmd.Parameters.AddWithValue("@diaChi", diaChi);
                        cmd.Parameters.AddWithValue("@sdt", sdt);
                        cmd.Parameters.AddWithValue("@vat", vat);
                        cmd.Parameters.AddWithValue("@email", notifyEmail);
                        cmd.Parameters.AddWithValue("@sms", notifySMS);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Đã lưu cài đặt thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu cài đặt: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadThongTinQuan()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT storename, address, phone, vatrate, notifybyemail, notifybysms FROM settings LIMIT 1";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtTenQuan.Text = reader.GetString(0);
                        txtDiaChi.Text = reader.GetString(1);
                        txtSDT.Text = reader.GetString(2);
                        txtVAT.Text = reader.GetDecimal(3).ToString();
                        chkNotifyEmail.IsChecked = reader.GetBoolean(4);
                        chkNotifySMS.IsChecked = reader.GetBoolean(5);
                    }
                }
            }
        }


    }
}

