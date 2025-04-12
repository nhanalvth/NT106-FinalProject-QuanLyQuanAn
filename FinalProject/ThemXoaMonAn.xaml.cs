#nullable enable
using FinalProject.Models;
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
using System.Windows.Shapes;
using Microsoft.Win32;

namespace FinalProject
{
    public partial class ThemXoaMonAn : Window
    {
        //public ObservableCollection<ThucDon> ThucDons { get; set; }
        public ObservableCollection<ThucDon> ThucDons { get; set; } = new();

        public ThemXoaMonAn(ObservableCollection<ThucDon> listMonChinh)
        {
            InitializeComponent();
        }

        #region === THÊM MÓN ===

        private void btn_Them_Click(object sender, RoutedEventArgs e)
        {
            if (cbThem_LoaiMonAn.SelectedItem is ComboBoxItem selectedItem &&
                !string.IsNullOrWhiteSpace(cbAddName.Text) &&
                !string.IsNullOrWhiteSpace(cbAddImage.Text) &&
                double.TryParse(cbAddRating.Text, out double rating))
            {
                ThucDon monMoi = new ThucDon
                {
                    Name = cbAddName.Text,
                    ImagePath = cbAddImage.Text,
                    Rating = rating
                };

                switch (selectedItem.Content.ToString())
                {
                    case "Món chính":
                        ThucDonData.Instance.ListMonChinh.Add(monMoi); break;
                    case "Món tráng miệng":
                        ThucDonData.Instance.ListMonTrangMieng.Add(monMoi); break;
                    case "Trà trái cây":
                        ThucDonData.Instance.ListTraTraiCay.Add(monMoi); break;
                    case "Nước ép":
                        ThucDonData.Instance.ListNuocEp.Add(monMoi); break;
                    case "Món khuyến mãi":
                        ThucDonData.Instance.ListMonKhuyenMai.Add(monMoi); break;
                    case "Món mới":
                        ThucDonData.Instance.ListMonMoi.Add(monMoi); break;
                    case "Món bán chạy":
                        ThucDonData.Instance.ListMonBanChay.Add(monMoi); break;
                    default:
                        MessageBox.Show("Danh mục không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                cbAddName.Clear();
                cbAddImage.Clear();
                cbAddRating.Clear();
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ThucDonData.Instance.SaveData();
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn ảnh món ăn",
                Filter = "Ảnh (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                cbAddImage.Text = openFileDialog.FileName;
            }
        }

        #endregion

        #region === SỬA MÓN ===

        private void cbSua_LoaiMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbSua_TenMonAn.Items.Clear();
            ObservableCollection<ThucDon>? selectedList = GetSelectedCategoryList(cbSua_LoaiMonAn);

            if (selectedList != null)
            {
                foreach (var item in selectedList)
                {
                    cbSua_TenMonAn.Items.Add(item.Name);
                }
            }
        }

        private void cbSua_TenMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedList = GetSelectedCategoryList(cbSua_LoaiMonAn);
            if (cbSua_TenMonAn.SelectedItem == null || selectedList == null) return;

            var selectedName = cbSua_TenMonAn.SelectedItem.ToString();
            var selectedDish = selectedList.FirstOrDefault(m => m.Name == selectedName);

            if (selectedDish != null)
            {
                txtSua_OldImage.Text = selectedDish.ImagePath;
                txt_OldRating.Text = selectedDish.Rating.ToString();
            }
        }

        private void btn_NewImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn ảnh món ăn",
                Filter = "Ảnh (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txt_NewImage.Text = openFileDialog.FileName;
            }
        }

        private void btn_Sua_Click(object sender, RoutedEventArgs e)
        {
            var danhSach = GetSelectedCategoryList(cbSua_LoaiMonAn);

            if (danhSach == null || cbSua_TenMonAn.SelectedItem == null) return;

            string selectedName = cbSua_TenMonAn.SelectedItem.ToString();
            var mon = danhSach.FirstOrDefault(m => m.Name == selectedName);

            if (mon != null)
            {
                if (!string.IsNullOrWhiteSpace(txtSua_NewName.Text))
                    mon.Name = txtSua_NewName.Text;

                if (!string.IsNullOrWhiteSpace(txt_NewImage.Text))
                    mon.ImagePath = txt_NewImage.Text;

                if (double.TryParse(txt_NewRating.Text, out double newRating))
                    mon.Rating = newRating;

                MessageBox.Show("Cập nhật món ăn thành công!");
            }
        }

        #endregion

        #region === XÓA MÓN ===

        private void cbXoa_LoaiMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var danhSach = GetSelectedCategoryList(cbXoa_LoaiMonAn);

            if (danhSach != null)
            {
                cbXoa_TenMonAn.ItemsSource = danhSach.Select(m => m.Name).ToList();
            }
        }

        private void cbXoa_TenMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedList = GetSelectedCategoryList(cbXoa_LoaiMonAn);
            if (cbXoa_TenMonAn.SelectedItem == null || selectedList == null) return;
        }

        private void btn_Xoa_Click(object sender, RoutedEventArgs e)
        {
            var selectedCategoryItem = cbXoa_LoaiMonAn.SelectedItem as ComboBoxItem;
            if (selectedCategoryItem == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục món ăn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selectedCategory = selectedCategoryItem.Content.ToString();
            var danhSach = GetSelectedCategoryList(cbXoa_LoaiMonAn);
            if (danhSach == null)
            {
                MessageBox.Show("Không tìm thấy danh sách món ăn!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string? selectedDish = cbXoa_TenMonAn.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedDish))
            {
                MessageBox.Show("Vui lòng chọn món ăn để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var monCanXoa = danhSach.FirstOrDefault(m => m.Name == selectedDish);
            if (monCanXoa != null)
            {
                danhSach.Remove(monCanXoa);
                MessageBox.Show($"Đã xóa món \"{selectedDish}\" khỏi danh mục \"{selectedCategory}\".", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                cbXoa_TenMonAn.ItemsSource = danhSach.Select(m => m.Name);
            }
            else
            {
                MessageBox.Show("Không tìm thấy món ăn để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region === HÀM PHỤ ===

        private ObservableCollection<ThucDon>? GetSelectedCategoryList(ComboBox comboBox)
        {
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return null;

            string category = selectedItem.Content.ToString();

            switch (category)
            {
                case "Món chính": return ThucDonData.Instance.ListMonChinh;
                case "Món tráng miệng": return ThucDonData.Instance.ListMonTrangMieng;
                case "Trà trái cây": return ThucDonData.Instance.ListTraTraiCay;
                case "Nước ép": return ThucDonData.Instance.ListNuocEp;
                case "Món khuyến mãi": return ThucDonData.Instance.ListMonKhuyenMai;
                case "Món mới": return ThucDonData.Instance.ListMonMoi;
                case "Món bán chạy": return ThucDonData.Instance.ListMonBanChay;
                default: return null;
            }
        }
        #endregion

    }
}