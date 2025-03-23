using FinalProject;
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
            // Kiểm tra thông tin đăng nhập (giả sử tài khoản là admin / 123)
            if (textBox_Username.Text == "admin" && textBox_Password.Text == "123")
            {
                // Đánh dấu đăng nhập thành công
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btn_Singup_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount registerWindow = new CreateAccount();
            registerWindow.ShowDialog(); // Hiển thị cửa sổ đăng ký
        }
    }
}
