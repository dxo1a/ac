using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ac.Models
{
    public class SerialNumbersModel
    {
        public string DEV_SN { get; set; }
        public string CC_ZN { get; set; }
        public override string ToString()
        {
            if (CC_ZN == null)
            {
                return DEV_SN;
            }
            else
            {
                return DEV_SN + " (" + CC_ZN + ")";
            }
        }

        public string FullSerialNumber
        {
            get
            {
                if (CC_ZN == null)
                {
                    return DEV_SN;
                }
                else
                {
                    return DEV_SN + " (" + CC_ZN + ")";
                }
            }
        }
    }
}
