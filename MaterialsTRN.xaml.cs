using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ac
{
    public partial class MaterialsTRN : Window
    {
        List<MaterialsTRNModel> TrnsList = new List<MaterialsTRNModel>();
        
        List<MaterialsTRNModel> SNList = new List<MaterialsTRNModel>();

        //string PRP { get; set; }

        public MaterialsTRN(List<MaterialsTRNModel> trnsList, List<MaterialsTRNModel> snList)
        {
            InitializeComponent();
            SNList = snList;
            TrnsList = trnsList;

            TRNDG.IsHitTestVisible = false;
            TRNDG.ItemsSource = TrnsList;
            SNDG.ItemsSource = SNList;

            

            this.Title = TrnsList.First().Material + " | Поставка";
        }

        private void Expander_Process(object sender, RoutedEventArgs e)
        {
            if (sender is Expander expander)
            {
                var row = DataGridRow.GetRowContainingElement(expander);

                row.DetailsVisibility = expander.IsExpanded ? Visibility.Visible
                                                            : Visibility.Collapsed;
            }
        }

        #region Поиск
        private void SearchSNTBX_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                search();
            }
        }

        private void SearchSPBtn_Click(object sender, RoutedEventArgs e)
        {
            search();
        }

        private void search()
        {
            List<MaterialsTRNModel> mtm = new List<MaterialsTRNModel>();
            string txt = SearchSNTBX.Text;
            if (txt.Length == 0)
                mtm = SNList;
            mtm = SNList.Where(u => u.SNListToString.ToLower().Contains(txt.Substring(2).ToLower())).ToList();
            SNDG.ItemsSource = mtm;
        }
        #endregion
    }
}
