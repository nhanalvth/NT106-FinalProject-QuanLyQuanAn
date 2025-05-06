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

            // Chuỗi kết nối - bạn cần chỉnh sửa server, user, pass cho phù hợp
            string connectionString = "Server=192.168.1.135;Database=QUANANDB;User Id=appuser;Password=123;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT FullName, Role FROM Users WHERE Username = @username AND PasswordHash = @password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fullName = reader["FullName"].ToString();
                                string role = reader["Role"].ToString();

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
    }
}
