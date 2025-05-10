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

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for QlyNhanVien.xaml
    /// </summary>
    public partial class QlyNhanVien : Page
    {
        public ObservableCollection<NhanVien> DanhSachNhanVien = new ObservableCollection<NhanVien>();
        public QlyNhanVien()
        {
            InitializeComponent();
            dgNhanVien.ItemsSource = DanhSachNhanVien;
        }
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            var nv = new NhanVien
            {
                MaNV = txtMaNV.Text,
                TenNV = txtTenNV.Text,
                VaiTro = ((ComboBoxItem)cbVaiTro.SelectedItem)?.Content.ToString(),
                IsQuanLy = chkQuanLy.IsChecked ?? false,
                CaLamViec = txtCaLam.Text
            };
            DanhSachNhanVien.Add(nv);
            ClearForm();
        }

        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgNhanVien.SelectedItem is NhanVien nv)
            {
                nv.TenNV = txtTenNV.Text;
                nv.VaiTro = ((ComboBoxItem)cbVaiTro.SelectedItem)?.Content.ToString();
                nv.IsQuanLy = chkQuanLy.IsChecked ?? false;
                nv.CaLamViec = txtCaLam.Text;
                dgNhanVien.Items.Refresh();
            }
        }

        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgNhanVien.SelectedItem is NhanVien nv)
            {
                DanhSachNhanVien.Remove(nv);
            }
        }

        private void ClearForm()
        {
            txtMaNV.Clear();
            txtTenNV.Clear();
            cbVaiTro.SelectedIndex = -1;
            chkQuanLy.IsChecked = false;
            txtCaLam.Clear();
        }
    }
    public class NhanVien
    {
        public string MaNV { get; set; }
        public string TenNV { get; set; }
        public string VaiTro { get; set; }
        public bool IsQuanLy { get; set; }
        public string CaLamViec { get; set; }
    }
}

