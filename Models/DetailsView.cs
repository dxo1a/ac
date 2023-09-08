using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ac.Models
{
    public class DetailsView
    {
        public string PP { get; set; }                  //Договор (плановая позиция)
        public double Count { get; set; }               //Количество
        public int Status { get; set; }                 //Статус (статус детали-операции)
        public bool StatusBool { get; set; }
        public int RSTS { get; set; }                   //RSTS  (статус плановой позиции)
        public string WCR { get; set; }                 //WCR (рабочий центр)
        public string PrP { get; set; }                 //ПрП (производственная партия)

        public string ProductName { get; set; }         //Изделие
        public string ProductNum { get; set; }          //НомерИ (номер изделия)

        public string DetailNode { get; set; }          //НомерД (номер детали/узел)
        public string DetailName { get; set; }          //НазваниеД (название детали)

        public DateTime? DateV { get; set; }            //ДатаВ (дата выпуска)
        public DateTime? DateZ { get; set; }            //ДатаЗ (дата по заказу (когда нужно изготовить))

        public string ProductRes { get; set; }

        public string Detail
        {
            get
            {
                return DetailNode + " - " + DetailName;
            }
        }

        public string Product
        {
            get
            {
                return ProductNum + " - " + ProductName;
            }
        }
    }
}
