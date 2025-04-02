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

namespace FinalProject
{
    public class ThucDon
    {
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public double Rating { get; set; }
    }
    /// <summary>
    /// Interaction logic for QlyThucDon.xaml
    /// </summary>
    public partial class QlyThucDon : Page
    {
        public ObservableCollection<ThucDon> ListMonChinh { get; set; }
        public ObservableCollection<ThucDon> ListMonTrangMieng { get; set; }
        public ObservableCollection<ThucDon> ListTraTraiCay { get; set; }
        public ObservableCollection<ThucDon> ListNuocEp { get; set; }
        public ObservableCollection<ThucDon> ListMonKhuyenMai { get; set; }
        public QlyThucDon()
        {
            InitializeComponent();

            //Danh sách Binding món chính 
            ListMonChinh = new ObservableCollection<ThucDon>()
            {
                new ThucDon() { Name="Phở Bò - 40.000d", ImagePath="Images/ThucDon/MonChinh/Pho.jpg", Rating=5},
                new ThucDon() { Name="Gà Kho - 35.000d", ImagePath="Images/ThucDon/MonChinh/GaKho.jpg", Rating=5},
                new ThucDon() { Name="Cơm Gà - 40.000d", ImagePath="Images/ThucDon/MonChinh/ComGa.jpg", Rating=4},
                new ThucDon() { Name="Hamberger - 35.000d", ImagePath="Images/ThucDon/MonChinh/Hamberger.jpg", Rating=4},
                new ThucDon() { Name="Hủ Tiếu - 35.000d", ImagePath="Images/ThucDon/MonChinh/HuTieu.jpg", Rating=5}
            };

            ListMonTrangMieng = new ObservableCollection<ThucDon>()
            {
                new ThucDon() { Name="Bánh Plan - 20.000d", ImagePath="Images/ThucDon/TrangMieng/BanhPlan.jpg", Rating=5},
                new ThucDon() { Name="Chè Bưởi - 20.000d", ImagePath="Images/ThucDon/TrangMieng/CheBuoi.jpg", Rating=5},
                new ThucDon() { Name="Chè Đạu Đen - 20.000d", ImagePath="Images/ThucDon/TrangMieng/CheDauDen.jpg", Rating=4},
                new ThucDon() { Name="Chè Khúc Bạch - 20.000d", ImagePath="Images/ThucDon/TrangMieng/CheKhucbach.jpg", Rating=4},
                new ThucDon() { Name="Chuối - 20.000d", ImagePath="Images/ThucDon/TrangMieng/Chuoi.jpg", Rating=5},          
                new ThucDon() { Name="Dưa Hấu - 20.000d", ImagePath="Images/ThucDon/TrangMieng/DuaHau.jpg", Rating=5},
                new ThucDon() { Name="Nho - 20.000d", ImagePath="Images/ThucDon/TrangMieng/Nho.jpg", Rating=4},
                new ThucDon() { Name="Xôi Đậu Xanh - 20.000d", ImagePath="Images/ThucDon/TrangMieng/XoiDauXanh.jpg", Rating=4},
                new ThucDon() { Name="Xôi Gấc - 20.000d", ImagePath="Images/ThucDon/TrangMieng/XoiGac.jpg", Rating=5}
            };

            ListTraTraiCay = new ObservableCollection<ThucDon>()
            {
                new ThucDon() { Name="Trà Đào - 20.000d", ImagePath="Images/ThucDon/TraTraiCay/TraDao.jpg", Rating=5},
                new ThucDon() { Name="Trà Dâu - 20.000d", ImagePath="Images/ThucDon/TraTraiCay/TraDau.jpg", Rating=5},
                new ThucDon() { Name="Trà Ổi - 20.000d", ImagePath="Images/ThucDon/TraTraiCay/TraOi.jpg", Rating=4},
                new ThucDon() { Name="Trà Vãi - 20.000d", ImagePath="Images/ThucDon/TraTraiCay/TraBuoi.jpg", Rating=4},
              
            };

            ListNuocEp = new ObservableCollection<ThucDon>()
            {
                new ThucDon() { Name="Nước Ép Cam - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpCam.jpg", Rating=5},
                new ThucDon() { Name="Nước Ép Dưa Hấu - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpDuaHau.jpg", Rating=5},
                new ThucDon() { Name="Nước Ép Dưa Lưới - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpDuaLuoi.jpg", Rating=5},
                new ThucDon() { Name="Nước Ép Táo - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpTao.jpg", Rating=4},
                new ThucDon() { Name="Nước Ép Thơm - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpThom.jpg", Rating=4},

            };

            ListMonKhuyenMai = new ObservableCollection<ThucDon>()
            {
                new ThucDon() { Name="Nước Ép Cam - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpCam.jpg", Rating=5},
                new ThucDon() { Name="Nước Ép Dưa Lưới - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpDuaLuoi.jpg", Rating=5},
                new ThucDon() { Name="Nước Ép Táo - 20.000d", ImagePath="Images/ThucDon/NuocEp/NuocEpTao.jpg", Rating=4},
                new ThucDon() { Name="Cơm Gà - 40.000d", ImagePath="Images/ThucDon/MonChinh/ComGa.jpg", Rating=4},
                new ThucDon() { Name="Bánh Plan - 20.000d", ImagePath="Images/ThucDon/TrangMieng/BanhPlan.jpg", Rating=5},

            };
            this.DataContext = this;
        }
    }
}
