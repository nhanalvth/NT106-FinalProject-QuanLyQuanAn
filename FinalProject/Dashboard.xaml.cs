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
        public ObservableCollection<ThucDon> ListMonBanChay { get; set; }

        // Danh sách chứa ảnh hiện tại
        private ObservableCollection<string> currentImage = new ObservableCollection<string>();
        // Danh sách ảnh
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
            currentImage.Add(imagePaths[currentIndex]); // Thêm ảnh đầu tiên vào danh sách hiển thị
            BannerSlider.ItemsSource = currentImage;   // Gán Binding

            // Tạo timer để tự động chuyển ảnh
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) => NextImage(null, null);
            timer.Start();

            //Danh sách Binding Món bán chạy
            ListMonBanChay = new ObservableCollection<ThucDon>()
            {
                new ThucDon() { Name="Phở Bò - 40.000d", ImagePath="Images/ThucDon/MonChinh/Pho.jpg", Rating=5},
                new ThucDon() { Name="Gà Kho - 35.000d", ImagePath="Images/ThucDon/MonChinh/GaKho.jpg", Rating=5},
                new ThucDon() { Name="Nước Ép Cam - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpCam.jpg", Rating=5},
                new ThucDon() { Name="Chuối - 20.000d", ImagePath="Images/ThucDon/TrangMieng/Chuoi.jpg", Rating=5},
                new ThucDon() { Name="Nước Ép Táo - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpTao.jpg", Rating=4},
                new ThucDon() { Name="Cơm Gà - 40.000d", ImagePath="Images/ThucDon/MonChinh/ComGa.jpg", Rating=4},
                new ThucDon() { Name="Bánh Plan - 20.000d", ImagePath="Images/ThucDon/TrangMieng/BanhPlan.jpg", Rating=5},
            };
            this.DataContext = this;
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
