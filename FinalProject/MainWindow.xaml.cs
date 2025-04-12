#nullable enable
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
    public partial class MainWindow : Window
    {
        private QlyBanAn qlyBanAnPage;
        private QlyDonHang qlyDonHangPage;
        private QlyThucDon qlyThucDonPage;
        private QlyNhanVien qlyNhanVienPage;
        private Dashboard DashboardPage;
        private ThongKe ThongKePage;
        private CaiDat CaiDatPage;
        public MainWindow()
        {
            InitializeComponent();
            qlyBanAnPage = new QlyBanAn();
            qlyDonHangPage = new QlyDonHang();
            qlyNhanVienPage = new QlyNhanVien();
            qlyThucDonPage = new QlyThucDon();
            DashboardPage = new Dashboard();
            ThongKePage = new ThongKe();
            CaiDatPage = new CaiDat();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double scaleX = ActualWidth / 800.0;  // 800 là kích thước thiết kế gốc
            double scaleY = ActualHeight / 600.0; // 600 là chiều cao thiết kế gốc
            double scale = Math.Min(scaleX, scaleY); // Đảm bảo tỷ lệ đồng nhất

            MainScaleTransform.ScaleX = Math.Max(0.5, 1.0);
            MainScaleTransform.ScaleY = Math.Max(0.5, 1.0);
        }
        private void Btn_QlyBanAn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(qlyBanAnPage);
        }
        private void Btn_QlyDonHang_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(qlyDonHangPage);
        }
        private void Btn_QlyNhanVien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(qlyNhanVienPage);
        }
        private void Btn_QlyThucDon_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(qlyThucDonPage);
        }
        private void Btn_Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(DashboardPage);
        }
        private void Btn_ThongKe_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(ThongKePage);
        }
        private void Btn_CaiDat_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(CaiDatPage);
        }
        // Mở Popup khi nhấn vào Button
        private void Btn_Chat_Click(object sender, RoutedEventArgs e)
        {
            chatPopup.IsOpen = !chatPopup.IsOpen; // Toggle Popup
        }

        // Gửi tin nhắn khi nhấn "Send"
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ChatInputTextBox.Text))
            {
                // Thêm tin nhắn vào ListBox
                ChatListBox.Items.Add("You: " + ChatInputTextBox.Text);
                ChatInputTextBox.Clear();
            }
        }

    }
}

