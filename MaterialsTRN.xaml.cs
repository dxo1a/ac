using ac.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ac
{
    public partial class MaterialsTRN : Window
    {
        List<MaterialsTRNModel> TrnsList = new List<MaterialsTRNModel>();
        List<MaterialsTRNModel> SerialNumbers = new List<MaterialsTRNModel>();

        Materials SelectedMaterial { get; set; }

        //string PRP { get; set; }

        public MaterialsTRN(List<MaterialsTRNModel> trnsList, List<MaterialsTRNModel> serialNumbers)
        {
            InitializeComponent();
            TrnsList = trnsList;
            SerialNumbers = serialNumbers;

            TRNDG.IsHitTestVisible = false;
            TRNDG.ItemsSource = TrnsList;
            SNDG.ItemsSource = SerialNumbers;

            this.Title = TrnsList.First().Material + " | Поставка";
        }
    }
}
