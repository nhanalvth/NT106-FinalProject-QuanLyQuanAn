using FinalProject.Models;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for CaiDat.xaml
    /// </summary>
    public partial class CaiDat : Page
    {
        public CaiDat()
        {
            InitializeComponent();
            txtUsername.Text = UserSession.UserName;
        }

        private void BtnChonLogo_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show($"Logo đã chọn: {dialog.FileName}", "Thông báo");
            }
        }

        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            string tenQuan = txtTenQuan.Text;
            string diaChi = txtDiaChi.Text;
            string sdt = txtSDT.Text;
            string vaiTro = ((ComboBoxItem)cbPhanQuyen.SelectedItem)?.Content.ToString();
            string vat = txtVAT.Text;
            bool thongBao = chkThongBao.IsChecked ?? false;
            string ngonNgu = ((ComboBoxItem)cbNgonNgu.SelectedItem)?.Content.ToString();
            string cheDo = ((ComboBoxItem)cbCheDo.SelectedItem)?.Content.ToString();

            MessageBox.Show($"Đã lưu cài đặt:\n- Tên quán: {tenQuan}\n- Vai trò: {vaiTro}\n- VAT: {vat}%\n- Ngôn ngữ: {ngonNgu}\n- Chế độ: {cheDo}\n- Thông báo: {(thongBao ? "Bật" : "Tắt")}",
                            "Lưu thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

