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
    public partial class OPSPAddWindow : Window
    {
        List<Operation> operationsList = new List<Operation>();
        List<SpecProcess> specprocessesList = new List<SpecProcess>();

        Operation SelectedOperation { get; set; }
        SpecProcess SelectedSpecProcess { get; set; }

        int operationID, specprocessID;

        public OPSPAddWindow()
        {
            InitializeComponent();
            operationsList = Odb.db.Database.SqlQuery<Operation>("select distinct ID_Operation as SID_Operation, Operation as SOperation from SerialNumber.dbo.OP").ToList();
            specprocessesList = Odb.db.Database.SqlQuery<SpecProcess>("select distinct ID_SpecProcess as SID_SpecProcess, SpecProcess as SSpecProcess from SerialNumber.dbo.SP").ToList();
            OperationsLB.ItemsSource = operationsList;
            SpecProcessesLB.ItemsSource = specprocessesList;
        }
            
        private void OperationsTB_GotFocus(object sender, RoutedEventArgs e)
        {
            OperationsLB.Visibility = Visibility.Visible;
            AddBtn.Visibility = Visibility.Hidden;
        }

        private void OperationsTB_LostFocus(object sender, RoutedEventArgs e)
        {
            OperationsLB.Visibility = Visibility.Collapsed;
            AddBtn.Visibility = Visibility.Visible;
        }

        private void SpecProcessesTB_GotFocus(object sender, RoutedEventArgs e)
        {
            SpecProcessesLB.Visibility = Visibility.Visible;
            AddBtn.Visibility = Visibility.Hidden;
        }

        private void SpecProcessesTB_LostFocus(object sender, RoutedEventArgs e)
        {
            SpecProcessesLB.Visibility = Visibility.Collapsed;
            AddBtn.Visibility = Visibility.Visible;
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
            }
        }

        private void SpecProcessesLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSpecProcess = (SpecProcess)SpecProcessesLB.SelectedItem;
            if (SelectedSpecProcess != null)
            {
                SpecProcessesTB.Text = SelectedSpecProcess.ToString();
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

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            operationID = SelectedOperation.SID_Operation;
            specprocessID = SelectedSpecProcess.SID_SpecProcess;
            AddOPSP();
        }

        public void AddOPSP()
        {
            Odb.db.Database.ExecuteSqlCommand("INSERT INTO SerialNumber.dbo.OPSP_Link(ID_Operation, ID_SpecProcess) VALUES (@operationID, @specprocessID)", new SqlParameter("operationID", operationID), new SqlParameter("specprocessID", specprocessID));
            MessageBox.Show("Запись создана.", "Создание", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}