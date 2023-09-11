using System;

namespace ac.Models
{
    public class Sp
    {
        public string DEV_SN { get; set; }          //серийник
        public DateTime? SS_START { get; set; }
        public DateTime? SS_END { get; set; }
        public string NUM_PP { get; set; }          // ПП
        public string NUM_PAR { get; set; }         // ПрП
        public string DEV_NAME { get; set; }
        public bool WARN { get; set; }              // Наличие нарушений в СП (спецпроцессе)
        public int SP_CR_USER { get; set; }         // id создавшего деталь

    }
}
