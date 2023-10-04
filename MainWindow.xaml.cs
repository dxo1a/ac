using ac.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ac
{
    public partial class MainWindow : Window
    {
        #region Переменные
        List<DetailsView> detailsView = new List<DetailsView>();
        List<SerialNumbersModel> SerialNumbersList = new List<SerialNumbersModel>();
        List<SerialNumbersModel> SerialNumbersForDetailList = new List<SerialNumbersModel>();
        List<ProductModel> ProductList = new List<ProductModel>();

        DetailsView SelectedDetail { get; set; }
        SerialNumbersModel SelectedSerialNumber { get; set; }
        ProductModel SelectedProduct { get; set; }
        public ViewModel ViewModel { get; set; }
        DetailsView productNameAndStatus { get; set; }

        public string SerialNumber, numpp, product, productnum;
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Odb.db = new System.Data.Entity.DbContext("Data Source=sql;initial catalog=dsl_sp;MultipleActiveResultSets=True;App=EntityFramework&quot;Integrated Security=SSPI;");

            ViewModel = new ViewModel();
            DataContext = ViewModel;

            LoadProducts();
        }

        #region Details
        #region Асинхронная загрузка деталей
        private async void UpdateGrid()
        {
            try
            {
                PBDetailsDG.Value = 0;
                SPPB.Background = new SolidColorBrush(Colors.Transparent);
                PBTBX.Text = "Добавление деталей...";
                DetailsDG.ItemsSource = new List<DetailsView>();
                DisableActions();
                Cursor = Cursors.AppStarting;
                await PrintDetails();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                EnableActions();
                Cursor = Cursors.Arrow;
                SPPB.Background = new SolidColorBrush(Colors.LightGreen);
                PBTBX.Text = "Детали добавлены!";
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
                if (details.Count <= 0)
                {
                    cmd = new SqlCommand("SELECT НомерД AS DetailNode, НазваниеД AS DetailName, Договор AS PP, ПрП AS PrP, (SELECT TOP(1) Data FROM dsl_sp.dbo.DEV_IMAGES_V as img WHERE НомерД = img.PRT$$$MNF) as Data FROM SerialNumber.[dbo].ARHDetailsView WHERE Договор='" + numpp + "' GROUP BY Договор, НомерД, НазваниеД, ПрП", connection);
                    reader = await cmd.ExecuteReaderAsync();
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
                }
                connection.Close();
            }
            /*#region progressBar
            int totalItems = details.Count;
            int processedItems = 0;
            foreach (DetailsView detail in details)
            {
                processedItems++;
                PBDetailsDG.Value = (double)processedItems / totalItems * 100;
                PBTBX.Text = $"Добавляем детали: {processedItems}/{totalItems}";
                await Task.Delay(1);
            }
            #endregion*/
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return details;
        }
        #endregion
        #endregion

        #region SN
        private void SerialNumbersLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSerialNumber = (SerialNumbersModel)SerialNumbersLB.SelectedItem;
            if (SelectedSerialNumber != null)
            {
                SerialNumberTBX.Text = SelectedSerialNumber.DEV_SN;
                SearchDetailsInDB();
            }
        }

        private void SerialNumberTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            SerialNumbersLB.Visibility = Visibility.Visible;
        }

        private void SerialNumberTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            SerialNumbersLB.Visibility = Visibility.Collapsed;
        }

        private void SerialNumberTBX_TextChanged(object sender, TextChangedEventArgs e)
        {

            string txt = SerialNumberTBX.Text;
            if (txt == null || txt == "")
            {
                SerialNumbersLB.ItemsSource = SerialNumbersList;
            }
            else
            {
                SerialNumbersLB.ItemsSource = SerialNumbersList.Where(u => u.FullSerialNumber.ToLower().Contains(txt.ToLower())).ToList();
            }
        }

        #region Асинхронная загрузка серийных номеров
        private async void LoadSerialNumbers()
        {
            try
            {
                SPPB.Background = new SolidColorBrush(Colors.Transparent);
                PBTBX.Text = "Добавление серийных номеров...";
                SerialNumberTBX.IsEnabled = false;
                ProductTBX.IsEnabled = false;
                SerialNumberTBX.Text = "Загрузка...";
                Cursor = Cursors.AppStarting;
                SerialNumbersLB.ItemsSource = new List<SerialNumbersModel>();
                await PrintSerialNumbers();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка[test]: " + ex.Message);
            }
            finally
            {
                //EnableActions();
                if (SerialNumbersList.Count > 0)
                {
                    PBTBX.Text = "Серийные номера добавлены!";
                    SPPB.Background = new SolidColorBrush(Colors.LightGreen);
                    ProductSP.Visibility = Visibility.Collapsed;
                    SNSP.Visibility = Visibility.Visible;
                }
                else
                {
                    PBTBX.Text = "Серийные номера не найдены!";
                    SPPB.Background = new SolidColorBrush(Colors.Red);
                    #region Анимация подсказки
                    Storyboard storyboard = (Storyboard)this.Resources["AppearDisappearStoryboard"];
                    if (storyboard != null)
                        storyboard.Begin(PDNotFoundTB);
                    #endregion
                    ProductTBX.Clear();
                }
                Cursor = Cursors.Arrow;
                SerialNumberTBX.IsEnabled = true;
                SerialNumberTBX.IsEnabled = true;
                SerialNumberTBX.Text = "";
            }
        }

        async Task PrintSerialNumbers()
        {
            Console.WriteLine("-----Загрузка серийных номеров началась-----");
            SerialNumbersList = await GetSerialNumbersAsync();
            Console.WriteLine("-----Загрузка серийных номеров закончена----");

            Dispatcher.Invoke(() => { SerialNumbersLB.ItemsSource = SerialNumbersList; });
        }

        async Task<List<SerialNumbersModel>> GetSerialNumbersAsync()
        {
            List<SerialNumbersModel> snsModel = new List<SerialNumbersModel>();


            using (SqlConnection connection = new SqlConnection(Odb.db.Database.Connection.ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select distinct DEV_SN, CC_ZN from SS_DEV_NUM as devnum left join SP_SS as spss on devnum.SS_ID = spss.SS_ID left join Cooperation.dbo.DetailsView as dv on spss.NUM_PP = dv.Договор collate Cyrillic_General_100_CI_AI LEFT JOIN CC_SUB_VIEW as subview on devnum.DEV_SN = subview.CC_SN where dv.Изделие = '" + product + "' and dv.НомерИ = '" + productnum + "' and DEV_SN is not null ORDER BY CC_ZN DESC", connection);
                cmd.CommandTimeout = 40000;
                var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    SerialNumbersModel snModel = new SerialNumbersModel()
                    {
                        DEV_SN = reader.GetString(0),
                        CC_ZN = reader.IsDBNull(1) ? "" : reader.GetString(1)
                    };
                    snsModel.Add(snModel);
                }
                connection.Close();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return snsModel;
        }
        #endregion
        #endregion

        #region Product
        private void ProductLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedProduct = (ProductModel)ProductLB.SelectedItem;
            if (SelectedProduct == null)
                return;
            PPNameTB.Text = SelectedProduct.ProductFull;

            product = SelectedProduct.Product;
            productnum = SelectedProduct.ProductNum;

            LoadSerialNumbers();
            
        }

        #region Асинхронная загрузка изделий
        private async void LoadProducts()
        {
            try
            {
                PBDetailsDG.Value = 0;
                PBTBX.Text = "Начало добавления изделий...";
                ProductLB.ItemsSource = new List<ProductModel>();
                DisableActions();
                await PrintProducts();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка[test]: " + ex.Message);
            }
            finally
            {
                EnableActions();
                SPPB.Background = new SolidColorBrush(Colors.LightGreen);
                PBTBX.Text = "Изделия добавлены!";
                
            }
        }

        async Task PrintProducts()
        {
            Console.WriteLine("-----Загрузка изделий началась-----");
            ProductTBX.Text = "Загрузка...";
            Cursor = Cursors.AppStarting;
            ProductTBX.IsEnabled = false;
            ProductList = await GetProductsAsync();
            Console.WriteLine("-----Загрузка изделий закончена----");
            Cursor = Cursors.Arrow;
            ProductTBX.Text = "";
            ProductTBX.IsEnabled = true;

            Dispatcher.Invoke(() => { ProductLB.ItemsSource = ProductList; });
        }

        async Task<List<ProductModel>> GetProductsAsync()
        {
            List<ProductModel> products = new List<ProductModel>();

            using (SqlConnection connection = new SqlConnection(Odb.db.Database.Connection.ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select distinct dv.Изделие as Product, dv.НомерИ as ProductNum from Cooperation.dbo.DetailsView as dv WHERE dv.Изделие is not null and dv.НомерИ is not null", connection);
                cmd.CommandTimeout = 40000;
                var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    ProductModel product = new ProductModel()
                    {
                        Product = reader.GetString(0),
                        ProductNum = reader.GetString(1)
                    };
                    products.Add(product);
                }
                connection.Close();
            }
            /*#region progressBar
            int totalItems = products.Count;
            int processedItems = 0;
            foreach (ProductModel product in products)
            {
                processedItems++;
                PBDetailsDG.Value = (double)processedItems / totalItems * 100;
                PBTBX.Text = $"Добавляем изделия: {processedItems}/{totalItems}";
                await Task.Delay(1);
            }
            #endregion
            */
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return products;
        }
        #endregion

        private void ProductTBX_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            searchProduct();
        }

        private void ProductTBX_GotFocus(object sender, RoutedEventArgs e)
        {
            ProductLB.Visibility = Visibility.Visible;
        }

        private void ProductTBX_LostFocus(object sender, RoutedEventArgs e)
        {
            ProductLB.Visibility = Visibility.Collapsed;
        }

        private void ProductTBX_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchProduct();
        }

        public void searchProduct()
        {
            List<ProductModel> pd = new List<ProductModel>();
            string txt = ProductTBX.Text;
            if (txt.Length == 0)
                pd = ProductList;
            pd = ProductList.Where(u => u.ProductFull.ToLower().Contains(txt.ToLower())).ToList();
            ProductLB.ItemsSource = pd;
        }

        private void BackToProductBtn_Click(object sender, RoutedEventArgs e)
        {
            ProductSP.Visibility = Visibility.Visible;
            SNSP.Visibility = Visibility.Collapsed;
            DetailsDG.ItemsSource = new List<DetailsView>();
        }
        #endregion

        #region Поиск в таблице
        private void DetailSearchBTN_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsDetailSearchVisible = !ViewModel.IsDetailSearchVisible;
        }

        private void searchInDG()
        {
            List<DetailsView> details = new List<DetailsView>();
            string txt = DetailSearchTBX.Text;
            if (txt.Length == 0)
                details = detailsView;
            details = detailsView.Where(u => u.DetailToString.ToLower().Contains(txt.ToLower())).ToList();
            DetailsDG.ItemsSource = details;
        }

        private void DetailSearchTBX_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchInDG();
        }
        #endregion

        #region Поиск деталей в БД
        private async void SearchDetailsInDB()
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
                string numppFromNumser = Odb.db.Database.SqlQuery<string>("SELECT DISTINCT SP_SS.NUM_PP FROM SP_SS LEFT JOIN SS_DEV_NUM as devnum on SP_SS.SS_ID = devnum.SS_ID LEFT JOIN CC_SUB_VIEW as subview on devnum.DEV_SN = subview.CC_SN WHERE subview.CC_ZN=@numser OR devnum.DEV_SN=@numser", new SqlParameter("numser", SerialNumberTBX.Text)).SingleOrDefault();

                if (!string.IsNullOrEmpty(numppFromNumser))
                {
                    PPTBX.Text = numppFromNumser;
                    SerialNumber = SerialNumberTBX.Text;

                    productNameAndStatus = Odb.db.Database.SqlQuery<DetailsView>("SELECT TOP (1) НомерИ AS ProductNum, Изделие AS ProductName, RSTS FROM [Cooperation].[dbo].[DetailsView] WHERE Договор=@numpp", new SqlParameter("numpp", numppFromNumser)).SingleOrDefault();
                    if (productNameAndStatus == null)
                    {
                        productNameAndStatus = Odb.db.Database.SqlQuery<DetailsView>("SELECT TOP (1) НомерИ AS ProductNum, Изделие AS ProductName, RSTS FROM SerialNumber.[dbo].ARHDetailsView WHERE Договор=@numpp", new SqlParameter("numpp", numppFromNumser)).SingleOrDefault();
                        if (productNameAndStatus != null)
                        {
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
                            MessageBox.Show("[Детали] Информация не найдена.");
                        }
                    }
                    else
                    {
                        PPNameTB.Text = productNameAndStatus.Product;
                        #region Цвет в зависимости от статуса
                        if (productNameAndStatus.RSTS == 4)
                        {
                            PPNameTB.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF56DA56"));
                        }
                        #endregion
                        UpdateGrid();
                    }
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

        private void ClearProductBtn_Click(object sender, RoutedEventArgs e)
        {
            ProductTBX.Clear();
        }

        private void ClearSNBtn_Click(object sender, RoutedEventArgs e)
        {
            SerialNumberTBX.Clear();
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

        private void SNListForProductBTN_Click(object sender, RoutedEventArgs e)
        {
            if (productNameAndStatus != null)
            {
                SerialNumbersForDetailList = Odb.db.Database.SqlQuery<SerialNumbersModel>(@"
                select distinct DEV_SN, CC_ZN from SS_DEV_NUM as devnum
                left join SP_SS as spss on devnum.SS_ID = spss.SS_ID
                left join Cooperation.dbo.DetailsView as dv on spss.NUM_PP = dv.Договор collate Cyrillic_General_100_CI_AI
                LEFT JOIN CC_SUB_VIEW as subview on devnum.DEV_SN = subview.CC_SN
                where dv.Изделие = @product and dv.НомерИ = @productnum and DEV_SN is not null
                order by CC_ZN desc
                ", new SqlParameter("product", productNameAndStatus.ProductName), new SqlParameter("productnum", productNameAndStatus.ProductNum)).ToList();
                SNForProductWindow snForDetailWindow = new SNForProductWindow(SerialNumbersForDetailList, productNameAndStatus);
                snForDetailWindow.Show();
            }
            else
            {
                MessageBox.Show("Изделие не выбрано");
            }
        }
        #endregion

        private void OPSPCatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            OPSPCatalogWindow oPSPCatalogWindow = new OPSPCatalogWindow();
            oPSPCatalogWindow.Show();
        }

        /*#region для вывода файла из бд
        byte[] fileByte = Odb.db.Database.SqlQuery<byte[]>("SELECT Data FROM dsl_sp.dbo.CC_SUB_VIEW WHERE CARD_ID = 291").FirstOrDefault();
        SaveByteArrayToFileWithBinaryWriter(fileByte, "C:\\Users\\it01\\Documents\\pic.jpg");

        public static void SaveByteArrayToFileWithBinaryWriter(byte[] data, string filePath)
        {
            var writer = new BinaryWriter(File.OpenWrite(filePath));
            writer.Write(data);
        }
        //либо
        public static void SaveByteArrayToFileWithStaticMethod(byte[] data, string filePath) => File.WriteAllBytes(filePath, data);*/
    }
}
