using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            string role = ((ComboBoxItem)comboBox_Role.SelectedItem).Content.ToString();

            // Chuyển đổi từ các tên tiếng Việt thành chuỗi phù hợp
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

            if (username == "" || password == "" || confirmPassword == "" || fullName == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.");
                return;
            }

            // Mã hóa mật khẩu đơn giản (có thể thay bằng SHA256 hoặc BCrypt)
            string hashedPassword = password; // hoặc viết hàm hash nếu cần

            try
            {
                string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO \"Users\" (\"Username\", \"Password\", \"FullName\", \"Role\") VALUES (@Username, @Password, @FullName, @Role)";
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Role", role);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Tạo tài khoản thành công!");
                        this.Close(); // hoặc chuyển sang màn hình đăng nhập
                    }
                    else
                    {
                        MessageBox.Show("Tạo tài khoản thất bại.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}

