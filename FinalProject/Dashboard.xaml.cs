#nullable enable
using FinalProject.Models;
using MaterialDesignThemes.Wpf;
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
        //fgdfgffg 
        private int currentIndex = 0;
        private DispatcherTimer timer;
        public Dashboard()
        {
            InitializeComponent();
            txtUsername.Text = UserSession.UserName;
            this.DataContext = this;//nạp dữ liệu 

            currentImage.Add(imagePaths[currentIndex]); // Thêm ảnh đầu tiên vào danh sách hiển thị
            BannerSlider.ItemsSource = currentImage;   // Gán Binding

            // Tạo timer để tự động chuyển ảnh
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) => NextImage(null, null);
            timer.Start();

        }

        private void NextImage(object? sender, RoutedEventArgs? e)
        {
            currentIndex = (currentIndex + 1) % imagePaths.Count;
            UpdateImage();
        }
        private void PrevImage(object? sender, RoutedEventArgs? e)
        {
            currentIndex = (currentIndex - 1 + imagePaths.Count) % imagePaths.Count;
            UpdateImage();
        }
        // Cập nhật ảnh hiện tại
        private void UpdateImage()
        {
            currentImage[0] = imagePaths[currentIndex];
        }

        //Tạo Trang tin tức
        private void LoadNews_Click(object sender, RoutedEventArgs e)
        {
            string url = txtUrl.Text.Trim();

            if (!url.StartsWith("http"))
                url = "https://" + url;

            try
            {
                NewsBrowser.Navigate(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải trang: " + ex.Message);
            }
        }

        //Focus đến từng danh sách
        private void ScrollToTarget(Border target)
        {
            if (DsMonAnScrollViewer != null && target != null)
            {
                double targetOffset = target.TranslatePoint(new Point(0, 0), DsMonAnScrollViewer).Y;
                DsMonAnScrollViewer.ScrollToVerticalOffset(targetOffset);
            }
        }
        private void btn_MonBanChay_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsMonBanChay);
        }
        private void btn_TinTuc_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsTinTuc);
        }
        private void btn_MonMoi_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsMonMoi);
        }
        private void btn_TimeSale_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsTimeSale);
        }
        private void btn_KhuyenMai_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsKhuyenMai);
        }
        //button thanh toán
        private void btn_MoTrangThanhToan_Click(object sender, RoutedEventArgs e)
        {
            var form = new ThanhToan("DH123", 185000); // truyền mã đơn và tổng tiền
            form.ShowDialog();
        }
    }
}
