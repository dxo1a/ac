using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ac.Models
{
    public class Materials
    {
        public string OrderNum { get; set; }
        public string PRC { get; set; }
        public string NM { get; set; }
        public double TotalPlanQTY { get; set; }
        public double TotalFactQTY { get; set; }
        public string Unit { get; set; }
        public string Size { get; set; }

        public string MaterialName
        {
            get
            {
                return PRC + " - " + NM;
            }
        }
    }
}
