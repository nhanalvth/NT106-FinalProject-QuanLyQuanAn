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
            string username = textBox_Username.Text;
            string password = textBox_Password.Password;
            string confirmPassword = textBox_ConfirmPassword.Password;

            // Kiểm tra mật khẩu nhập lại
            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Giả lập lưu tài khoản (có thể thay bằng lưu vào Database)
            MessageBox.Show($"Tạo tài khoản thành công!\nTài khoản: {username}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            // Đóng cửa sổ đăng ký
            this.Close();
        }
    }
}

