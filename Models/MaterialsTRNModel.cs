using ControlzEx.Standard;
using System;
using System.Security.Cryptography;

namespace ac.Models
{
    public class MaterialsTRNModel
    {
        public string DOC { get; set; }
        public string PRTIDN { get; set; }
        public string Material { get; set; }
        public double QTY { get; set; }
        public string EIZ { get; set; }
        public string WRH { get; set; }
        public string IDN { get; set; }
        public DateTime? DTAPostav { get; set; }
        
        public string Product { get; set; }
        public string PP { get; set; }
        public string SN { get; set; }

        public string SNListToString
        {
            get
            {
                return Product + " " + PP + "" + SN;
            }
        }
    }
}
