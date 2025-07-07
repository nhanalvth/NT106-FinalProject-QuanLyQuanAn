using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class DonHang
    {
        public int DonHangID { get; set; }
        public string TrangThai { get; set; }
        public string TenBan { get; set; }
        public DateTime GioDat { get; set; }
        public decimal TongTien { get; set; }
    }


    public class ChiTietDonHang
    {
        public string TenMon { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public string GhiChu { get; set; }
    }

}
