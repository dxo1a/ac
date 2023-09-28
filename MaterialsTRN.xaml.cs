using ac.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ac
{
    public partial class MaterialsTRN : Window
    {
        List<MaterialsTRNModel> TrnsList = new List<MaterialsTRNModel>();

        Materials SelectedMaterial { get; set; }

        //string PRP { get; set; }

        public MaterialsTRN(Materials selectedMaterial, string prp, List<MaterialsTRNModel> trns)
        {
            InitializeComponent();
            SelectedMaterial = selectedMaterial;
            TrnsList = trns;

            DocTB.Text = $"DOC {SelectedMaterial.DOC}";
            //DocTB.Text = $"IDN {TrnsList.First().IDN}";
            TRNDG.IsHitTestVisible = false;
            TRNDG.ItemsSource = TrnsList;
            SNDG.ItemsSource = TrnsList;

            this.Title = TrnsList.First().Material + " | Поставка";
        }
    }
}
