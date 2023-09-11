using System;

namespace ac.Models
{
    public class Materials
    {
        public string PrP { get; set; }
        public string Matname { get; set; }
        public string Size { get; set; }
        public DateTime? EditDTE { get; set; }

        public string MaterialName
        {
            get
            {
                return Matname + EDSize;
            }
        }

        public string EDSize
        {
            get
            {
                if (Size == null)
                {
                    return "";
                }
                else
                {
                    return " (" + Size + ")";
                }
            }
        }

        public bool StatusBool { get; set; }
        public int Status { get; set; }
    }
}
