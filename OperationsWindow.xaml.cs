using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Linq;

namespace ac
{
    public partial class OperationsWindow : Window
    {
        List<Operations> OperationsList = new List<Operations>();

        private DetailsView SelectedDetail { get; set; }
        
        public string SerialNumber;

        public OperationsWindow(DetailsView selectedDetail, string serialNumber)
        {
            InitializeComponent();
            this.Title = SelectedDetail.Detail;

            SelectedDetail = selectedDetail;
            SerialNumber = serialNumber;

            OperationsList = Odb.db.Database.SqlQuery<Operations>("SELECT Договор AS PP, НомерД AS DetailNode, Операция AS OperationName, НомерО AS OperationNum, Исполнитель AS Executor, Цена AS Price, Стоимость AS Cost FROM Cooperation.dbo.DetailsView WHERE НомерД=@detnode AND Договор=@numpp AND ПрП=@prp GROUP BY Договор, Операция, НомерО, Исполнитель, Цена, Стоимость, Количество, Статус, НомерД", new SqlParameter("detnode", SelectedDetail.DetailNode), new SqlParameter("numpp", SelectedDetail.PP), new SqlParameter("prp", SelectedDetail.PrP)).ToList();
            OperationsDG.ItemsSource = OperationsList;
        }

        #region ImgCB
        private void ImgCB_Checked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility = Visibility.Visible;
        }

        private void ImgCB_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility= Visibility.Collapsed;
        }
        #endregion

        private void SpecProcessesBtn_Click(object sender, RoutedEventArgs e)
        {
            SpecProcessInfoWindow specProcessInfoWindow = new SpecProcessInfoWindow(SelectedDetail, SerialNumber);
            specProcessInfoWindow.ShowDialog();
        }

        private void MaterialsBtn_Click(object sender, RoutedEventArgs e)
        {
            MaterialsWindow materialsWindow = new MaterialsWindow(SelectedDetail);
            materialsWindow.ShowDialog();
        }
    }
}
