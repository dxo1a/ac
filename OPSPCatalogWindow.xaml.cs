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
        List<OPSPModel> groupedData = new List<OPSPModel>();

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
                var dialogResult = MessageBox.Show("Удалить операцию?.", "Создание", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    Odb.db.Database.ExecuteSqlCommand("delete from SerialNumber.dbo.OPSP_Link where ID_Operation=@operationID", new SqlParameter("operationID", SelectedOPSP.ID_Operation));
                    UpdateGrid();
                }
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
            if (OPSPDG.SelectedItem is OPSPModel selectedOPSP)
            {
                SelectedOPSPtoOperation = new Operation
                {
                    SID_Operation = selectedOPSP.ID_Operation,
                    SOperation = selectedOPSP.Operation
                };

                SelectedOPSPtoSpecProcess = new SpecProcess
                {
                    SID_SpecProcess = selectedOPSP.ID_SpecProcess,
                    SSpecProcess = selectedOPSP.SpecProcess
                };

                SelectedOPSP = selectedOPSP;
                OPSPEditBtn.IsEnabled = true;
                OPSPRemoveBtn.IsEnabled = true;
            }
            else
            {
                SelectedOPSPtoOperation = null;
                SelectedOPSPtoSpecProcess = null;
                SelectedOPSP = null;
                OPSPEditBtn.IsEnabled = false;
                OPSPRemoveBtn.IsEnabled = false;
            }
        }

        private void OPSPRefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        void UpdateGrid()
        {
            opspList = Odb.db.Database.SqlQuery<OPSPModel>("select distinct ID, a.ID_Operation, b.Operation, a.ID_SpecProcess, c.SpecProcess from SerialNumber.dbo.OPSP_Link as a left join SerialNumber.dbo.OP as b on a.ID_Operation = b.ID_Operation left join SerialNumber.dbo.SP as c on a.ID_SpecProcess = c.ID_SpecProcess").ToList();

            groupedData = opspList.GroupBy(link => link.Operation)
                           .Select(group => new OPSPModel
                           {
                               ID_Operation = group.First().ID_Operation,
                               Operation = group.Key,
                               SpecProcesses = group.Select(link => new SpecProcess
                               {
                                   SID_SpecProcess = link.ID_SpecProcess,
                                   SSpecProcess = link.SpecProcess
                               }).ToList()
                           })
                           .ToList();

            OPSPDG.ItemsSource = groupedData;
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
                opsp = groupedData;
            opsp = groupedData.Where(u => u.OPSPFull.ToLower().Contains(txt.ToLower())).ToList();
            OPSPDG.ItemsSource = opsp;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTBX.Clear();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
                    break;
                }
        }
    }
}
