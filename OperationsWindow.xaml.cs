using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ac
{
    public partial class OperationsWindow : Window
    {
        List<Operations> OperationsList = new List<Operations>();
        List<SmenZadanie> SZList = new List<SmenZadanie>();
        List<SpecProcesses> SPList = new List<SpecProcesses>();

        private DetailsView SelectedDetail { get; set; }
        private Operations SelectedOperation { get; set; }
        private DetailsView ProductName { get; set; }

        public string SerialNumber;

        public OperationsWindow(DetailsView selectedDetail, string serialNumber, DetailsView productName)
        {
            InitializeComponent();
            
            SelectedDetail = selectedDetail;
            SerialNumber = serialNumber;
            ProductName = productName;

            OperationsList = Odb.db.Database.SqlQuery<Operations>("SELECT Договор AS PP, НомерД AS DetailNode, Операция AS OperationName, НомерО AS OperationNum, Исполнитель AS Executor, Цена AS Price, Стоимость AS Cost FROM Cooperation.dbo.DetailsView WHERE НомерД=@detnode AND Договор=@numpp AND ПрП=@prp GROUP BY Договор, Операция, НомерО, Исполнитель, Цена, Стоимость, Количество, Статус, НомерД", new SqlParameter("detnode", SelectedDetail.DetailNode), new SqlParameter("numpp", SelectedDetail.PP), new SqlParameter("prp", SelectedDetail.PrP)).ToList();
            OperationsDG.ItemsSource = OperationsList;

            this.Title = SelectedDetail.Detail;


            int ss_id = Odb.db.Database.SqlQuery<int>("SELECT SS_DEV_NUM.SS_ID FROM SP_SS LEFT JOIN SS_DEV_NUM ON SP_SS.SS_ID = SS_DEV_NUM.SS_ID WHERE DEV_SN=@devsn", new SqlParameter("devsn", SerialNumber)).SingleOrDefault();
            SPList = Odb.db.Database.SqlQuery<SpecProcesses>(
                @"

                SELECT DISTINCT
                  SP_SS.NUM_NOM,
                  SP_DES.OP_SEQ, 
                  OPS.OP_NAME, 
                  OPS.OP_DESCR, 
                  OP_EL.EL_NAME, 
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
                  SP_SS.SS_ID = @ssid
                ORDER BY 
                  SP_DES.OP_SEQ DESC

                ", new SqlParameter("ssid", ss_id), new SqlParameter("prp", SelectedDetail.PrP)).ToList();
            if (SPList.Count <= 0)
                SpecProcessesBtn.IsEnabled = false;
        }

        #region ImgCB
        private void ImgCB_Checked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility = Visibility.Visible;
        }

        private void ImgCB_Unchecked(object sender, RoutedEventArgs e)
        {
            ImageColumn.Visibility = Visibility.Collapsed;
        }
        #endregion

        private void SpecProcessesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SPList.Count > 0)
            {
                SpecProcessInfoWindow specProcessInfoWindow = new SpecProcessInfoWindow(SelectedDetail, SerialNumber);
                specProcessInfoWindow.Show();
            }
        }

        private void MaterialsBtn_Click(object sender, RoutedEventArgs e)
        {
            MaterialsWindow materialsWindow = new MaterialsWindow(SelectedDetail);
            materialsWindow.ShowDialog();
        }

        private void OperationsDG_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedOperation = (Operations)OperationsDG.SelectedItem;
            if (OperationsDG.SelectedItem is Operations selectedOperation)
            {
                SZList = Odb.db.Database.SqlQuery<SmenZadanie>("SELECT DISTINCT id_Tabel, DTE, Cost, FIO, NUM, Detail, OrderNum, OperationNum, Operation, Status AS StatusBool, Count, DEP$$$DEP AS DEP, WCR$$$WCR AS WCR, SHIFT, Product FROM [Zarplats].[dbo].[SmenZadView] WHERE Operation=@operationname AND OperationNum=@operationnum AND Product=@productname AND NUM=@prp", new SqlParameter("operationname", SelectedOperation.OperationName), new SqlParameter("operationnum", SelectedOperation.OperationNum), new SqlParameter("productname", ProductName.ProductWithSpace), new SqlParameter("@prp", SelectedDetail.PrP)).ToList();
                if (SZList.Count > 0)
                {
                    SmenZadanieWindow smenZadanieWindow = new SmenZadanieWindow(SelectedOperation, ProductName, SelectedDetail);
                    smenZadanieWindow.Show();
                }
            }
        }
    }
}
