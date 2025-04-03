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
            ThucDons = listMonChinh;

            cbOldName.ItemsSource = ThucDons.Select(m => m.Name);
            cbOldImage.ItemsSource = ThucDons.Select(m => m.ImagePath);
            cbOldRating.ItemsSource = ThucDons.Select(m => m.Rating.ToString());

            cbDeleteName.ItemsSource = ThucDons.Select(m => m.Name);
        }

        private void btn_Them_Click(object sender, RoutedEventArgs e)
        {
            string name = cbAddName.Text;
            string image = cbAddImage.Text;
            double rating;
            if (double.TryParse(cbAddRating.Text, out rating))
            {
                ThucDons.Add(new ThucDon { Name = name, ImagePath = image, Rating = rating });
                MessageBox.Show("Món ăn đã được thêm!");
            }
            else
            {
                MessageBox.Show("Rating không hợp lệ!");
            }
        }
        private void btn_Sua_Click(object sender, RoutedEventArgs e)
        {
            var oldName = cbOldName.SelectedItem as string;
            var oldItem = ThucDons.FirstOrDefault(m => m.Name == oldName);
            if (oldItem != null)
            {
                oldItem.Name = cbNewName.Text;
                oldItem.ImagePath = cbNewImage.Text;
                double rating;
                if (double.TryParse(cbNewRating.Text, out rating))
                {
                    oldItem.Rating = rating;
                    MessageBox.Show("Món ăn đã được cập nhật!");
                }
                else
                {
                    MessageBox.Show("Rating không hợp lệ!");
                }
            }
            else
            {
                MessageBox.Show("Không tìm thấy món ăn để sửa!");
            }
        }
        private void btn_Xoa_Click(object sender, RoutedEventArgs e)
        {
            var name = cbDeleteName.SelectedItem as string;
            var itemToRemove = ThucDons.FirstOrDefault(m => m.Name == name);
            if (itemToRemove != null)
            {
                ThucDons.Remove(itemToRemove);
                MessageBox.Show("Món ăn đã được xóa!");
            }
            else
            {
                MessageBox.Show("Không tìm thấy món để xóa!");
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Tất cả thay đổi đã được lưu!");
            Close();
        }
    }
}

