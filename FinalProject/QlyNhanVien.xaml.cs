using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace FinalProject
{
    public partial class QlyNhanVien : Page
    {
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";

        // Danh sách nhân viên
        public ObservableCollection<NhanVien> DanhSachNhanVien { get; set; } = new ObservableCollection<NhanVien>();

        // Danh sách chấm công
        public ObservableCollection<ChamCong> DanhSachChamCong { get; set; } = new ObservableCollection<ChamCong>();

        public QlyNhanVien()
        {
            InitializeComponent();
            txtUsername.Text = "Admin";
            dgNhanVien.ItemsSource = DanhSachNhanVien;
            dgChamCong.ItemsSource = DanhSachChamCong;
            dpNgayChamCong.SelectedDate = DateTime.Today;

            LoadNhanVienFromDatabase();
        }

        // Load dữ liệu nhân viên từ database
        private void LoadNhanVienFromDatabase()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    var cmd = new NpgsqlCommand("SELECT * FROM nhanvien", conn);
                    var reader = cmd.ExecuteReader();

                    DanhSachNhanVien.Clear();
                    while (reader.Read())
                    {
                        DanhSachNhanVien.Add(new NhanVien
                        {
                            MaNV = reader["manv"].ToString(),
                            TenNV = reader["tennv"].ToString(),
                            VaiTro = reader["vaitro"].ToString(),
                            IsQuanLy = Convert.ToBoolean(reader["isquanly"]),
                            CaLamViec = reader["calamviec"].ToString()
                        });
                    }
                }
            }
            catch { /* Xử lý lỗi nếu cần */ }
        }

        // Thêm nhân viên mới
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            var nv = new NhanVien
            {
                MaNV = txtMaNV.Text,
                TenNV = txtTenNV.Text,
                VaiTro = ((ComboBoxItem)cbVaiTro.SelectedItem)?.Content.ToString(),
                IsQuanLy = chkQuanLy.IsChecked ?? false,
                CaLamViec = txtCaLam.Text
            };
            DanhSachNhanVien.Add(nv);
            ClearForm();
        }

        // Sửa thông tin nhân viên
        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            if (dgNhanVien.SelectedItem is NhanVien nv)
            {
                nv.TenNV = txtTenNV.Text;
                nv.VaiTro = ((ComboBoxItem)cbVaiTro.SelectedItem)?.Content.ToString();
                nv.IsQuanLy = chkQuanLy.IsChecked ?? false;
                nv.CaLamViec = txtCaLam.Text;
                dgNhanVien.Items.Refresh();
            }
        }

        // Xóa nhân viên
        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (dgNhanVien.SelectedItem is NhanVien nv)
            {
                DanhSachNhanVien.Remove(nv);
            }
        }

        // Chấm công
        private void BtnCheckInOut_Click(object sender, RoutedEventArgs e)
        {
            if (dgNhanVien.SelectedItem is NhanVien nv && dpNgayChamCong.SelectedDate.HasValue)
            {
                var ngay = dpNgayChamCong.SelectedDate.Value;
                var gio = DateTime.Now.ToString("HH:mm");

                var chamCong = DanhSachChamCong.FirstOrDefault(cc =>
                    cc.MaNV == nv.MaNV && cc.Ngay.Date == ngay.Date);

                if (chamCong == null)
                {
                    // Check-in lần đầu
                    DanhSachChamCong.Add(new ChamCong
                    {
                        MaNV = nv.MaNV,
                        TenNV = nv.TenNV,
                        Ngay = ngay,
                        CheckIn = gio,
                        CheckOut = ""
                    });
                }
                else if (string.IsNullOrEmpty(chamCong.CheckOut))
                {
                    // Check-out
                    chamCong.CheckOut = gio;
                    dgChamCong.Items.Refresh();
                }
                else
                {
                    // Đã check-out rồi, không làm gì
                }
            }
        }

        // Đặt ngày về hôm nay
        private void BtnHomNay_Click(object sender, RoutedEventArgs e)
        {
            dpNgayChamCong.SelectedDate = DateTime.Today;
        }

        // Clear form nhập liệu
        private void ClearForm()
        {
            txtMaNV.Clear();
            txtTenNV.Clear();
            cbVaiTro.SelectedIndex = -1;
            chkQuanLy.IsChecked = false;
            txtCaLam.Clear();
        }

        // Model nhân viên
        public class NhanVien
        {
            public string MaNV { get; set; }
            public string TenNV { get; set; }
            public string VaiTro { get; set; }
            public bool IsQuanLy { get; set; }
            public string CaLamViec { get; set; }
        }

        // Model chấm công
        public class ChamCong
        {
            public string MaNV { get; set; }
            public string TenNV { get; set; }
            public DateTime Ngay { get; set; }
            public string CheckIn { get; set; }
            public string CheckOut { get; set; }
        }
    }
}