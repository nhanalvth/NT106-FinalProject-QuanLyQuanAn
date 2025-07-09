using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class MonAn
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal Gia { get; set; }
        public override string ToString() => ItemName;
        public int SoLuong { get; set; }
        public string GhiChu { get; set; }
    }
}