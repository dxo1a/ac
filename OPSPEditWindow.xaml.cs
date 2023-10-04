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
        List<SpecProcess> specProcessesFromOperation = new List<SpecProcess>();
        List<SpecProcess> selectedSpecProcesses = new List<SpecProcess>();

        Operation SelectedOperation { get; set; }
        SpecProcess SelectedSpecProcess { get; set; }
        OPSPModel SelectedOPSP { get; set; }

        int operationID, specprocessID;

        public OPSPEditWindow(Operation selectedOperation, SpecProcess selectedSpecProcess, OPSPModel selectedOPSB)
        {
            InitializeComponent();

            SelectedOperation = selectedOperation;
            SelectedSpecProcess = selectedSpecProcess;
            SelectedOPSP = selectedOPSB;

            specProcessesFromOperation = Odb.db.Database.SqlQuery<SpecProcess>("select distinct ID_SpecProcess as SID_SpecProcess, SpecProcess as SSpecProcess from SerialNumber.dbo.OPandSP where Operation=@operation", new SqlParameter("operation", SelectedOperation.SOperation)).ToList();

            operationsList = Odb.db.Database.SqlQuery<Operation>("select distinct ID_Operation as SID_Operation, Operation as SOperation from SerialNumber.dbo.OP").ToList();
            specprocessesList = Odb.db.Database.SqlQuery<SpecProcess>("select distinct ID_SpecProcess as SID_SpecProcess, SpecProcess as SSpecProcess from SerialNumber.dbo.SP").ToList();
            OperationsLB.ItemsSource = operationsList;
            SpecProcessesLB.ItemsSource = specprocessesList;

            OperationsLB.SelectedItem = SelectedOperation;
            OperationsTB.Text = SelectedOperation.SOperation;

            SpecProcessesLB.SelectedItem = SelectedSpecProcess;
            SpecProcessesTB.Text = string.Join(", ", specProcessesFromOperation.Select(sp => sp.SSpecProcess));
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOperation != null && selectedSpecProcesses.Count > 0)
            {
                using (SqlConnection connection = new SqlConnection(Odb.db.Database.Connection.ConnectionString))
                {
                    connection.Open();

                    string queryDelete = "DELETE FROM SerialNumber.dbo.OPSP_Link WHERE ID_Operation=@operationID";
                    using (SqlCommand command = new SqlCommand(queryDelete, connection))
                    {
                        command.Parameters.AddWithValue("@operationID", SelectedOperation.SID_Operation);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                if (SelectedOperation != null && SpecProcessesLB.SelectedItems.Count > 0)
                {
                    operationID = SelectedOperation.SID_Operation;
                    MessageBox.Show($"{operationID}");
                    foreach (SpecProcess selectedSpecProcess in SpecProcessesLB.SelectedItems)
                    {
                        specprocessID = selectedSpecProcess.SID_SpecProcess;
                        Odb.db.Database.ExecuteSqlCommand("INSERT INTO SerialNumber.dbo.OPSP_Link(ID_Operation, ID_SpecProcess) VALUES (@operationID, @specprocessID)", new SqlParameter("operationID", operationID), new SqlParameter("specprocessID", specprocessID));
                    }
                }
                else
                {
                    MessageBox.Show("Выберите операцию и хотя бы один специальный процесс.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                MessageBox.Show("Записи обновлены.", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите операцию и хотя бы один специальный процесс.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            SaveBtn.Visibility = Visibility.Visible;
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
            }
        }

        private void SpecProcessesLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedSpecProcesses.Clear();
            foreach (SpecProcess selectedSpecProcess in SpecProcessesLB.SelectedItems)
            {
                selectedSpecProcesses.Add(selectedSpecProcess);
            }

            SpecProcessesTB.Text = string.Join(", ", selectedSpecProcesses.Select(specProcess => specProcess.ToString()));
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

        private void CollapseSpecProcessesLBBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
