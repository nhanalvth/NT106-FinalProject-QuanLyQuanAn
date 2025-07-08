using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using Npgsql;

namespace FinalProject
{
    public partial class ForgotPassword : Window
    {
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        public ForgotPassword()
        {
            InitializeComponent();
        }

        private void btn_SendRequest_Click(object sender, RoutedEventArgs e)
        {
            string username = textBox_Username.Text.Trim();
            string email = textBox_Email.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và email.", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Địa chỉ email không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                // Kiểm tra tài khoản tồn tại
                string selectQuery = "SELECT email FROM users WHERE username = @username";
                using var selectCmd = new NpgsqlCommand(selectQuery, conn);
                selectCmd.Parameters.AddWithValue("@username", username);

                var result = selectCmd.ExecuteScalar();
                if (result == null)
                {
                    MessageBox.Show("Tên đăng nhập không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string dbEmail = result.ToString();
                if (!dbEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Email không khớp với tên đăng nhập!", "Lỗi xác minh", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Tạo mật khẩu tạm thời
                string tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8);

                // Gửi email
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential("nhom14nt106@gmail.com", "usmi djnc ilgh vluo")
                };

                string body = $@"
                    <h3>Yêu cầu khôi phục mật khẩu</h3>
                    <p>Bạn vừa yêu cầu khôi phục mật khẩu. Mật khẩu tạm thời của bạn là:</p>
                    <h2>{tempPassword}</h2>
                    <p>Vui lòng đăng nhập và thay đổi mật khẩu sau khi đăng nhập thành công.</p>";

                var message = new MailMessage
                {
                    From = new MailAddress("nhom14nt106@gmail.com", "Hệ thống Quản lý Nhà hàng"),
                    Subject = "Khôi phục mật khẩu",
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(email);
                smtp.Send(message);

                // Cập nhật mật khẩu mới vào DB (ở đây bạn có thể mã hóa nếu cần)
                string updateQuery = "UPDATE users SET password = @password WHERE username = @username";
                using var updateCmd = new NpgsqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@password", tempPassword);
                updateCmd.Parameters.AddWithValue("@username", username);
                updateCmd.ExecuteNonQuery();

                MessageBox.Show("Mật khẩu tạm thời đã được gửi đến email của bạn!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể khôi phục mật khẩu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
