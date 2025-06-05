using FinalProject.Models;
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
    /// Interaction logic for ThanhToan.xaml
    /// </summary>
    public partial class ThanhToan : Window
    {
        // Chuỗi kết nối đến cơ sở dữ liệu PostgreSQL
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        private Dictionary<string, decimal> giaTienTheoBan = new Dictionary<string, decimal>
        {
            { "Bàn 1", 150000 },
            { "Bàn 2", 180000 },
            { "Bàn 3", 200000 },
            { "Bàn 4", 220000 },
            { "Bàn 5", 170000 },
            { "Bàn 6", 190000 },
            { "Bàn 7", 210000 },
            { "Bàn 8", 230000 },
            { "Bàn 9", 250000 }
        };
        public ThanhToan(string maDonHang, decimal tongTien)
        {
            InitializeComponent();

            for (int i = 1; i <= 9; i++)
                cbBanAn.Items.Add($"Bàn {i}");

            txtMaDonHang.Text = maDonHang;
            txtTongTien.Text = tongTien.ToString("N0");

            // Danh sách phương thức thanh toán
            cbPhuongThuc.Items.Add("Tiền mặt");
            cbPhuongThuc.Items.Add("ViettinBank");
            cbPhuongThuc.Items.Add("MoMo");
            cbPhuongThuc.Items.Add("ZaloPay");
            cbPhuongThuc.SelectedIndex = 0;
        }

        private void CbBanAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbBanAn.SelectedItem is string tenBan)
            {
                string maDon = "HD" + DateTime.Now.Ticks.ToString().Substring(10); // mã hóa đơn tạm thời
                txtMaDonHang.Text = maDon;
                txtTongTien.Text = giaTienTheoBan[tenBan].ToString("N0");
            }
        }

        private void CbPhuongThuc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string phuongThuc = cbPhuongThuc.SelectedItem?.ToString();
            string imagePath = "";

            switch (phuongThuc)
            {
                case "MoMo":
                    imagePath = "Images/Logo/MoMo.png";
                    break;
                case "ViettinBank":
                    imagePath = "Images/Logo/ViettinBank.png";
                    break;
                case "ZaloPay":
                    imagePath = "Images/Logo/ZaloPay.png";
                    break;
                default:
                    imagePath = ""; // không hiển thị
                    break;
            }
            if (!string.IsNullOrEmpty(imagePath))
                imgQRCode.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            else
                imgQRCode.Source = null;

        }

        private void Btn_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            string maDon = txtMaDonHang.Text;
            string tongTien = txtTongTien.Text;
            string phuongThuc = cbPhuongThuc.Text;
            string tenBan = cbBanAn.Text;

            if (string.IsNullOrEmpty(tenBan))
            {
                MessageBox.Show("Vui lòng chọn bàn ăn trước khi thanh toán.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show(
                $"Đã thanh toán đơn hàng {maDon}\nTổng tiền: {tongTien} VNĐ\nPhương thức: {phuongThuc}",
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }
    }
    
}
