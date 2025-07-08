using FinalProject;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            string username = textBox_Username.Text.Trim();
            string password = textBox_Password.Text.Trim();
            
            //Xóa IF bên dưới nếu chạy database
            //bool devMode = true;
            //if (devMode)
            //{
            //    // Bỏ qua SQL, cho phép đăng nhập giả lập
            //    MessageBox.Show($"[DEV MODE] Đăng nhập giả lập thành công!\nXin chào: Admin (Dev)", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            //    MainWindow main = new MainWindow();
            //    main.Show();
            //    return;
            //}

            // Chuỗi kết nối - bạn cần chỉnh sửa server, user, pass cho phù hợp
            string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT fullname, role FROM users WHERE username = @username AND password = @password";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fullName = reader["fullname"].ToString();
                                string role = reader["role"].ToString();

                                switch (role)
                                {
                                    case "nhanvien":
                                        role = "Nhân viên";
                                        break;
                                    case "admin":
                                        role = "Admin";
                                        break;
                                    case "thungan":
                                        role = "Thu ngân";
                                        break;
                                    case "bep":
                                        role = "Bếp";
                                        break;
                                    default:
                                        MessageBox.Show("Chức vụ không hợp lệ.");
                                        return;
                                }

                                MessageBox.Show($"Đăng nhập thành công!\nXin chào: {fullName} ({role})", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                                MainWindow main = new MainWindow();
                                main.Show();
                            }
                            else
                            {
                                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối:\n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btn_Singup_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount registerWindow = new CreateAccount();
            registerWindow.ShowDialog(); // Hiển thị cửa sổ đăng ký
        }

        private void btn_Quenmk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var forgotWindow = new ForgotPassword();
                forgotWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không mở được cửa sổ Quên mật khẩu:\n" + ex.Message);
            }
        }
    }
}
