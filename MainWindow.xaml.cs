using ac.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ac
{
    public partial class MainWindow : Window
    {
        #region Переменные
        List<DetailsView> detailsView = new List<DetailsView>();
        List<DetailsView> detailsViewWithOP = new List<DetailsView>();

        DetailsView SelectedDetail { get; set; }
        public ViewModel ViewModel { get; set; }
        DetailsView productNameAndStatus { get; set; }

        public string SerialNumber;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Odb.db = new System.Data.Entity.DbContext("Data Source=sql;initial catalog=dsl_sp;MultipleActiveResultSets=True;App=EntityFramework&quot;Integrated Security=SSPI;");

            ViewModel = new ViewModel();
            DataContext = ViewModel;

            /*byte[] fileByte = Odb.db.Database.SqlQuery<byte[]>("SELECT Data FROM dsl_sp.dbo.CC_SUB_VIEW WHERE CARD_ID = 291").FirstOrDefault();
            SaveByteArrayToFileWithBinaryWriter(fileByte, "C:\\Users\\it01\\Documents\\pic.jpg");*/

        }

        #region Асинхронная загрузка деталей
        private async void UpdateGrid()
        {
            try
            {
                PBDetailsDG.Value = 0;
                DetailsDG.ItemsSource = new List<DetailsView>();
                DisableActions();
                await PrintDetails();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                EnableActions();
            }
        }

        async Task PrintDetails()
        {
            Console.WriteLine("-----Загрузка деталей началась-----");
            detailsView = await GetDetailsAsync();
            Console.WriteLine("-----Загрузка деталей закончена----");

            Dispatcher.Invoke(() => { DetailsDG.ItemsSource = detailsView; });
        }

        async Task<List<DetailsView>> GetDetailsAsync()
        {
            List<DetailsView> details = new List<DetailsView>();
            string numpp = PPTBX.Text;

            using (SqlConnection connection = new SqlConnection(Odb.db.Database.Connection.ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT НомерД AS DetailNode, НазваниеД AS DetailName, Договор AS PP, ПрП AS PrP, (SELECT TOP(1) Data FROM dsl_sp.dbo.DEV_IMAGES_V as img WHERE НомерД = img.PRT$$$MNF) as Data FROM [Cooperation].[dbo].[DetailsView] WHERE Договор='" + numpp + "' GROUP BY Договор, НомерД, НазваниеД, ПрП", connection);
                var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    DetailsView detail = new DetailsView()
                    {
                        DetailNode = reader.GetString(0),
                        DetailName = reader.GetString(1),
                        PP = reader.GetString(2),
                        PrP = reader.GetString(3),
                        Data = reader.IsDBNull(4) ? new byte[0] : (byte[])reader["Data"]
                    };
                    details.Add(detail);
                }
                connection.Close();
            }
            #region progressBar
            int totalItems = details.Count;
            int processedItems = 0;
            foreach (DetailsView detail in details)
            {
                processedItems++;
                PBDetailsDG.Value = (double)processedItems / totalItems * 100;
                await Task.Delay(1);
            }
            #endregion

            GC.Collect();
            GC.WaitForPendingFinalizers();
            return details;
        }
        #endregion

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

        #region Поиск деталей
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
                string numppFromNumser = Odb.db.Database.SqlQuery<string>("SELECT DISTINCT SP_SS.NUM_PP FROM SP_SS LEFT JOIN SS_DEV_NUM as devnum on SP_SS.SS_ID = devnum.SS_ID INNER JOIN CC_SUB_VIEW as subview on devnum.DEV_SN = subview.CC_SN WHERE subview.CC_ZN=@numser OR devnum.DEV_SN=@numser", new SqlParameter("numser", SerialNumberTBX.Text)).SingleOrDefault();

                if (!string.IsNullOrEmpty(numppFromNumser))
                {
                    PPTBX.Text = numppFromNumser;
                    SerialNumber = SerialNumberTBX.Text;

                    productNameAndStatus = Odb.db.Database.SqlQuery<DetailsView>("SELECT TOP (1) НомерИ AS ProductNum, Изделие AS ProductName, RSTS FROM [Cooperation].[dbo].[DetailsView] WHERE Договор=@numpp", new SqlParameter("numpp", numppFromNumser)).SingleOrDefault();
                    PPNameTB.Text = productNameAndStatus.Product;

                    #region Цвет в зависимости от статуса
                    if (productNameAndStatus.RSTS == 4)
                    {
                        PPNameTB.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF56DA56"));
                    }
                    #endregion

                    UpdateGrid();

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
        #endregion

        #region IMG
        /*
        private void UpdateImage(List<DetailsView> detail)
        {
            if (ImgCB.IsChecked.Value)
            {
                HashSet<string> DetNode = new HashSet<string>();
                detailsView.ForEach(u => DetNode.Add(u.DetailNode));
                foreach (string detnode in DetNode)
                {
                    List<DetailsView> det = detailsView.Where(u => u.DetailNode == detnode).ToList();
                    if (det.Count == 0) continue;
                    byte[] img = Odb.db.Database.SqlQuery<byte[]>("select Data from dsl_sp.dbo.DEV_IMAGES_V where PRT$$$MNF = @param1",
                        new SqlParameter("@param1", detnode)).FirstOrDefault();
                    if (img == null) continue;
                    det.ForEach(u => u.Data = img);
                }
            }
        }
        */

        private void ImgCB_Checked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility = Visibility.Visible;
        }

        private void ImgCB_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Копирование ПП
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
        #endregion

        #region Информация о детали
        private void DetailsDG_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedDetail = (DetailsView)DetailsDG.SelectedItem;
            if (DetailsDG.SelectedItem is DetailsView selectedDetail)
            {
                OperationsWindow operationsWindow = new OperationsWindow(SelectedDetail, SerialNumber, productNameAndStatus);
                operationsWindow.Show();
            }
        }

        private void DetailsDG_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedDetail = (DetailsView)DetailsDG.SelectedItem;
            if (DetailsDG.SelectedItem is DetailsView selectedDetail)
            {
                PPPrPTB.Text = "ПрП: " + SelectedDetail.PrP;
            }
        }
        #endregion

        #region Поиск и контроль элементов
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }

        private void DisableActions()
        {
            foreach (Button btn in FindVisualChildren<Button>(this))
            {
                btn.IsEnabled = false;
            }
            foreach (TextBox tbx in FindVisualChildren<TextBox>(this))
            {
                tbx.IsEnabled = false;
            }
            ImgCB.IsEnabled = false;
        }

        private void EnableActions()
        {
            foreach (Button btn in FindVisualChildren<Button>(this))
            {
                btn.IsEnabled = true;
            }
            foreach (TextBox tbx in FindVisualChildren<TextBox>(this))
            {
                tbx.IsEnabled = true;
            }
            ImgCB.IsEnabled = true;
        }
        #endregion

        /*public static void SaveByteArrayToFileWithBinaryWriter(byte[] data, string filePath)
        {
            var writer = new BinaryWriter(File.OpenWrite(filePath));
            writer.Write(data);
        }
        public static void SaveByteArrayToFileWithStaticMethod(byte[] data, string filePath) => File.WriteAllBytes(filePath, data);*/

        private void OPSPCatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            OPSPCatalogWindow oPSPCatalogWindow = new OPSPCatalogWindow();
            oPSPCatalogWindow.Show();
        }
    }
}
