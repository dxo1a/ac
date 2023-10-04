using ac.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ac
{
    public partial class SNForProductWindow : Window
    {
        List<SerialNumbersModel> SNListForDetail = new List<SerialNumbersModel>();
        DetailsView ProductNameAndStatus { get; set; }

        public SNForProductWindow(List<SerialNumbersModel> snListForDetail, DetailsView productNameAndStatus)
        {
            InitializeComponent();
            
            SNListForDetail = snListForDetail;
            SNListForDetailDG.ItemsSource = SNListForDetail;
            ProductNameAndStatus = productNameAndStatus;
            this.Title = $"Серийные номера ({ProductNameAndStatus.Product})";
        }

        private void SearchTBX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                search();
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            search();
        }

        private void search()
        {
            List<SerialNumbersModel> sn = new List<SerialNumbersModel>();
            string txt = SearchTBX.Text;
            if (txt.Length == 0)
                sn = SNListForDetail;
            sn = SNListForDetail.Where(u => u.FullSerialNumber.ToLower().Contains(txt.ToLower())).ToList();
            SNListForDetailDG.ItemsSource = sn;
        }

        private void SearchTBX_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            search();
        }

        /*
        #region Асинхронная загрузка
        private async void UpdateGrid()
        {
            try
            {
                SNListForDetailDG.ItemsSource = new List<SerialNumbersModel>();
                await PrintDetails();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

        async Task PrintDetails()
        {
            Console.WriteLine("-----Загрузка деталей началась-----");
            SNListForDetail = await GetDetailsAsync();
            Console.WriteLine("-----Загрузка деталей закончена----");

            Dispatcher.Invoke(() => { SNListForDetailDG.ItemsSource = SNListForDetail; });
        }

        async Task<List<SerialNumbersModel>> GetDetailsAsync()
        {
            List<SerialNumbersModel> serialnumbers = new List<SerialNumbersModel>();
            string product = ProductNameAndStatus.ProductName;
            string productnum = ProductNameAndStatus.ProductNum;

            using (SqlConnection connection = new SqlConnection(Odb.db.Database.Connection.ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select distinct DEV_SN, CC_ZN from SS_DEV_NUM as devnum left join SP_SS as spss on devnum.SS_ID = spss.SS_ID left join Cooperation.dbo.DetailsView as dv on spss.NUM_PP = dv.Договор collate Cyrillic_General_100_CI_AI LEFT JOIN CC_SUB_VIEW as subview on devnum.DEV_SN = subview.CC_SN where dv.Изделие = '" + product + "' and dv.НомерИ = '" + productnum + "' and DEV_SN is not null order by CC_ZN desc", connection);
                var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    SerialNumbersModel serialnumber = new SerialNumbersModel()
                    {
                        DEV_SN = reader.GetString(0),
                        CC_ZN = reader.GetString(1)
                    };
                    serialnumbers.Add(serialnumber);
                }
                connection.Close();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return serialnumbers;
        }
        #endregion
        */
    }
}
