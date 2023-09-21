using System;
using System.Windows.Media;

namespace ac.Models
{
    public class SpecProcesses
    {
        public string NUM_NOM { get; set; }
        public string DEV_SN { get; set; }          //серийник
        public string OP_NAME { get; set; }
        public string OP_DESCR { get; set; }
        public int? OP_EL_ID { get; set; }
        public string CurTemp { get; set; }
        public string CurHum { get; set; }
        public int? TL_TYPE { get; set; }
        public string EL_NAME { get; set; }

        public string OP_TYPE_NAME { get; set; }

        public string TL_VALUE1 { get; set; }
        public string TL_VALUE2 { get; set; }
        public string TL_VALUE { get; set; }
        public string TL_RESULT { get; set; }
        public Brush TL_VALUE_COLOR { get; set; }

        public DateTime? T_START { get; set; }
        public DateTime? T_END { get; set; }
        public string T_START_Date { get; set; }
        public string T_END_Date { get; set; }
        public string T_START_Time { get; set; }
        public string T_END_Time { get; set; }

        public bool? EL_TMP_RESULT { get; set; }
        public Brush EL_TMP_RESULT_COLOR { get; set; }
        public bool? EL_HUM_RESULT { get; set; }
        public Brush EL_HUM_RESULT_COLOR { get; set; }

        public string USER_SFIO { get; set; }

        public int Status { get; set; }
        public bool StatusBool { get; set; }

        public string Temp
        {
            get
            {
                return CurTemp + "°C";
            }
        }
    }
}
