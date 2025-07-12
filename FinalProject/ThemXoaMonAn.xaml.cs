#nullable enable
using FinalProject.Models;
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
using System.Windows.Shapes;
using Microsoft.Win32;
using Npgsql;
using System.IO;

namespace FinalProject
{
    public partial class ThemXoaMonAn : Window
    {
        // Chuỗi kết nối đến cơ sở dữ liệu PostgreSQL
        private readonly string connectionString = "Host=ep-super-frost-a1wzegym-pooler.ap-southeast-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_NZgous1jTzB9;SSL Mode=Require;Trust Server Certificate=true";
        private readonly Action? onReloadMenuCallback;

        public ObservableCollection<ThucDon> ThucDons { get; set; } = new();
        private ObservableCollection<ThucDon> listMonChinh;
        private ObservableCollection<ThucDon> listTrangMieng;
        private ObservableCollection<ThucDon> listTraTraiCay;
        private ObservableCollection<ThucDon> listNuocEp;
        private ObservableCollection<ThucDon> listKhuyenMai;

        public ObservableCollection<ThucDon> ListMonChinh => listMonChinh; // để binding XAML

        public ThemXoaMonAn(
            ObservableCollection<ThucDon> monChinh,
            ObservableCollection<ThucDon> trangMieng,
            ObservableCollection<ThucDon> traTraiCay,
            ObservableCollection<ThucDon> nuocEp,
            ObservableCollection<ThucDon> khuyenMai,
            Action? onReload = null)
        {
            InitializeComponent();
            DataContext = this;

            listMonChinh = monChinh;
            listTrangMieng = trangMieng;
            listTraTraiCay = traTraiCay;
            listNuocEp = nuocEp;
            listKhuyenMai = khuyenMai;
            onReloadMenuCallback = onReload;
        }

        public void LoadThucDonFromDatabase()
        {
            // Clear danh sách cũ
            ThucDons.Clear();
            listMonChinh.Clear();
            listTrangMieng.Clear();
            listTraTraiCay.Clear();
            listNuocEp.Clear();
            listKhuyenMai.Clear();

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                string query = "SELECT itemname, imageurl, category FROM menuitems";
                using var cmd = new NpgsqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var item = new ThucDon
                    {
                        ItemName = reader.GetString(0),
                        ImagePath = reader.IsDBNull(1) ? "" : reader.GetString(1),
                        Rating = 5
                    };

                    string category = reader.IsDBNull(2) ? "Khac" : reader.GetString(2);

                    switch (category)
                    {
                        case "MonChinh":
                            listMonChinh.Add(item);
                            break;
                        case "TrangMieng":
                            listTrangMieng.Add(item);
                            break;
                        case "TraTraiCay":
                            listTraTraiCay.Add(item);
                            break;
                        case "NuocEp":
                            listNuocEp.Add(item);
                            break;
                        case "KhuyenMai":
                            listKhuyenMai.Add(item);
                            break;
                        default:
                            // Nếu bạn có list "Khac", thêm vào đây
                            break;
                    }

                    ThucDons.Add(item); // Nếu ThucDons dùng cho combo box sửa/xoá
                }

                // Cập nhật lại ComboBox sửa & xoá (tùy bạn có thể lọc theo danh mục khác nữa)
                cbXoa_TenMonAn.ItemsSource = ThucDons.Select(m => m.ItemName).ToList();
                cbSua_TenMonAn.ItemsSource = ThucDons.Select(m => m.ItemName).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải thực đơn từ CSDL: " + ex.Message);
            }
        }



        #region === THÊM MÓN ===

        private void btn_Them_Click(object sender, RoutedEventArgs e)
        {
            if (cbThem_LoaiMonAn.SelectedItem is ComboBoxItem selectedItem &&
                !string.IsNullOrWhiteSpace(cbAddName.Text) &&
                !string.IsNullOrWhiteSpace(cbAddImage.Text) &&
                decimal.TryParse(cbAddPrice.Text, out decimal gia))
            {
                string itemName = cbAddName.Text;
                string imageUrl = cbAddImage.Text;
                string rawCategory = cbThem_LoaiMonAn.Text;
                string mappedCategory = ChuyenDanhMuc(rawCategory);

                using (var conn = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string insertQuery = @"
                    INSERT INTO menuitems (itemname, category, price, imageurl)
                    VALUES (@name, @category, @price, @image)";

                        using (var cmd = new NpgsqlCommand(insertQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", itemName);
                            cmd.Parameters.AddWithValue("@category", mappedCategory);
                            cmd.Parameters.AddWithValue("@price", gia);
                            cmd.Parameters.AddWithValue("@image", imageUrl);
                            cmd.ExecuteNonQuery();
                        }

                        // ✅ Tạo món mới để thêm vào danh sách hiển thị
                        var monMoi = new ThucDon
                        {
                            ItemName = itemName,
                            ImagePath = imageUrl,
                            Rating = 5 // hoặc mặc định
                        };

                        // ✅ Thêm vào đúng danh sách danh mục tương ứng
                        switch (mappedCategory)
                        {
                            case "MonChinh": listMonChinh.Add(monMoi); break;
                            case "TrangMieng": listTrangMieng.Add(monMoi); break;
                            case "TraTraiCay": listTraTraiCay.Add(monMoi); break;
                            case "NuocEp": listNuocEp.Add(monMoi); break;
                            case "KhuyenMai": listKhuyenMai.Add(monMoi); break;
                                // bạn có thể thêm case "MonBanChay" nếu có
                        }

                        MessageBox.Show("Thêm món ăn thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Reset form
                        cbAddName.Clear();
                        cbAddPrice.Clear();
                        cbAddImage.Clear();
                        cbThem_LoaiMonAn.SelectedIndex = -1;
                        onReloadMenuCallback?.Invoke();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi thêm món: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng nhập đầy đủ và hợp lệ các thông tin!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn ảnh món ăn",
                Filter = "Ảnh (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                cbAddImage.Text = openFileDialog.FileName;
            }
        }


        private string ChuyenDanhMuc(string text)
        {
            switch (text.Trim().ToLowerInvariant())
            {
                case "món chính": case "mon chinh": return "MonChinh";
                case "tráng miệng": case "trang mieng": return "TrangMieng";
                case "trà trái cây": case "tra trai cay": return "TraTraiCay";
                case "nước ép": case "nuoc ep": return "NuocEp";
                case "khuyến mãi": case "khuyen mai": return "KhuyenMai";
                case "món bán chạy": case "mon ban chay": return "MonBanChay";
                default: return "Khac";
            }
        }

        private ObservableCollection<ThucDon>? GetSelectedCategoryList(ComboBox comboBox)
        {
            var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return null;

            string category = selectedItem.Content.ToString();

            return category switch
            {
                "Món chính" => listMonChinh,
                "Món tráng miệng" => listTrangMieng,
                "Trà trái cây" => listTraTraiCay,
                "Nước ép" => listNuocEp,
                "Món khuyến mãi" => listKhuyenMai,
                _ => null
            };
        }

        #endregion

        #region === SỬA MÓN ===

        private void cbSua_LoaiMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ObservableCollection<ThucDon>? selectedList = GetSelectedCategoryList(cbSua_LoaiMonAn);

            if (selectedList != null)
            {
                cbSua_TenMonAn.ItemsSource = selectedList.Select(item => item.ItemName).ToList();
            }
            else
            {
                cbSua_TenMonAn.ItemsSource = null;
            }
        }


        private void cbSua_TenMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedList = GetSelectedCategoryList(cbSua_LoaiMonAn);
            if (cbSua_TenMonAn.SelectedItem == null || selectedList == null) return;

            var selectedName = cbSua_TenMonAn.SelectedItem.ToString();
            var selectedDish = selectedList.FirstOrDefault(m => m.ItemName == selectedName);

            if (selectedDish != null)
            {
                txtSua_OldImage.Text = selectedDish.ImagePath;
                txt_OldRating.Text = selectedDish.Rating.ToString();
            }
        }


        private void btn_NewImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn ảnh món ăn",
                Filter = "Ảnh (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                txt_NewImage.Text = openFileDialog.FileName;
            }
        }

        private void btn_Sua_Click(object sender, RoutedEventArgs e)
        {
            if (cbSua_LoaiMonAn.SelectedItem is not ComboBoxItem selectedCategoryItem ||
                cbSua_TenMonAn.SelectedItem is null)
            {
                MessageBox.Show("Vui lòng chọn danh mục và món cần sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string oldName = cbSua_TenMonAn.SelectedItem.ToString()!;
            string newName = string.IsNullOrWhiteSpace(txtSua_NewName.Text) ? oldName : txtSua_NewName.Text.Trim();
            string newImage = string.IsNullOrWhiteSpace(txt_NewImage.Text) ? txtSua_OldImage.Text : txt_NewImage.Text.Trim();

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                var cmd = new NpgsqlCommand(@"
            UPDATE menuitems 
            SET itemname = @newname, imageurl = @newimage 
            WHERE itemname = @oldname", conn);

                cmd.Parameters.AddWithValue("@newname", newName);
                cmd.Parameters.AddWithValue("@newimage", newImage);
                cmd.Parameters.AddWithValue("@oldname", oldName);

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    MessageBox.Show("Cập nhật món ăn thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow?.MainFrame.Content is QlyThucDon qlyPage)
                    {
                        qlyPage.Reload();
                    }


                }
                else
                {
                    MessageBox.Show("Không tìm thấy món để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật món ăn: " + ex.Message);
            }
        }



        #endregion

        #region === XÓA MÓN ===

        private void cbXoa_LoaiMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var danhSach = GetSelectedCategoryList(cbXoa_LoaiMonAn);

            if (danhSach != null)
            {
                cbXoa_TenMonAn.ItemsSource = danhSach.Select(m => m.ItemName).ToList();
            }
        }

        private void cbXoa_TenMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedList = GetSelectedCategoryList(cbXoa_LoaiMonAn);
            if (cbXoa_TenMonAn.SelectedItem == null || selectedList == null) return;
        }

        private void btn_Xoa_Click(object sender, RoutedEventArgs e)
        {
            if (cbXoa_LoaiMonAn.SelectedItem is not ComboBoxItem selectedCategoryItem)
            {
                MessageBox.Show("Vui lòng chọn danh mục món ăn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string? selectedDish = cbXoa_TenMonAn.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedDish))
            {
                MessageBox.Show("Vui lòng chọn món ăn để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var conn = new NpgsqlConnection(connectionString);
                conn.Open();

                var cmd = new NpgsqlCommand("DELETE FROM menuitems WHERE itemname = @name", conn);
                cmd.Parameters.AddWithValue("@name", selectedDish);

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    // ✅ Cập nhật lại danh sách sau khi xóa
                    LoadThucDonFromDatabase();

                    MessageBox.Show($"Đã xóa món \"{selectedDish}\" khỏi danh mục \"{selectedCategoryItem.Content}\".", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy món ăn để xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa món ăn: " + ex.Message);
            }
        }



        #endregion

    }
}