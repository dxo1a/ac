using ac.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
    public partial class OPSPEditWindow : Window
    {
        List<Operation> operationsList = new List<Operation>();
        List<SpecProcess> specprocessesList = new List<SpecProcess>();

        Operation SelectedOperation { get; set; }
        SpecProcess SelectedSpecProcess { get; set; }
        OPSPModel SelectedOPSP { get; set; }

        public OPSPEditWindow(Operation selectedOperation, SpecProcess selectedSpecProcess, OPSPModel selectedOPSB)
        {
            InitializeComponent();

            SelectedOperation = selectedOperation;
            SelectedSpecProcess = selectedSpecProcess;
            SelectedOPSP = selectedOPSB;

            operationsList = Odb.db.Database.SqlQuery<Operation>("select distinct ID_Operation as SID_Operation, Operation as SOperation from SerialNumber.dbo.OP").ToList();
            specprocessesList = Odb.db.Database.SqlQuery<SpecProcess>("select distinct ID_SpecProcess as SID_SpecProcess, SpecProcess as SSpecProcess from SerialNumber.dbo.SP").ToList();
            OperationsLB.ItemsSource = operationsList;
            SpecProcessesLB.ItemsSource = specprocessesList;

            OperationsLB.SelectedItem = SelectedOperation;
            OperationsTB.Text = SelectedOperation.SOperation;

            SpecProcessesLB.SelectedItem = SelectedSpecProcess;
            SpecProcessesTB.Text = SelectedSpecProcess.SSpecProcess;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Odb.db.Database.ExecuteSqlCommand("update SerialNumber.dbo.OPSP_Link set ID_Operation=@operationID, ID_SpecProcess=@specprocessID where ID=@opspID", new SqlParameter("operationID", SelectedOperation.SID_Operation), new SqlParameter("specprocessID", SelectedSpecProcess.SID_SpecProcess), new SqlParameter("opspID", SelectedOPSP.ID));
            MessageBox.Show("Запись обновлена.", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OperationsTB_GotFocus(object sender, RoutedEventArgs e)
        {
            OperationsLB.Visibility = Visibility.Visible;
            SaveBtn.Visibility = Visibility.Hidden;
        }

        private void OperationsTB_LostFocus(object sender, RoutedEventArgs e)
        {
            OperationsLB.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Visible;
        }

        private void SpecProcessesTB_GotFocus(object sender, RoutedEventArgs e)
        {
            SpecProcessesLB.Visibility = Visibility.Visible;
            SaveBtn.Visibility = Visibility.Hidden;
        }

        private void SpecProcessesTB_LostFocus(object sender, RoutedEventArgs e)
        {
            SpecProcessesLB.Visibility = Visibility.Collapsed;
            SaveBtn.Visibility = Visibility.Visible;
        }

        private void OperationsTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txt = OperationsTB.Text;
            if (txt == null || txt == "")
            {
                OperationsLB.ItemsSource = operationsList;
            }
            else
                OperationsLB.ItemsSource = operationsList.Where(u => u.SOperation.ToLower().Contains(txt.ToLower())).ToList();
        }

        private void OperationsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedOperation = (Operation)OperationsLB.SelectedItem;
            if (SelectedOperation != null)
            {
                OperationsTB.Text = SelectedOperation.ToString();
                MessageBox.Show($"OP ID: {SelectedOperation.SID_Operation}");
            }
        }

        private void SpecProcessesLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSpecProcess = (SpecProcess)SpecProcessesLB.SelectedItem;
            if (SelectedSpecProcess != null)
            {
                SpecProcessesTB.Text = SelectedSpecProcess.ToString();
                MessageBox.Show($"SP ID: {SelectedSpecProcess.SID_SpecProcess}");
            }
        }

        private void SpecProcessesTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txt = SpecProcessesTB.Text;
            if (txt == null || txt == "")
            {
                SpecProcessesLB.ItemsSource = specprocessesList;
            }
            else
                SpecProcessesLB.ItemsSource = specprocessesList.Where(u => u.SSpecProcess.ToLower().Contains(txt.ToLower())).ToList();
        }
    }
}
