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
    /// <summary>
    /// Interaction logic for ThemXoaMonAn.xaml
    /// </summary>
    public partial class ThemXoaMonAn : Window
    {
        public ObservableCollection<ThucDon> ThucDons { get; set; }
        public ThemXoaMonAn(ObservableCollection<ThucDon> listMonChinh)
        {
            InitializeComponent();        
        }

        private void btn_Them_Click(object sender, RoutedEventArgs e)
        {
            if (cbCategory.SelectedItem is ComboBoxItem selectedItem &&
            !string.IsNullOrWhiteSpace(cbAddName.Text) &&
            !string.IsNullOrWhiteSpace(cbAddImage.Text) &&
            double.TryParse(cbAddRating.Text, out double rating))
            {
                // Tạo món ăn mới
                ThucDon monMoi = new ThucDon
                {
                    Name = cbAddName.Text,
                    ImagePath = cbAddImage.Text,
                    Rating = rating
                };

                // Thêm vào danh sách phù hợp
                switch (selectedItem.Content.ToString())
                {
                    case "Món chính":
                        ThucDonData.Instance.ListMonChinh.Add(monMoi);
                        break;
                    case "Món tráng miệng":
                        ThucDonData.Instance.ListMonTrangMieng.Add(monMoi);
                        break;
                    case "Trà trái cây":
                        ThucDonData.Instance.ListTraTraiCay.Add(monMoi);
                        break;
                    case "Nước ép":
                        ThucDonData.Instance.ListNuocEp.Add(monMoi);
                        break;
                    case "Món khuyến mãi":
                        ThucDonData.Instance.ListMonKhuyenMai.Add(monMoi);
                        break;
                    case "Món mới":
                        ThucDonData.Instance.ListMonMoi.Add(monMoi);
                        break;
                    case "Món bán chạy":
                        ThucDonData.Instance.ListMonBanChay.Add(monMoi);
                        break;
                    default:
                        MessageBox.Show("Danh mục không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }
                // Reset input
                cbAddName.Clear();
                cbAddImage.Clear();
                cbAddRating.Clear();
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin hợp lệ!",
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void cbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Kiểm tra xem người dùng đã chọn danh mục chưa
            if (cbCategory.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedCategory = selectedItem.Content.ToString();
                MessageBox.Show($"Bạn đã chọn danh mục: {selectedCategory}",
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
                cbAddImage.Text = openFileDialog.FileName; // Lưu đường dẫn ảnh vào TextBox
            }
        }

        

    }
}

