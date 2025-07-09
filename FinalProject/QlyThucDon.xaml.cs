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
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using FinalProject.Models;
using Newtonsoft.Json;
using System.Data;
using Npgsql;

namespace FinalProject
{
    public partial class QlyThucDon : Page
    {
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        public ObservableCollection<ThucDon> ListMonChinh { get; set; } = new ObservableCollection<ThucDon>();
        public ObservableCollection<ThucDon> ListMonTrangMieng { get; set; } = new ObservableCollection<ThucDon>();
        public ObservableCollection<ThucDon> ListTraTraiCay { get; set; } = new ObservableCollection<ThucDon>();
        public ObservableCollection<ThucDon> ListNuocEp { get; set; } = new ObservableCollection<ThucDon>();
        public ObservableCollection<ThucDon> ListMonKhuyenMai { get; set; } = new ObservableCollection<ThucDon>(); // nếu có
        public ObservableCollection<ThucDon> ListMonBanChay { get; set; } = new ObservableCollection<ThucDon>(); // nếu có
        public ObservableCollection<ChiTietDonHang> DanhSachMonTamThoi { get; set; } = new ObservableCollection<ChiTietDonHang>();


        public QlyThucDon()
        {
            InitializeComponent();
            LoadDataFromDatabase();
            this.DataContext = this;
            DataContext = FinalProject.Models.ThucDonData.Instance;
        }

        private void LoadDataFromDatabase()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT itemid, itemname, category, price FROM menuitems";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new ThucDon
                        {
                            ItemID = reader.GetInt32(0),
                            ItemName = reader.GetString(1),
                            DanhMuc = reader.GetString(2),
                            Gia = reader.GetDecimal(3)
                        };

                        switch (item.DanhMuc)
                        {
                            case "MonChinh":
                                ListMonChinh.Add(item); break;
                            case "TrangMieng":
                                ListMonTrangMieng.Add(item); break;
                            case "TraTraiCay":
                                ListTraTraiCay.Add(item); break;
                            case "NuocEp":
                                ListNuocEp.Add(item); break;
                            case "KhuyenMai":
                                ListMonKhuyenMai.Add(item); break;
                        }
                    }
                }
            }
        }

        // Scroll methods giữ nguyên
        private void ScrollToTarget(Border target)
        {
            if (DsMonAnScrollViewer != null && target != null)
            {
                double targetOffset = target.TranslatePoint(new Point(0, 0), DsMonAnScrollViewer).Y;
                DsMonAnScrollViewer.ScrollToVerticalOffset(targetOffset);
            }
        }

        private void btn_MonChinh_Click(object sender, RoutedEventArgs e) => ScrollToTarget(DsMonChinh);
        private void btn_TrangMieng_Click(object sender, RoutedEventArgs e) => ScrollToTarget(DsTrangMieng);
        private void btn_TraTraiCay_Click(object sender, RoutedEventArgs e) => ScrollToTarget(DsTraTraiCay);
        private void btn_NuocEp_Click(object sender, RoutedEventArgs e) => ScrollToTarget(DsNuocEp);
        private void btn_MonKhuyenMai_Click(object sender, RoutedEventArgs e) => ScrollToTarget(DsMonKhuyenMai);
        private void btn_ThemXoaMonAn_Click(object sender, RoutedEventArgs e)
        {
            var themXoaMonAnWindow = new ThemXoaMonAn(ListMonChinh); // ✅ truyền danh sách mới
            themXoaMonAnWindow.ShowDialog();
        }
        private List<MonAn> danhSachMonChon = new List<MonAn>();
        public static Action<List<MonAn>> OnMonAnDuocChon;

        private void Mon_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ThucDon mon)
            {
                var monChon = new MonAn
                {
                    ItemID = mon.ItemID,
                    ItemName = mon.ItemName,
                    Gia = mon.Gia,
                    SoLuong = 1,
                    GhiChu = ""
                };

                if (!danhSachMonChon.Any(m => m.ItemID == monChon.ItemID))
                {
                    danhSachMonChon.Add(monChon);
                    MessageBox.Show($"Đã chọn: {monChon.ItemName}");
                }
                else
                {
                    MessageBox.Show($"{monChon.ItemName} đã được chọn trước đó.");
                }
            }
        }

        private void btnXacNhanMon_Click(object sender, RoutedEventArgs e)
        {
            if (danhSachMonChon.Count == 0)
            {
                MessageBox.Show("Bạn chưa chọn món nào.");
                return;
            }

            OnMonAnDuocChon?.Invoke(danhSachMonChon);
            NavigationService.GoBack();
        }

    }
}
