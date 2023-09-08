using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ac.Models
{
    public class SmenZadanie
    {
        public string OrderNum { get; set; }
        public string Product { get; set; }
        public string Detail { get; set; }
        public double Count { get; set; }
        public string NUM { get; set; }
        public string Operation { get; set; }
        public int OperationNum { get; set; }
        public string DEP { get; set; }
        public string WCR { get; set; }
        public int SHIFT { get; set; }
        public string FIO { get; set; }
        public DateTime? DTE { get; set; }

        public bool StatusBool { get; set; }
        public int Status { get; set; }
    }
}
