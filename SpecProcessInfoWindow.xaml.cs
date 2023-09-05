using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ac
{
    /// <summary>
    /// Логика взаимодействия для SpecProcessInfoWindow.xaml
    /// </summary>
    public partial class SpecProcessInfoWindow : Window
    {
        List<SpecProcesses> specProcesses = new List<SpecProcesses>();
        private DetailsView SelectedDetail { get; set; }
        private int ss_id;

        public string SerialNumber;

        public SpecProcessInfoWindow(DetailsView selectedDetail, string serialNumber)
        {
            InitializeComponent();
            SelectedDetail = selectedDetail;
            SerialNumber = serialNumber;

            ss_id = Odb.db.Database.SqlQuery<int>("SELECT SS_DEV_NUM.SS_ID FROM SP_SS LEFT JOIN SS_DEV_NUM ON SP_SS.SS_ID = SS_DEV_NUM.SS_ID WHERE DEV_SN=@devsn", new SqlParameter("devsn", SerialNumber)).SingleOrDefault();

            MessageBox.Show(SelectedDetail.PrP);

            specProcesses = Odb.db.Database.SqlQuery<SpecProcesses>(
                @"

                SELECT DISTINCT
                  SP_DES.OP_SEQ, 
                  OPS.OP_NAME, 
                  OPS.OP_DESCR, 
                  OPS.OP_EL_ID, 
                  OP_EL.EL_TMP_VALUE1 as MinTemp,
                  OP_EL.EL_TMP_VALUE2 as MaxTemp,
                  OP_ENV_RES.EL_TMP as CurTemp,
                  OP_ENV_RES.EL_TMP_RESULT,
                  OP_EL.EL_TMP_TYPE as TempType, 
                  OP_ENV_RES.EL_HUM as CurHum, 
                  OP_ENV_RES.EL_HUM_RESULT, 
                  OP_TIME.TL_TYPE, 
                  OP_TIME.TL_VALUE1, 
                  OP_TIME.TL_VALUE2, 
                  OP_TIME_RES.TL_VALUE, 
                  OP_TIME_RES.TL_RESULT, 
                  OP_CON_RES.CON_RES, 
                  OP_CON.CON_NAME, 
                  OP_CON.CON_DESCR, 
                  USERS_DATA.USER_SFIO,
                  USERS_DATA.USER_FNAME, USERS_DATA.USER_LNAME, USERS_DATA.USER_MNAME,
                  OP_TIME_EXP.T_START, 
                  OP_TIME_EXP.T_END, 
                  OPS.OP_TYPE, 
                  OPS_TYPE.OP_TYPE_NAME, 
                  OP_CON_SET.OP_CON_SET_NAME 
                FROM 
                  SP_SS 
                  LEFT JOIN SP_DES ON SP_SS.SP_ID = SP_DES.SP_ID 
                  LEFT JOIN OPS ON SP_DES.OP_ID = OPS.OP_ID 
                  LEFT JOIN OP_EL ON OPS.OP_EL_ID = OP_EL.EL_ID 
                  LEFT JOIN OP_ENV_RES ON SP_SS.SS_ID = OP_ENV_RES.SS_ID 
                  AND SP_DES.OP_SEQ = OP_ENV_RES.OP_SEQ 
                  LEFT JOIN OP_TIME ON OPS.OP_TIME_ID = OP_TIME.TL_ID 
                  LEFT JOIN OP_TIME_RES ON SP_SS.SS_ID = OP_TIME_RES.SS_ID 
                  AND SP_DES.OP_SEQ = OP_TIME_RES.OP_SEQ 
                  LEFT JOIN OP_CON_RES ON SP_SS.SS_ID = OP_CON_RES.SS_ID 
                  AND SP_DES.OP_SEQ = OP_CON_RES.OP_SEQ 
                  LEFT JOIN OP_CON ON OP_CON_RES.CON_RES = OP_CON.CON_ID 
                  LEFT JOIN OP_USER_RES ON SP_SS.SS_ID = OP_USER_RES.SS_ID 
                  AND SP_DES.OP_SEQ = OP_USER_RES.OP_SEQ 
                  LEFT JOIN USERS_DATA ON OP_USER_RES.USER_ID = USERS_DATA.USER_PERCO_ID 
                  LEFT JOIN OP_TIME_EXP ON SP_SS.SS_ID = OP_TIME_EXP.SS_ID 
                  AND SP_DES.OP_SEQ = OP_TIME_EXP.OP_SEQ 
                  LEFT JOIN OPS_TYPE ON OPS.OP_TYPE = OPS_TYPE.OP_TYPE_ID 
                  LEFT JOIN OP_CON_SET ON OP_CON_RES.OP_CON_SET_ID = OP_CON_SET.OP_CON_SET_ID
                WHERE 
                  SP_SS.SS_ID = @ssid AND DEV_NAME = @devname
                ORDER BY 
                  SP_DES.OP_SEQ DESC

                ", new SqlParameter("ssid", ss_id), new SqlParameter("prp", SelectedDetail.PrP), new SqlParameter("devname", SelectedDetail.DetailNode)).ToList();
            SpecProcessesItemsControl.ItemsSource = specProcesses;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                SpecProcesses data = border.DataContext as SpecProcesses;
                if (data != null)
                {
                    MessageBox.Show($"SelectedDetail PrP: {SelectedDetail.PrP}");
                }
            }
        }

        private void search()
        {
            List<SpecProcesses> sp = new List<SpecProcesses>();
            string txt = SearchSPTBX.Text;
            if (txt.Length == 0)
                sp = specProcesses;
            sp = specProcesses.Where(u => u.OP_NAME.ToLower().Contains(txt.ToLower())).ToList();
            SpecProcessesItemsControl.ItemsSource = sp;
        }

        private void SearchSPTBX_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                search();
            }
        }
    }
}
