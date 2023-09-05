using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ac.Models
{
    public class SpecProcesses
    {
        public string DEV_SN { get; set; }          //серийник
        public string OP_NAME { get; set; }
        public string OP_DESCR { get; set; }
        public int? OP_EL_ID { get; set; }
        public string CurTemp { get; set; }
        public string CurHum { get; set; }
        public int? TL_TYPE { get; set; }
        
        public string TL_VALUE1 { get; set; }
        public string TL_VALUE2 { get; set; }
        public string TL_VALUE { get; set; }

        public DateTime? T_START { get; set; }
        public DateTime? T_END { get; set; }

        public string USER_SFIO { get; set; }

        public int Status { get; set; }

        public string Temp
        {
            get
            {
                return CurTemp + "°C";
            }
        }
    }
}
