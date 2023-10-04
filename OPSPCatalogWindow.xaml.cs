using ac.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ac
{
    public partial class OPSPCatalogWindow : Window
    {
        List<OPSPModel> opspList = new List<OPSPModel>();
        List<Operation> operationsList = new List<Operation>();
        List<SpecProcess> specprocessesList = new List<SpecProcess>();

        OPSPModel SelectedOPSP { get; set; }
        SpecProcess SelectedOPSPtoSpecProcess { get; set; }
        Operation SelectedOPSPtoOperation { get; set; }

        public OPSPCatalogWindow()
        {
            InitializeComponent();
            UpdateGrid();
        }

        private void OPSPAddBtn_Click(object sender, RoutedEventArgs e)
        {
            OPSPAddWindow opspAddWindow = new OPSPAddWindow();
            opspAddWindow.ShowDialog();
        }

        private void OPSPRemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OPSPDG.SelectedItem is OPSPModel)
            {
                Odb.db.Database.ExecuteSqlCommand("delete from SerialNumber.dbo.OPSP_Link where ID_Operation=@operationID and ID_SpecProcess=@specprocessID", new SqlParameter("operationID", SelectedOPSP.ID_Operation), new SqlParameter("specprocessID", SelectedOPSP.ID_SpecProcess));
                UpdateGrid();
                var dialogResult = MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButton.YesNo);
            }
        }

        private void OPSPEditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OPSPDG.SelectedItem is OPSPModel)
            {
                OPSPEditWindow opspEditWindow = new OPSPEditWindow(SelectedOPSPtoOperation, SelectedOPSPtoSpecProcess, SelectedOPSP);
                opspEditWindow.ShowDialog();
            }
        }

        private void OPSPDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OPSPDG.SelectedItem is OPSPModel)
            {
                OPSPEditBtn.IsEnabled = true;
                OPSPRemoveBtn.IsEnabled = true;
            }
            else
            {
                OPSPEditBtn.IsEnabled = false;
                OPSPRemoveBtn.IsEnabled = false;
            }

            #region Convert SelectedOPSP to Operation
            SelectedOPSPtoOperation = new Operation();
            OPSPModel selectedOPSPfO = (OPSPModel)OPSPDG.SelectedItem;
            if (selectedOPSPfO != null)
            {
                Operation operationSelectedOPSP = new Operation
                {
                    SOperation = selectedOPSPfO.Operation,
                    SID_Operation = selectedOPSPfO.ID_Operation
                };
                SelectedOPSPtoOperation = operationSelectedOPSP;
            }
            #endregion

            #region Convert SelectedOPSP to SpecProcess
            SelectedOPSPtoSpecProcess = new SpecProcess();
            OPSPModel selectedOPSPfP = (OPSPModel)OPSPDG.SelectedItem;
            if (selectedOPSPfP != null)
            {
                SpecProcess specprocessSelectedOPSP = new SpecProcess
                {
                    SSpecProcess = selectedOPSPfP.SpecProcess,
                    SID_SpecProcess = selectedOPSPfP.ID_SpecProcess
                };
                SelectedOPSPtoSpecProcess = specprocessSelectedOPSP;
            }
            #endregion

            SelectedOPSP = (OPSPModel)OPSPDG.SelectedItem;
        }

        private void OPSPRefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        void UpdateGrid()
        {
            opspList = Odb.db.Database.SqlQuery<OPSPModel>("select distinct ID, a.ID_Operation, b.Operation, a.ID_SpecProcess, c.SpecProcess from SerialNumber.dbo.OPSP_Link as a left join SerialNumber.dbo.OP as b on a.ID_Operation = b.ID_Operation left join SerialNumber.dbo.SP as c on a.ID_SpecProcess = c.ID_SpecProcess").ToList();
            OPSPDG.ItemsSource = opspList;
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            search();
        }

        private void SearchTBX_TextChanged(object sender, TextChangedEventArgs e)
        {
            search();
        }

        private void search()
        {
            List<OPSPModel> opsp = new List<OPSPModel>();
            string txt = SearchTBX.Text;
            if (txt.Length == 0)
                opsp = opspList;
            opsp = opspList.Where(u => u.OPSPFull.ToLower().Contains(txt.ToLower())).ToList();
            OPSPDG.ItemsSource = opsp;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTBX.Clear();
        }
    }
}
