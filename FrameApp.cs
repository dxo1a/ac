using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ac
{
    internal class FrameApp
    {
        public static Frame FrameObj { get; set; }
    }

    internal class Odb
    {
        public static System.Data.Entity.DbContext db { get; set; }
    }
}
