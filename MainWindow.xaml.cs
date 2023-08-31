using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System;
using System.Windows.Media;
using System.Diagnostics;

namespace ac
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        List<DetailsView> detailsView = new List<DetailsView>();

        public MainWindow()
        {
            InitializeComponent();

            Odb.db = new System.Data.Entity.DbContext("Data Source=sql;initial catalog=dsl_sp;MultipleActiveResultSets=True;App=EntityFramework&quot;Integrated Security=SSPI;");

            updateGrid();
        }

        private void updateGrid()
        {
            //2 - из Cooperation
            detailsView = Odb.db.Database.SqlQuery<DetailsView>("SELECT НомерД AS DetailNode, НазваниеД AS DetailName, Операция AS OperationName, НомерО AS OperationNum, Исполнитель AS Executor, Цена AS Price, Стоимость AS Cost, Количество AS Count, Статус AS Status FROM [Cooperation].[dbo].[DetailsView] WHERE Договор=@numpp GROUP BY НомерД, НазваниеД, Операция, НомерО, Исполнитель, Цена, Стоимость, Количество, Статус", new SqlParameter("numpp", PPTBX.Text)).ToList();
            DetailsDG.ItemsSource = detailsView;
        }

        private async void FindPPBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SerialNumberTBX.Text))
            {
                #region Анимация
                SerialNumberTBX.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2F2F"));
                await Task.Delay(2000);
                SerialNumberTBX.BorderBrush = Brushes.LightGray;
                #endregion Анимация
            }
            else
            {
                // 1 - из dsl_sp
                string numppFromNumser = Odb.db.Database.SqlQuery<string>("SELECT NUM_PP FROM SP_SS LEFT JOIN SS_DEV_NUM ON SP_SS.SS_ID = SS_DEV_NUM.SS_ID WHERE DEV_SN=@numser", new SqlParameter("numser", SerialNumberTBX.Text)).SingleOrDefault();
                PPTBX.Text = numppFromNumser;

                DetailsView productNameAndStatus = Odb.db.Database.SqlQuery<DetailsView>("SELECT TOP (1) НомерИ AS ProductNum, Изделие AS ProductName, RSTS FROM [Cooperation].[dbo].[DetailsView] WHERE Договор=@numpp", new SqlParameter("numpp", numppFromNumser)).SingleOrDefault();
                PPNameTB.Text = productNameAndStatus.Product;

                #region Цвет в зависимости от статуса
                if (productNameAndStatus.RSTS == 4)
                {
                    PPNameTB.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF56DA56"));
                }
                #endregion

                updateGrid();
            }
        }

        private async void PPTBX_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(PPTBX.Text))
            {
                PPTBX.Copy();

                #region Анимация подсказки
                CopyNotificationTB.Visibility = Visibility.Visible;
                DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                CopyNotificationTB.BeginAnimation(OpacityProperty, fadeInAnimation);

                await Task.Delay(1000);

                DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
                CopyNotificationTB.BeginAnimation(OpacityProperty, fadeOutAnimation);
                #endregion
            }
            else
            {
                return;
            }
        }

        private void DetailsDG_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DetailsDG.SelectedItem is DetailsView selectedDetail)
            {
                DetailInfoWindow detailInfoWindow = new DetailInfoWindow(selectedDetail);
                detailInfoWindow.Show();
            }
        }
    }
}
