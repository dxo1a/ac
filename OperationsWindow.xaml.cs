using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ac
{
    public partial class OperationsWindow : Window
    {
        List<Operations> OperationsList = new List<Operations>();

        private DetailsView SelectedDetail { get; set; }
        private Operations SelectedOperation { get; set; }
        private DetailsView ProductName { get; set; }

        public string SerialNumber;

        public OperationsWindow(DetailsView selectedDetail, string serialNumber, DetailsView productName)
        {
            InitializeComponent();
            
            SelectedDetail = selectedDetail;
            SerialNumber = serialNumber;
            ProductName = productName;

            OperationsList = Odb.db.Database.SqlQuery<Operations>("SELECT Договор AS PP, НомерД AS DetailNode, Операция AS OperationName, НомерО AS OperationNum, Исполнитель AS Executor, Цена AS Price, Стоимость AS Cost FROM Cooperation.dbo.DetailsView WHERE НомерД=@detnode AND Договор=@numpp AND ПрП=@prp GROUP BY Договор, Операция, НомерО, Исполнитель, Цена, Стоимость, Количество, Статус, НомерД", new SqlParameter("detnode", SelectedDetail.DetailNode), new SqlParameter("numpp", SelectedDetail.PP), new SqlParameter("prp", SelectedDetail.PrP)).ToList();
            OperationsDG.ItemsSource = OperationsList;

            this.Title = SelectedDetail.Detail;
        }

        #region ImgCB
        private void ImgCB_Checked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility = Visibility.Visible;
        }

        private void ImgCB_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility = Visibility.Collapsed;
        }
        #endregion

        private void SpecProcessesBtn_Click(object sender, RoutedEventArgs e)
        {
            SpecProcessInfoWindow specProcessInfoWindow = new SpecProcessInfoWindow(SelectedDetail, SerialNumber);
            specProcessInfoWindow.Show();
        }

        private void MaterialsBtn_Click(object sender, RoutedEventArgs e)
        {
            MaterialsWindow materialsWindow = new MaterialsWindow(SelectedDetail);
            materialsWindow.ShowDialog();
        }

        private void OperationsDG_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedOperation = (Operations)OperationsDG.SelectedItem;
            if (OperationsDG.SelectedItem is Operations selectedOperation)
            {
                SmenZadanieWindow smenZadanieWindow = new SmenZadanieWindow(SelectedOperation, ProductName, SelectedDetail);
                smenZadanieWindow.Show();
            }
        }
    }
}
