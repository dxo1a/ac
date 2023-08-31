using ac.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;

namespace ac
{
    /// <summary>
    /// Логика взаимодействия для DetailInfo.xaml
    /// </summary>
    public partial class DetailInfoWindow
    {
        private DetailsView SelectedDetail { get; set; }
        DetailsView Detail { get; set; }
        List<DetailsView> Details = new List<DetailsView>();

        public DetailInfoWindow(DetailsView selectedDetail)
        {
            InitializeComponent();
            SelectedDetail = selectedDetail;
            this.Title = SelectedDetail.Detail;
            Odb.db = new System.Data.Entity.DbContext("Data Source=sql;initial catalog=dsl_sp;MultipleActiveResultSets=True;App=EntityFramework&quot;Integrated Security=SSPI;");

            OperationTB.Text = SelectedDetail.OperationName + " (" + SelectedDetail.OperationNum + ")";
            ExecutorTB.Text = SelectedDetail.Executor;

            double price = Convert.ToDouble(SelectedDetail.Price);
            price = Math.Round(price, 3);
            PriceTB.Text = price.ToString();

            double cost = Convert.ToDouble(SelectedDetail.Cost);
            cost = Math.Round(cost, 3);
            CostTB.Text = cost.ToString();

            CountTB.Text = SelectedDetail.Count.ToString();

            StatusTB.Text = StatusConverter.ConvertThis(SelectedDetail.Status);
            if (SelectedDetail.Status == 4)
            {
                StatusTB.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF56DA56"));
            }

            SetImage();
        }

        private void SetImage()
        {
            string detailnode = SelectedDetail.DetailNode;
            byte[] imageData = Odb.db.Database.SqlQuery<byte[]>("select Data from dsl_sp.dbo.DEV_IMAGES_V where PRT$$$MNF = @param1", new SqlParameter("@param1", detailnode)).FirstOrDefault();
            BitmapImage image = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(imageData))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // Загрузка изображения в память
                image.StreamSource = memoryStream;
                image.EndInit();
            }
            Img.Source = image;
        }
    }
}
