using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace FinalProject
{
    public partial class CreateAccount : Window
    {
        public CreateAccount()
        {
            InitializeComponent();
        }

        private void btn_Singup_Click(object sender, RoutedEventArgs e)
        {
            string username = textBox_Username.Text.Trim();
            string password = textBox_Password.Password.Trim();
            string confirmPassword = textBox_ConfirmPassword.Password.Trim();
            string fullName = textBox_FullName.Text.Trim();
            string email = textBox_Email.Text.Trim();
            string phone = textBox_Phone.Text.Trim();
            string role = ((ComboBoxItem)comboBox_Role.SelectedItem)?.Content?.ToString();

            // Vai trò DB
            role = role switch
            {
                "Nhân viên" => "nhanvien",
                "Admin" => "admin",
                "Thu ngân" => "thungan",
                "Bếp" => "bep",
                _ => null
            };

            if (role == null)
            {
                MessageBox.Show("Chức vụ không hợp lệ.");
                return;
            }

            // Validate thông tin
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) ||
                string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.");
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ.");
                return;
            }

            if (!Regex.IsMatch(phone, @"^0\d{9,10}$"))
            {
                MessageBox.Show("Số điện thoại không hợp lệ.");
                return;
            }

            string hashedPassword = password; // TODO: Replace with SHA256 or bcrypt if needed

            try
            {
                string connStr = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

                using var conn = new NpgsqlConnection(connStr);
                conn.Open();

                string query = @"
                    INSERT INTO users (username, password, fullname, email, phone, role, isactive)
                    VALUES (@username, @password, @fullname, @email, @phone, @role, false)";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", hashedPassword);
                cmd.Parameters.AddWithValue("@fullname", fullName);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@role", role);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    GuiEmailXacNhan(email, fullName);
                    MessageBox.Show("Tạo tài khoản thành công! Vui lòng kiểm tra email để xác nhận.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Tạo tài khoản thất bại.");
                }
            }
            catch (PostgresException pgEx) when (pgEx.SqlState == "23505")
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void GuiEmailXacNhan(string toEmail, string fullName)
        {
            try
            {
                var fromEmail = "nhom14nt106@gmail.com";
                var fromPassword = "usmi djnc ilgh vluo"; // App password
                var subject = "Xác nhận tài khoản";

                var body = $@"
                    <html>
                        <body style='font-family: Arial;'>
                            <h3>Xin chào {fullName},</h3>
                            <p>Tài khoản của bạn đã được tạo thành công.</p>
                            <p>Vui lòng liên hệ quản trị viên để được kích hoạt.</p>
                            <br/>
                            <p>Trân trọng,<br/>Hệ thống Quản lý Nhà hàng</p>
                        </body>
                    </html>";

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(fromEmail, fromPassword)
                };

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Nhóm 14 - Quản lý nhà hàng", System.Text.Encoding.UTF8),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    BodyEncoding = System.Text.Encoding.UTF8,
                    SubjectEncoding = System.Text.Encoding.UTF8
                };

                message.To.Add(toEmail);
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể gửi email xác nhận: " + ex.Message, "Lỗi gửi email", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
