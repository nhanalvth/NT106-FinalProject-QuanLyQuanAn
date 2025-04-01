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
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        private ObservableCollection<string> currentImage = new ObservableCollection<string>(); // Danh sách chứa ảnh hiện tại
        private List<string> imagePaths = new List<string> // Danh sách ảnh
        {
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
            InitializeComponent();
            currentImage.Add(imagePaths[currentIndex]); // Thêm ảnh đầu tiên vào danh sách hiển thị
            BannerSlider.ItemsSource = currentImage;   // Gán Binding

            // Tạo timer để tự động chuyển ảnh
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
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

        private void UpdateImage()
        {
            currentImage[0] = imagePaths[currentIndex]; // Cập nhật ảnh hiện tại
        }


    }
}
