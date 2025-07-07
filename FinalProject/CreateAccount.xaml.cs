using System;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for CreateAccount.xaml
    /// </summary>
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
            string role = ((ComboBoxItem)comboBox_Role.SelectedItem)?.Content?.ToString();

            // Chuyển đổi tiếng Việt sang vai trò trong DB
            switch (role)
            {
                case "Nhân viên":
                    role = "nhanvien";
                    break;
                case "Admin":
                    role = "admin";
                    break;
                case "Thu ngân":
                    role = "thungan";
                    break;
                case "Bếp":
                    role = "bep";
                    break;
                default:
                    MessageBox.Show("Chức vụ không hợp lệ.");
                    return;
            }

            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) ||
                string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.");
                return;
            }

            // TODO: Hash password ở đây nếu cần (SHA256/Bcrypt)
            string hashedPassword = password;

            try
            {
                string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO users (username, password, fullname, role) 
                        VALUES (@username, @password, @fullname, @role)";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);
                        cmd.Parameters.AddWithValue("@fullname", fullName);
                        cmd.Parameters.AddWithValue("@role", role);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Tạo tài khoản thành công!");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Tạo tài khoản thất bại.");
                        }
                    }
                }
            }
            catch (PostgresException pgEx) when (pgEx.SqlState == "23505")
            {
                // 23505 = unique_violation
                MessageBox.Show("Tên đăng nhập đã tồn tại!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}
