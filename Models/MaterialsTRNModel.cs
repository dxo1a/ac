using System;

namespace ac.Models
{
    public class MaterialsTRNModel
    {
        public string PRTIDN { get; set; }
        public string Material { get; set; }
        public double QTY { get; set; }
        public string EIZ { get; set; }
        public string WRH { get; set; }
        public string SN { get; set; }
        public string IDN { get; set; }
        public DateTime? DTAPostav { get; set; }
    }
}
