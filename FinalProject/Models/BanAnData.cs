using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class BanAn
    {
        public int TableID { get; set; }
        public string TableNumber { get; set; }
        public string TrangThai { get; set; }
        public override string ToString() => TableNumber;
    }
}
