using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ac.Models
{
    public class MaterialsTRNModel
    {
        public string Detail { get; set; }
        public string PRTIDN { get; set; }
        public string Material { get; set; }
        public double QTY { get; set; }
        public string EIZ { get; set; }
        public string WRH { get; set; }
        public string SN { get; set; }
        public string IDN { get; set; }
        public DateTime? DTA { get; set; }
    }
}
