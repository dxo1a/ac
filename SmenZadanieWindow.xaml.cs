using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ac
{
    public partial class SmenZadanieWindow : Window
    {
        List<SmenZadanie> SZList = new List<SmenZadanie>();

        private Operations SelectedOperation { get; set; }
        private DetailsView SelectedDetail { get; set; }
        private DetailsView ProductName { get; set; }

        public SmenZadanieWindow(Operations selectedOperation, DetailsView prodName, DetailsView selectedDetail, List<SmenZadanie> szList)
        {
            InitializeComponent();
            SelectedOperation = selectedOperation;
            ProductName = prodName;
            SelectedDetail = selectedDetail;

            this.Title = SelectedDetail.Detail + " - " + SelectedOperation.OperationName;

            //MessageBox.Show($"Operation: {SelectedOperation.OperationName} | OperationNum: {SelectedOperation.OperationNum} | Product: {ProductName.ProductWithSpace} | PrP: {SelectedDetail.PrP}");

            SZList = szList;
            SmenZadanieDG.ItemsSource = SZList;
        }

        #region Поиск
        private void search()
        {
            List<SmenZadanie> sz = new List<SmenZadanie>();
            string txt = SearchSPTBX.Text;
            if (txt.Length == 0)
                sz = SZList;
            sz = SZList.Where(u => u.SmenZadString.ToLower().Contains(txt.ToLower())).ToList();
            SmenZadanieDG.ItemsSource = sz;
        }

        private void SearchSPTBX_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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
        #endregion
    }
}
