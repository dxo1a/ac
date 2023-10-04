using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ac
{
    public class ProductModel
    {
        public string DEV_SN { get; set; }
        public string CC_ZN { get; set; }
        public string Product { get; set; }
        public string ProductNum { get; set; }
        public override string ToString()
        {
            return $"{ProductNum} {Product}";
        }

        public string ProductFull
        {
            get
            {
                return $"{ProductNum} {Product}";
            }
        }
    }
}
