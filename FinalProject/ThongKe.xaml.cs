using FinalProject.Models;
using LiveCharts;
using LiveCharts.Wpf;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FinalProject
{
    public partial class ThongKe : Page
    {
        private readonly string connectionString =
            "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        public class MonBanChay
        {
            public string TenMon { get; set; }
            public int SoLuong { get; set; }
        }

        public class DoanhThuModel
        {
            public string Ngay { get; set; }
            public int TongDon { get; set; }
            public decimal DoanhThu { get; set; }
        }

        public ThongKe()
        {
            InitializeComponent();
            //txtUsername.Text = UserSession.UserName;
            Loaded += ThongKe_Loaded;
        }

        private void ThongKe_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMonBanChay();
            LoadThoiGianCaoDiem();
            LoadDoanhThuTheoPhuongThuc();
            LoadDoanhThu30Ngay();

        }

        private void btnXemBaoCao_Click(object sender, RoutedEventArgs e)
        {
            DateTime? tuNgay = datePickerFrom.SelectedDate;
            DateTime? denNgay = datePickerTo.SelectedDate;

            if (tuNgay == null || denNgay == null)
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian.");
                return;
            }

            LoadDanhSachBills(tuNgay.Value, denNgay.Value);
        }


        private void LoadDanhSachBills(DateTime from, DateTime to)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT billid, DATE(issuedate), user, paymentmethod, finalamount
                        FROM bills
                        WHERE issuedate BETWEEN @from AND @to";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@from", from);
                        cmd.Parameters.AddWithValue("@to", to);

                        using (var reader = cmd.ExecuteReader())
                        {
                            var list = new List<HoaDonModel>();
                            decimal tongTien = 0;

                            while (reader.Read())
                            {
                                var hd = new HoaDonModel
                                {
                                    BillID = reader.GetInt32(0),
                                    Ngay = reader.GetDateTime(1).ToString("dd/MM/yyyy"),
                                    NguoiThanhToan = reader.GetString(2),
                                    PhuongThuc = reader.GetString(3),
                                    TongTien = reader.GetDecimal(4)
                                };
                                tongTien += hd.TongTien;
                                list.Add(hd);
                            }

                            // Thêm dòng tổng cộng
                            list.Add(new HoaDonModel
                            {
                                BillID = 0,
                                Ngay = "",
                                NguoiThanhToan = "",
                                PhuongThuc = "TỔNG CỘNG",
                                TongTien = tongTien
                            });

                            listViewDoanhThu.ItemsSource = list;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách hóa đơn: " + ex.Message);
            }
        }



        private void LoadMonBanChay()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT m.itemname, SUM(d.quantity) AS totalsold
                        FROM orderdetails d
                        JOIN menuitems m ON d.itemid = m.itemid
                        GROUP BY m.itemname
                        ORDER BY totalsold DESC
                        LIMIT 5";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var topItems = new List<MonBanChay>();
                        while (reader.Read())
                        {
                            topItems.Add(new MonBanChay
                            {
                                TenMon = reader.GetString(0),
                                SoLuong = reader.GetInt32(1)
                            });
                        }

                        listViewMonBanChay.ItemsSource = topItems;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải món bán chạy: " + ex.Message);
            }
        }

        private void LoadThoiGianCaoDiem()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT EXTRACT(HOUR FROM issuedate) AS hour, COUNT(*) AS order_count
                FROM bills
                GROUP BY hour
                ORDER BY order_count DESC
                LIMIT 3";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        listBoxCaoDiem.Items.Clear();
                        while (reader.Read())
                        {
                            int hour = (int)reader.GetDouble(0);
                            listBoxCaoDiem.Items.Add($"{hour}:00 - {hour + 1}:00");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thời gian cao điểm: " + ex.Message);
            }
        }

        private void LoadDoanhThuTheoPhuongThuc()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                SELECT paymentmethod, SUM(finalamount) as tongtien
                FROM bills
                GROUP BY paymentmethod
                ORDER BY tongtien DESC";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var values = new ChartValues<double>();
                        var labels = new List<string>();

                        while (reader.Read())
                        {
                            string method = reader.IsDBNull(0) ? "Không xác định" : reader.GetString(0);
                            double amount = reader.IsDBNull(1) ? 0 : (double)reader.GetDecimal(1);

                            labels.Add(method);
                            values.Add(amount);
                        }

                        chartPhuongThuc.Series = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Doanh thu",
                                Values = values,
                                DataLabels = true,
                                LabelPoint = point => point.Y.ToString("C0"),
                                Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                            }
                        };

                        chartPhuongThuc.AxisX.Clear();
                        chartPhuongThuc.AxisX.Add(new Axis
                        {
                            Title = "Phương thức",
                            Labels = labels
                        });

                        chartPhuongThuc.AxisY.Clear();
                        chartPhuongThuc.AxisY.Add(new Axis
                        {
                            Title = "VNĐ",
                            LabelFormatter = value => value.ToString("C0")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thống kê phương thức: " + ex.Message);
            }
        }
        private void LoadDoanhThu30Ngay()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                    SELECT DATE(issuedate) AS ngay, SUM(finalamount) AS doanhthu
                    FROM bills
                    WHERE issuedate >= CURRENT_DATE - INTERVAL '30 days'
                    GROUP BY ngay
                    ORDER BY ngay";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        var values = new ChartValues<decimal>();
                        var labels = new List<string>();

                        while (reader.Read())
                        {
                            DateTime ngay = reader.GetDateTime(0);
                            decimal doanhThu = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);

                            labels.Add(ngay.ToString("dd/MM"));
                            values.Add(doanhThu);
                        }

                        cartesianChart.Series = new SeriesCollection
                        {
                            new ColumnSeries
                            {
                                Title = "Doanh thu",
                                Values = values,
                                DataLabels = true,
                                LabelPoint = point => point.Y.ToString("C0"),
                                Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                            }
                        };
                        cartesianChart.AxisX.Clear();
                        cartesianChart.AxisX.Add(new Axis
                        {
                            Title = "Ngày",
                            Labels = labels
                        });

                        cartesianChart.AxisY.Clear();
                        cartesianChart.AxisY.Add(new Axis
                        {
                            Title = "VNĐ",
                            LabelFormatter = value => value.ToString("C0")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải biểu đồ doanh thu 30 ngày: " + ex.Message);
            }
        }



    }
}