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

        public SmenZadanieWindow(Operations selectedOperation, DetailsView prodName, DetailsView selectedDetail)
        {
            InitializeComponent();
            SelectedOperation = selectedOperation;
            ProductName = prodName;
            SelectedDetail = selectedDetail;

            this.Title = SelectedDetail.Detail + " - " + SelectedOperation.OperationName;

            //MessageBox.Show($"Operation: {SelectedOperation.OperationName} | OperationNum: {SelectedOperation.OperationNum} | Product: {ProductName.ProductWithSpace} | PrP: {SelectedDetail.PrP}");

            SZList = Odb.db.Database.SqlQuery<SmenZadanie>("SELECT DISTINCT id_Tabel, DTE, Cost, FIO, NUM, Detail, OrderNum, OperationNum, Operation, Status AS StatusBool, Count, DEP$$$DEP AS DEP, WCR$$$WCR AS WCR, SHIFT, Product FROM [Zarplats].[dbo].[SmenZadView] WHERE Operation=@operationname AND OperationNum=@operationnum AND Product=@productname AND NUM=@prp", new SqlParameter("operationname", SelectedOperation.OperationName), new SqlParameter("operationnum", SelectedOperation.OperationNum), new SqlParameter("productname", ProductName.ProductWithSpace), new SqlParameter("@prp", SelectedDetail.PrP)).ToList();
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
