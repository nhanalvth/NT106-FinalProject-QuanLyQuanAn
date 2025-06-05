using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for QlyBanAn.xaml
    /// </summary>
    public partial class QlyBanAn : Page
    {
        // Chuỗi kết nối đến cơ sở dữ liệu PostgreSQL
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        public QlyBanAn()
        {
            InitializeComponent();          
        }
        private void Ban_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string banSo = btn.Content.ToString();
            string trangThai = btn.Tag.ToString();

            MessageBox.Show($"{banSo} - Trạng thái: {HienThiTrangThai(trangThai)}", "Thông tin bàn");
        }

        private string HienThiTrangThai(string tag)
        {
            return tag switch
            {
                "Trong" => "Bàn trống",
                "DangPhucVu" => "Đang phục vụ",
                "DaThanhToan" => "Đã thanh toán",
                "ChuaDonDep" => "Chưa dọn dẹp",
                _ => "Không xác định"
            };
        }
    }
}
