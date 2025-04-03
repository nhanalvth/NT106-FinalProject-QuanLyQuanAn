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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FinalProject
{
    public partial class Dashboard : Page
    {
        public ObservableCollection<ThucDon> ListMonBanChay => ThucDonData.Instance.ListMonBanChay;
        public ObservableCollection<ThucDon> ListMonMoi => ThucDonData.Instance.ListMonMoi;

        // Danh sách chứa ảnh hiện tại
        private ObservableCollection<string> currentImage = new ObservableCollection<string>();
        private List<string> imagePaths = new List<string> { 
            "/Images/banner1.jpg",
            "/Images/banner2.jpg",
            "/Images/banner3.jpg",
            "/Images/banner4.jpg"
        };

        private int currentIndex = 0;
        private DispatcherTimer timer;
        public Dashboard()
        {
            InitializeComponent();
            this.DataContext = this;//nạp dữ liệu 

            currentImage.Add(imagePaths[currentIndex]); // Thêm ảnh đầu tiên vào danh sách hiển thị
            BannerSlider.ItemsSource = currentImage;   // Gán Binding

            // Tạo timer để tự động chuyển ảnh
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) => NextImage(null, null);
            timer.Start();

        }

        private void NextImage(object sender, RoutedEventArgs e)
        {
            currentIndex = (currentIndex + 1) % imagePaths.Count;
            UpdateImage();
        }
        private void PrevImage(object sender, RoutedEventArgs e)
        {
            currentIndex = (currentIndex - 1 + imagePaths.Count) % imagePaths.Count;
            UpdateImage();
        }
        // Cập nhật ảnh hiện tại
        private void UpdateImage()
        {
            currentImage[0] = imagePaths[currentIndex];
        }


    }
}
