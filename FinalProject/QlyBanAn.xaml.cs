using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Npgsql;

namespace FinalProject
{
    public partial class QlyBanAn : Page
    {
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        public QlyBanAn()
        {
            InitializeComponent();
            Loaded += QlyBanAn_Loaded;
        }

        private void QlyBanAn_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBanAn();
        }

        private void LoadBanAn()
        {
            try
            {
                var danhSachBan = new List<(int tableId, string tableNumber, string status)>();

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    // Bước 1: Load tất cả bàn
                    string queryBan = "SELECT tableid, tablenumber FROM tables";
                    var cmdBan = new NpgsqlCommand(queryBan, conn);
                    var readerBan = cmdBan.ExecuteReader();

                    var trangThaiTam = new Dictionary<string, string>();
                    var tableIdMap = new Dictionary<string, int>();

                    while (readerBan.Read())
                    {
                        int id = readerBan.GetInt32(0);
                        string soBan = readerBan.GetString(1);
                        trangThaiTam[soBan] = "Trong"; // mặc định
                        tableIdMap[soBan] = id;
                    }
                    readerBan.Close();

                    // Bước 2: cập nhật trạng thái
                    string queryOrder = @"
                        SELECT t.tablenumber
                        FROM orders o
                        JOIN tables t ON o.tableid = t.tableid
                        WHERE o.paid = false";

                    var cmdOrder = new NpgsqlCommand(queryOrder, conn);
                    var readerOrder = cmdOrder.ExecuteReader();

                    while (readerOrder.Read())
                    {
                        string soBan = readerOrder.GetString(0);
                        trangThaiTam[soBan] = "DangPhucVu";
                    }
                    readerOrder.Close();

                    // Ghép lại danh sách với tableid
                    foreach (var kvp in trangThaiTam)
                    {
                        danhSachBan.Add((tableIdMap[kvp.Key], kvp.Key, kvp.Value));
                    }
                }


                WrapPanelBanAn.Children.Clear();
                foreach (var ban in danhSachBan.OrderBy(b => b.tableId))
                {
                    var btn = new Button
                    {
                        Content = $"{ban.tableNumber}",
                        Tag = ban.status,
                        Width = 160,
                        Height = 130,
                        Margin = new Thickness(10),
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        Padding = new Thickness(10),
                        Background = ban.status == "DangPhucVu" ? Brushes.Orange : Brushes.LightGreen
                    };
                    btn.Click += Ban_Click;
                    WrapPanelBanAn.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải bàn ăn: " + ex.Message);
            }
        }

        private void Ban_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string banSo = btn.Content.ToString();
                string trangThai = btn.Tag.ToString();

                string hienThi = trangThai == "DangPhucVu" ? "Đang phục vụ" : "Trống";
                MessageBox.Show($"{banSo} - Trạng thái: {hienThi}", "Thông tin bàn");
            }
        }
    }
}
