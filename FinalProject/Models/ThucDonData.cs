#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;

namespace FinalProject.Models;
public class ThucDon
{
    public string? Name { get; set; }
    public string? ImagePath { get; set; }
    public double Rating { get; set; }
}

public class ThucDonData
{
    private static ThucDonData? _instance;
    public static ThucDonData Instance => _instance ??= new ThucDonData();

    public ObservableCollection<ThucDon> ListMonChinh { get; set; } = new();
    public ObservableCollection<ThucDon> ListMonTrangMieng { get; set; } = new();
    public ObservableCollection<ThucDon> ListTraTraiCay { get; set; } = new();
    public ObservableCollection<ThucDon> ListNuocEp { get; set; } = new();
    public ObservableCollection<ThucDon> ListMonKhuyenMai { get; set; } = new();
    public ObservableCollection<ThucDon> ListMonMoi { get; set; } = new();
    public ObservableCollection<ThucDon> ListMonBanChay { get; set; } = new();

    public static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models", "data.json");

    public ThucDonData()
    {
        LoadData();

        // Nếu chưa có dữ liệu thì tạo dữ liệu mẫu
        if (ListMonChinh.Count == 0)
        {
            KhoiTaoMonAn();
        }
    }

    public void SaveData()
    {
        var data = new ThucDonDataWrapper
        {
            ListMonChinh = new(ListMonChinh),
            ListMonTrangMieng = new(ListMonTrangMieng),
            ListTraTraiCay = new(ListTraTraiCay),
            ListNuocEp = new(ListNuocEp),
            ListMonKhuyenMai = new(ListMonKhuyenMai),
            ListMonMoi = new(ListMonMoi),
            ListMonBanChay = new(ListMonBanChay)
        };

        var dir = Path.GetDirectoryName(FilePath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(FilePath, JsonConvert.SerializeObject(data, Formatting.Indented));
    }

    public void LoadData()
    {
        if (!File.Exists(FilePath)) return;

        var json = File.ReadAllText(FilePath);
        var data = JsonConvert.DeserializeObject<ThucDonDataWrapper>(json);
        if (data == null) return;

        ListMonChinh = new(data.ListMonChinh ?? []);
        ListMonTrangMieng = new(data.ListMonTrangMieng ?? []);
        ListTraTraiCay = new(data.ListTraTraiCay ?? []);
        ListNuocEp = new(data.ListNuocEp ?? []);
        ListMonKhuyenMai = new(data.ListMonKhuyenMai ?? []);
        ListMonMoi = new(data.ListMonMoi ?? []);
        ListMonBanChay = new(data.ListMonBanChay ?? []);
    }

    public void KhoiTaoMonAn()
    {
        var phoBo = new ThucDon() { Name = "Phở Bò - 40.000d", ImagePath = "Images/ThucDon/MonChinh/Pho.jpg", Rating = 5 };
        var gaKho = new ThucDon() { Name = "Gà Kho - 35.000d", ImagePath = "Images/ThucDon/MonChinh/GaKho.jpg", Rating = 5 };
        var comGa = new ThucDon() { Name = "Cơm Gà - 40.000d", ImagePath = "Images/ThucDon/MonChinh/ComGa.jpg", Rating = 4 };
        var hamberger = new ThucDon() { Name = "Hamberger - 35.000d", ImagePath = "Images/ThucDon/MonChinh/Hamberger.jpg", Rating = 4 };
        var huTieu = new ThucDon() { Name = "Hủ Tiếu - 35.000d", ImagePath = "Images/ThucDon/MonChinh/HuTieu.jpg", Rating = 5 };

        var banhPlan = new ThucDon() { Name = "Bánh Plan - 20.000d", ImagePath = "Images/ThucDon/TrangMieng/BanhPlan.jpg", Rating = 5 };
        var cheBuoi = new ThucDon() { Name = "Chè Bưởi - 20.000d", ImagePath = "Images/ThucDon/TrangMieng/CheBuoi.jpg", Rating = 5 };
        var cheDauDen = new ThucDon() { Name = "Chè Đậu Đen - 20.000d", ImagePath = "Images/ThucDon/TrangMieng/CheDauDen.jpg", Rating = 4 };
        var cheKhacBach = new ThucDon() { Name = "Chè Khúc Bạch - 20.000d", ImagePath = "Images/ThucDon/TrangMieng/CheKhucbach.jpg", Rating = 4 };
        var chuoi = new ThucDon() { Name = "Chuối - 20.000d", ImagePath = "Images/ThucDon/TrangMieng/Chuoi.jpg", Rating = 5 };
        var duaHau = new ThucDon() { Name = "Dưa Hấu - 20.000d", ImagePath = "Images/ThucDon/TrangMieng/DuaHau.jpg", Rating = 5 };
        var nho = new ThucDon() { Name = "Nho - 20.000d", ImagePath = "Images/ThucDon/TrangMieng/Nho.jpg", Rating = 4 };
        var xoiDauXanh = new ThucDon() { Name = "Xôi Đậu Xanh - 20.000d", ImagePath = "Images/ThucDon/TrangMieng/XoiDauXanh.jpg", Rating = 4 };
        var xoiGac = new ThucDon() { Name = "Xôi Gấc - 20.000d", ImagePath = "Images/ThucDon/TrangMieng/XoiGac.jpg", Rating = 5 };

        var traDao = new ThucDon() { Name = "Trà Đào - 20.000d", ImagePath = "Images/ThucDon/TraTraiCay/TraDao.jpg", Rating = 5 };
        var traDau = new ThucDon() { Name = "Trà Dâu - 20.000d", ImagePath = "Images/ThucDon/TraTraiCay/TraDau.jpg", Rating = 5 };
        var traOi = new ThucDon() { Name = "Trà Ổi - 20.000d", ImagePath = "Images/ThucDon/TraTraiCay/TraOi.jpg", Rating = 4 };
        var traVai = new ThucDon() { Name = "Trà Vãi - 20.000d", ImagePath = "Images/ThucDon/TraTraiCay/TraVai.jpg", Rating = 4 };

        var nuocEpCam = new ThucDon() { Name = "Nước Ép Cam - 20.000d", ImagePath = "Images/ThucDon/NuocEp/NuocEpCam.jpg", Rating = 5 };
        var nuocEpDuaHau = new ThucDon() { Name = "Nước Ép Dưa Hấu - 20.000d", ImagePath = "Images/ThucDon/NuocEp/NuocEpDuaHau.jpg", Rating = 5 };
        var nuocEpDuaLuoi = new ThucDon() { Name = "Nước Ép Dưa Lưới - 20.000d", ImagePath = "Images/ThucDon/NuocEp/NuocEpDuaLuoi.jpg", Rating = 5 };
        var nuocEpTao = new ThucDon() { Name = "Nước Ép Táo - 20.000d", ImagePath = "Images/ThucDon/NuocEp/NuocEpTao.jpg", Rating = 4 };
        var nuocEpThom = new ThucDon() { Name = "Nước Ép Thơm - 20.000d", ImagePath = "Images/ThucDon/NuocEp/NuocEpThom.jpg", Rating = 4 };

        ListMonChinh = new ObservableCollection<ThucDon> { phoBo, gaKho, comGa, hamberger, huTieu };
        ListMonTrangMieng = new ObservableCollection<ThucDon> { banhPlan, cheBuoi, cheDauDen, cheKhacBach, chuoi, duaHau, nho, xoiDauXanh, xoiGac };
        ListTraTraiCay = new ObservableCollection<ThucDon> { traDao, traDau, traOi, traVai };
        ListNuocEp = new ObservableCollection<ThucDon> { nuocEpCam, nuocEpDuaHau, nuocEpDuaLuoi, nuocEpTao, nuocEpThom };
        ListMonKhuyenMai = new ObservableCollection<ThucDon> { banhPlan, comGa, nuocEpThom, duaHau };
        ListMonMoi = new ObservableCollection<ThucDon> { phoBo, traDao, banhPlan, comGa, nuocEpDuaLuoi, huTieu, nuocEpThom};
        ListMonBanChay = new ObservableCollection<ThucDon> { phoBo, gaKho, chuoi, comGa, banhPlan };
    }

    private class ThucDonDataWrapper
    {
        public List<ThucDon>? ListMonChinh { get; set; }
        public List<ThucDon>? ListMonTrangMieng { get; set; }
        public List<ThucDon>? ListTraTraiCay { get; set; }
        public List<ThucDon>? ListNuocEp { get; set; }
        public List<ThucDon>? ListMonKhuyenMai { get; set; }
        public List<ThucDon>? ListMonMoi { get; set; }
        public List<ThucDon>? ListMonBanChay { get; set; }
    }
}


