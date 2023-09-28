using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ac
{
    /// <summary>
    /// Логика взаимодействия для SpecProcessInfoWindow.xaml
    /// </summary>
    public partial class SpecProcessInfoWindow : Window
    {
        List<SpecProcesses> SpecProcessesList = new List<SpecProcesses>();

        private DetailsView SelectedDetail { get; set; }
        private Operations SelectedOperation { get; set; }

        //private int ss_id;
        public string SerialNumber;

        public SpecProcessInfoWindow(DetailsView selectedDetail, string serialNumber, Operations selectedOperation, List<SpecProcesses> specProcessesList)
        {
            InitializeComponent();
            SelectedDetail = selectedDetail;
            SerialNumber = serialNumber;
            SelectedOperation = selectedOperation;
            SpecProcessesList = specProcessesList;

            this.Title = SelectedDetail.Detail + " | Спец. процесс";

            //ss_id = Odb.db.Database.SqlQuery<int>("SELECT SS_DEV_NUM.SS_ID FROM SP_SS LEFT JOIN SS_DEV_NUM ON SP_SS.SS_ID = SS_DEV_NUM.SS_ID WHERE DEV_SN=@devsn", new SqlParameter("devsn", SerialNumber)).SingleOrDefault();

            for (int i = 0; i < SpecProcessesList.Count; i++)
            {
                if (SpecProcessesList[i].T_START_Date == SpecProcessesList[i].T_END_Date)
                {
                    SpecProcessesList[i].T_END_Date = null;
                }
            }
            SpecProcessesItemsControl.ItemsSource = SpecProcessesList;

            #region Цвет для поля в зависимости от результата другого поля, учитывая DataTemplate
            for (int i = 0; i < SpecProcessesList.Count; i++)
            {
                if (SpecProcessesList[i].EL_TMP_RESULT.HasValue)
                {
                    bool tmpResultValue = SpecProcessesList[i].EL_TMP_RESULT.Value;
                    SpecProcessesList[i].EL_TMP_RESULT_COLOR = tmpResultValue ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF56DA56")) : Brushes.Red;
                }

                if (SpecProcessesList[i].EL_HUM_RESULT.HasValue)
                {
                    bool tmpResultValue = SpecProcessesList[i].EL_HUM_RESULT.Value;
                    SpecProcessesList[i].EL_HUM_RESULT_COLOR = tmpResultValue ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF56DA56")) : Brushes.Red;
                }

                if (SpecProcessesList[i].TL_RESULT != null)
                {
                    if (SpecProcessesList[i].TL_RESULT == "1")
                    {
                        Brush lightGreen = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF56DA56"));
                        SpecProcessesList[i].TL_VALUE_COLOR = lightGreen;
                    }
                    else
                    {
                        SpecProcessesList[i].TL_VALUE_COLOR = Brushes.Red;
                    }
                }
            }
            #endregion
        }

        #region Поиск
        private void search()
        {
            List<SpecProcesses> sp = new List<SpecProcesses>();
            string txt = SearchSPTBX.Text;
            if (txt.Length == 0)
                sp = SpecProcessesList;
            sp = SpecProcessesList.Where(u => u.OP_NAME.ToLower().Contains(txt.ToLower())).ToList();
            SpecProcessesItemsControl.ItemsSource = sp;
        }

        private void SearchSPTBX_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                search();
            }
        }

        private void SearchSPBtn_Click(object sender, RoutedEventArgs e)
        {
            search();
        }
        #endregion
    }
}
