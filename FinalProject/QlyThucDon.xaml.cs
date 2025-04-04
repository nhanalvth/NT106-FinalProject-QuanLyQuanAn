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

namespace FinalProject
{
    public partial class QlyThucDon : Page
    {
        public ObservableCollection<ThucDon> ListMonChinh => ThucDonData.Instance.ListMonChinh;
        public ObservableCollection<ThucDon> ListMonTrangMieng => ThucDonData.Instance.ListMonTrangMieng;
        public ObservableCollection<ThucDon> ListTraTraiCay => ThucDonData.Instance.ListTraTraiCay;
        public ObservableCollection<ThucDon> ListNuocEp => ThucDonData.Instance.ListNuocEp;
        public ObservableCollection<ThucDon> ListMonKhuyenMai => ThucDonData.Instance.ListMonKhuyenMai;
        public QlyThucDon()
        {
            InitializeComponent();
            this.DataContext = this;//nap dữ liệu
            
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
        private void btn_MonChinh_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsMonChinh);
        }
        private void btn_TrangMieng_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsTrangMieng);
        }
        private void btn_TraTraiCay_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsTraTraiCay);
        }
        private void btn_NuocEp_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsNuocEp);
        }
        private void btn_MonKhuyenMai_Click(object sender, RoutedEventArgs e)
        {
            ScrollToTarget(DsMonKhuyenMai);
        }
        private void btn_ThemXoaMonAn_Click(object sender, RoutedEventArgs e)
        {
            var themXoaMonAnWindow = new ThemXoaMonAn(ThucDonData.Instance.ListMonChinh); // ✅ Truyền tham số hợp lệ
            themXoaMonAnWindow.ShowDialog();
        }
    }
}
