using System;

namespace ac.Models
{
    public class SmenZadanie
    {
        public string id_Tabel { get; set; }
        public string OrderNum { get; set; }
        public string Product { get; set; }
        public string Detail { get; set; }
        public double? Count { get; set; }
        public string NUM { get; set; }
        public string Operation { get; set; }
        public int OperationNum { get; set; }
        public double? Cost { get; set; }
        public string DEP { get; set; }
        public string WCR { get; set; }
        public int SHIFT { get; set; }
        public string FIO { get; set; }
        public DateTime? DTE { get; set; }

        public bool StatusBool { get; set; }
        public int Status { get; set; }

        public string CostString
        {
            get
            {
                return Convert.ToString(Cost);
            }
        }

        public string SmenZadString
        {
            get
            {
                return OrderNum + " " + Product + "" + Detail + " " + Convert.ToString(Count) + " " + Operation + " " + Convert.ToString(OperationNum) + " " + Convert.ToString(Cost) + " " + DEP + " " + WCR + " " + (int)SHIFT + FIO;
            }
        }

        public string Person
        {
            get
            {
                return $"({id_Tabel}) {FIO}";
            }
        }
    }
}
