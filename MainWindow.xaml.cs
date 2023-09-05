using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System;
using System.Windows.Media;

namespace ac
{
    public partial class MainWindow : Window
    {
        List<DetailsView> detailsView = new List<DetailsView>();
        DetailsView SelectedDetail { get; set; }

        public string serialNumber;
        public ViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new ViewModel();
            DataContext = ViewModel;

            Odb.db = new System.Data.Entity.DbContext("Data Source=sql;initial catalog=dsl_sp;MultipleActiveResultSets=True;App=EntityFramework&quot;Integrated Security=SSPI;");

            updateGrid();
        }

        private async void FindPPBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SerialNumberTBX.Text))
            {
                #region Анимация
                SerialNumberTBX.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2F2F"));
                await Task.Delay(2000);
                SerialNumberTBX.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFABADB3"));
                #endregion Анимация
            }
            else
            {
                // 1 - из dsl_sp
                string numppFromNumser = Odb.db.Database.SqlQuery<string>("SELECT NUM_PP FROM SP_SS LEFT JOIN SS_DEV_NUM ON SP_SS.SS_ID = SS_DEV_NUM.SS_ID WHERE DEV_SN=@numser", new SqlParameter("numser", SerialNumberTBX.Text)).SingleOrDefault();

                if (!string.IsNullOrEmpty(numppFromNumser))
                {
                    PPTBX.Text = numppFromNumser;

                    DetailsView productNameAndStatus = Odb.db.Database.SqlQuery<DetailsView>("SELECT TOP (1) НомерИ AS ProductNum, Изделие AS ProductName, RSTS FROM [Cooperation].[dbo].[DetailsView] WHERE Договор=@numpp", new SqlParameter("numpp", numppFromNumser)).SingleOrDefault();
                    PPNameTB.Text = productNameAndStatus.Product;
                    serialNumber = SerialNumberTBX.Text;

                    #region Цвет в зависимости от статуса
                    if (productNameAndStatus.RSTS == 4)
                    {
                        PPNameTB.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF56DA56"));
                    }
                    #endregion

                    updateGrid();
                    
                }
                else
                {
                    #region Анимация подсказки
                    SNNotFoundTB.Visibility = Visibility.Visible;
                    DoubleAnimation fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                    SNNotFoundTB.BeginAnimation(OpacityProperty, fadeInAnimation);

                    await Task.Delay(1000);

                    DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
                    SNNotFoundTB.BeginAnimation(OpacityProperty, fadeOutAnimation);
                    #endregion
                }
            }
        }

        private void updateGrid()
        {
            //2 - из Cooperation
            detailsView = Odb.db.Database.SqlQuery<DetailsView>("SELECT НомерД AS DetailNode, НазваниеД AS DetailName, Договор AS PP, ПрП AS PrP FROM [Cooperation].[dbo].[DetailsView] WHERE Договор=@numpp GROUP BY Договор, НомерД, НазваниеД, ПрП", new SqlParameter("numpp", PPTBX.Text)).ToList();
            DetailsDG.ItemsSource = detailsView;
        }

        #region Поиск в таблице
        private void DetailSearchBTN_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsDetailSearchVisible = !ViewModel.IsDetailSearchVisible;
        }

        private void DetailSearchTBX_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                searchInDG();
            }
        }

        private void searchInDG()
        {
            List<DetailsView> details = new List<DetailsView>();
            string txt = DetailSearchTBX.Text;
            if (txt.Length == 0)
                details = detailsView;
            details = detailsView.Where(u => u.Detail.ToLower().Contains(txt.ToLower())).ToList();
            DetailsDG.ItemsSource = details;
        }
        #endregion

        // Копирование ПП
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
        }

        // Инфо о детали
        private void DetailsDG_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedDetail = (DetailsView)DetailsDG.SelectedItem;
            if (DetailsDG.SelectedItem is DetailsView selectedDetail)
            {
                PPPrP.Text = "ПрП: " + SelectedDetail.PrP;
                DetailInfoWindow detailInfoWindow = new DetailInfoWindow(SelectedDetail, serialNumber);
                detailInfoWindow.Show();
            }
        }
    }
}
