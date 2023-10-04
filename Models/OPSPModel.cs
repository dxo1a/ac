using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ac.Models
{
    public class OPSPModel
    {
        public int ID { get; set; }
        public int ID_Operation { get; set; }
        public int ID_SpecProcess { get; set; }
        public string Operation { get; set; }
        public string SpecProcess { get; set; }
        public override string ToString()
        {
            return Operation;
        }

        public string OPSPFull
        {
            get
            {
                return $"{Operation}{SpecProcess}";
            }
        }
    }

    public class Operation
    {
        public int SID_Operation { get; set; }
        public string SOperation { get; set; }
        public override string ToString()
        {
            return SOperation;
        }
    }

    public class SpecProcess
    {
        public int SID_SpecProcess { get; set; }
        public string SSpecProcess { get; set; }
        public override string ToString()
        {
            return SSpecProcess;
        }
    }
}
