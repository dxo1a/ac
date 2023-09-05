using ac.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Input;

namespace ac
{
    /// <summary>
    /// Логика взаимодействия для DetailInfo.xaml
    /// </summary>
    public partial class DetailInfoWindow : Window
    {
        private DetailsView SelectedDetail { get; set; }
        List<DetailsView> DetailInfo = new List<DetailsView>();

        public string SerialNumber;

        public DetailInfoWindow(DetailsView selectedDetail, string serialNumber)
        {
            InitializeComponent();
            SelectedDetail = selectedDetail;
            SerialNumber = serialNumber;
            this.Title = SelectedDetail.Detail;

            //DetailInfo = Odb.db.Database.SqlQuery<DetailsView>("SELECT ПрП AS PrP, Операция AS OperationName, НомерО AS OperationNum, Исполнитель AS Executor, Цена AS Price, Стоимость AS Cost, Количество AS Count, Статус AS Status, (SELECT TOP (1) Data from dsl_sp.dbo.DEV_IMAGES_V WHERE ПрП = NUMM) as Data FROM [Cooperation].[dbo].[DetailsView] LEFT JOIN dsl_sp.dbo.DEV_IMAGES_V ON НомерД = PRT$$$MNF WHERE НомерД=@detnode AND Договор=@numpp GROUP BY Операция, НомерО, Исполнитель, Цена, Стоимость, Количество, Статус, ПрП, НомерД, Договор", new SqlParameter("detnode", SelectedDetail.DetailNode), new SqlParameter("numpp", SelectedDetail.PP)).ToList();
            DetailInfo = Odb.db.Database.SqlQuery<DetailsView>("SELECT ПрП AS PrP, Операция AS OperationName, НомерО AS OperationNum, Исполнитель AS Executor, Цена AS Price, Стоимость AS Cost, Количество AS Count, Статус AS Status, (SELECT TOP (1) Data from dsl_sp.dbo.DEV_IMAGES_V WHERE ПрП = NUMM) as Data FROM [Cooperation].[dbo].[DetailsView] LEFT JOIN dsl_sp.dbo.DEV_IMAGES_V ON НомерД = PRT$$$MNF WHERE НомерД=@detnode AND Договор=@numpp AND ПрП=@prp GROUP BY Операция, НомерО, Исполнитель, Цена, Стоимость, Количество, Статус, ПрП, НомерД, Договор", new SqlParameter("detnode", SelectedDetail.DetailNode), new SqlParameter("numpp", SelectedDetail.PP), new SqlParameter("prp", SelectedDetail.PrP)).ToList();
            DetailDG.ItemsSource = DetailInfo;
        }

        private void ImgCB_Checked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility = Visibility.Visible;
        }

        private void ImgCB_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility= Visibility.Collapsed;
        }

        private void DetailDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DetailDG.SelectedItem is DetailsView detailsView)
            {
                SpecProcessInfoWindow specProcessInfoWindow = new SpecProcessInfoWindow(SelectedDetail, SerialNumber); //поменять на сущность операции SP_SS (и создать её) чтобы передавать именно операцию
                specProcessInfoWindow.ShowDialog();
            }
        }
    }
}
