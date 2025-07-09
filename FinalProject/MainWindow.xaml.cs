#nullable enable
using FinalProject.Models;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Text.Json;

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
        private static ChatServer server;
        public ObservableCollection<ChatMessage> ChatMessages { get; set; } = new ObservableCollection<ChatMessage>();
        private string currentUserName = UserSession.UserName;
        private string currentUserRole = UserSession.Role;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            qlyBanAnPage = new QlyBanAn();
            qlyDonHangPage = new QlyDonHang();
            qlyNhanVienPage = new QlyNhanVien();
            qlyThucDonPage = new QlyThucDon();
            DashboardPage = new Dashboard();
            ThongKePage = new ThongKe();
            CaiDatPage = new CaiDat();
            Closed += Window_Closed;
            DataContext = this;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Dashboard());
            if (server == null)
            {
                server = new ChatServer();
                server.Start(); // Lắng nghe trên cổng 8888
            }

            ConnectToServer();
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
            string msgText = ChatInputTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(msgText))
            {
                var chatMsg = new ChatMessage
                {
                    UserName = currentUserName,
                    Role = currentUserRole,
                    Message = msgText
                };

                // Thêm vào UI
                ChatMessages.Add(chatMsg);

                // Gửi đi (chuyển sang JSON hoặc định dạng string bạn định sẵn)
                SendChatMessageToServer(chatMsg);

                ChatInputTextBox.Clear();
            }
        }


        private TcpClient client;
        private NetworkStream stream;
        private Thread receiveThread;

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 8888); // IP local
                stream = client.GetStream();

                receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến server: " + ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                byte[] buffer = new byte[4096];
                while (true)
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    if (bytes == 0) break;

                    string json = Encoding.UTF8.GetString(buffer, 0, bytes).Trim();
                    // Xử lý nhiều tin nhắn nếu xuống dòng
                    foreach (string line in json.Split('\n'))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var message = JsonSerializer.Deserialize<ChatMessage>(line);
                            Dispatcher.Invoke(() => ChatMessages.Add(message));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => MessageBox.Show("Lỗi nhận tin nhắn: " + ex.Message));
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (server != null)
            {
                server.Stop();
            }

            // Đóng kết nối của client nếu đang mở
            try
            {
                stream?.Close();
                client?.Close();
            }
            catch { }
        }
        private void SendChatMessageToServer(ChatMessage message)
        {
            string json = JsonSerializer.Serialize(message);
            byte[] buffer = Encoding.UTF8.GetBytes(json + "\n");
            stream.Write(buffer, 0, buffer.Length);
        }

    }
}

