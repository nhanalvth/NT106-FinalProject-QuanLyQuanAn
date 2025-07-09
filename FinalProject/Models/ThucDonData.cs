#nullable enable
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Npgsql;


namespace FinalProject.Models;

public class ThucDon
{
    public int ItemID { get; set; }                // itemid
    public string ItemName { get; set; } = "";   // itemname
    public string DanhMuc { get; set; } = "";  // category
    public decimal Gia { get; set; }           // price
    public string? ImagePath { get; set; }     // Nếu có hình ảnh
    public double Rating { get; set; }         // Nếu cần cho UI
}


public class ThucDonData
{
    private static ThucDonData? _instance;
    public static ThucDonData Instance => _instance ??= new ThucDonData();

    public ObservableCollection<ThucDon> ListMonChinh { get; private set; } = new();
    public ObservableCollection<ThucDon> ListMonTrangMieng { get; private set; } = new();
    public ObservableCollection<ThucDon> ListTraTraiCay { get; private set; } = new();
    public ObservableCollection<ThucDon> ListNuocEp { get; private set; } = new();
    public ObservableCollection<ThucDon> ListMonKhuyenMai { get; private set; } = new();
    public ObservableCollection<ThucDon> ListMonBanChay { get; private set; } = new();
    public ObservableCollection<ThucDon> ListMonMoi { get; private set; } = new();

    private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

    public ThucDonData()
    {
        LoadDataFromDatabase();
    }

    public void LoadDataFromDatabase()
    {
        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT itemid, itemname, category, price, imageurl FROM menuitems";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var item = new ThucDon
                {
                    ItemID = reader.GetInt32(0),
                    ItemName = reader.GetString(1),
                    DanhMuc = reader.GetString(2),
                    Gia = reader.GetDecimal(3),
                    ImagePath = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Rating = 5         // Có thể random/để mặc định
                };

                var danhMucs = item.DanhMuc.Split(',', (char)StringSplitOptions.RemoveEmptyEntries);

                foreach (var dm in danhMucs)
                {
                    switch (dm.Trim())
                    {
                        case "MonChinh":
                            ListMonChinh.Add(item); break;
                        case "MonMoi":
                            ListMonMoi.Add(item); break;
                        case "TrangMieng":
                            ListMonTrangMieng.Add(item); break;
                        case "TraTraiCay":
                            ListTraTraiCay.Add(item); break;
                        case "NuocEp":
                            ListNuocEp.Add(item); break;
                        case "MonKhuyenMai":
                            ListMonKhuyenMai.Add(item); break;
                        case "MonBanChay":
                            ListMonBanChay.Add(item); break;
                    }
                }
            }
        }

        catch (Exception ex)
        {
            // Ghi log hoặc hiện popup lỗi
            Console.WriteLine("Lỗi kết nối DB: " + ex.Message);
        }
    }

}



