using ac.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ac
{
    public partial class OperationsWindow : Window
    {
        List<Operations> OperationsList = new List<Operations>();
        List<SmenZadanie> SZList = new List<SmenZadanie>();
        List<SpecProcesses> SPList = new List<SpecProcesses>();
        List<Materials> MTList = new List<Materials>();
        List<DetailsView> DetailsList = new List<DetailsView>();

        private DetailsView SelectedDetail { get; set; }
        private Operations SelectedOperation { get; set; }
        private DetailsView ProductName { get; set; }

        public string SerialNumber;
        int ss_id;

        public OperationsWindow(DetailsView selectedDetail, string serialNumber, DetailsView productName)
        {
            InitializeComponent();

            SelectedDetail = selectedDetail;
            SerialNumber = serialNumber;
            ProductName = productName;

            OperationsList = Odb.db.Database.SqlQuery<Operations>("select distinct toz.NOP as OperationNum, tho.NAME as OperationName, toz.WCR$$$WCR + ' -  ' + wcr.NMC$$$NAM as Executor from SPRUT.OKP.dbo.OKP_TOZ as toz LEFT JOIN SPRUT.OKP.dbo.OKP_THO as tho on toz.TOP$$$KTO = tho.IDN LEFT JOIN SPRUT.OKP.dbo.OKP_WCR as wcr on toz.WCR$$$WCR = wcr.WCR$$$IDN WHERE toz.PRT$$$MNF=@detnode and toz.PPPNUM=@numpp and toz.NUM=@prp", new SqlParameter("detnode", SelectedDetail.DetailNode), new SqlParameter("numpp", SelectedDetail.PP), new SqlParameter("prp", SelectedDetail.PrP)).ToList();
            OperationsDG.ItemsSource = OperationsList;

            this.Title = SelectedDetail.Detail;

            ss_id = Odb.db.Database.SqlQuery<int>("SELECT SS_DEV_NUM.SS_ID FROM SP_SS LEFT JOIN SS_DEV_NUM ON SP_SS.SS_ID = SS_DEV_NUM.SS_ID WHERE DEV_SN=@devsn", new SqlParameter("devsn", SerialNumber)).SingleOrDefault();
        }

        private void SpecProcessesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SPList.Count > 0)
            {
                SpecProcessInfoWindow specProcessInfoWindow = new SpecProcessInfoWindow(SelectedDetail, SerialNumber, SelectedOperation, SPList);
                specProcessInfoWindow.Show();
            }
        }

        private void MaterialsBTN_Click(object sender, RoutedEventArgs e)
        {
            SelectedOperation = (Operations)OperationsDG.SelectedItem;
            if (OperationsDG.SelectedItem is Operations selectedOperation)
            {
                MaterialsWindow materialsWindow = new MaterialsWindow(SelectedDetail, SelectedOperation);
                materialsWindow.ShowDialog();
            }
        }

        private void SmenZadaniaBTN_Click(object sender, RoutedEventArgs e)
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

        private void OperationsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedOperation = (Operations)OperationsDG.SelectedItem;
            ButtonsCheck();
        }

        private void ButtonsCheck()
        {
            #region список спец. процессов (SPList)
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
                  OP_TIME_EXP.T_START, 
                  OP_TIME_EXP.T_END,
                  CONVERT(NVARCHAR(45), OP_TIME_EXP.T_START, 104) AS T_START_Date,
                  CONVERT(NVARCHAR(45), OP_TIME_EXP.T_START, 108) AS T_START_Time,
                  CONVERT(NVARCHAR(45), OP_TIME_EXP.T_END, 104) AS T_END_Date,
                  CONVERT(NVARCHAR(45), OP_TIME_EXP.T_END, 108) AS T_END_Time,
                  OPS.OP_TYPE, 
                  OPS_TYPE.OP_TYPE_NAME, 
                  OP_CON_SET.OP_CON_SET_NAME
                FROM 
                  SP_SS
                  LEFT JOIN sprut.okp.dbo.okp_toz as toz on SP_SS.NUM_PAR = toz.NUM collate Cyrillic_General_100_CI_AI
                  LEFT JOIN sprut.okp.dbo.okp_tho as tho on toz.TOP$$$KTO = tho.IDN
                  LEFT JOIN SP_DES ON SP_SS.SP_ID = SP_DES.SP_ID 
                  LEFT JOIN OPS ON SP_DES.OP_ID = OPS.OP_ID
                  INNER JOIN SerialNumber.dbo.OPandSP as sp on tho.NAME = sp.Operation and OPS.OP_NAME = sp.SpecProcess collate Cyrillic_General_100_CI_AI
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
	                toz.NUM=@prp and tho.NAME = @operation
                ORDER BY 
                  SP_DES.OP_SEQ

                ", new SqlParameter("prp", SelectedDetail.PrP), new SqlParameter("operation", SelectedOperation.OperationName), new SqlParameter("nop", SelectedOperation.OperationNum)).ToList();
            #endregion
            #region список смен. заданий (SZList)
            SZList = Odb.db.Database.SqlQuery<SmenZadanie>("SELECT DISTINCT id_Tabel, DTE, Cost, FIO, NUM, Detail, OrderNum, OperationNum, Operation, Status AS StatusBool, Count, DEP$$$DEP AS DEP, WCR$$$WCR AS WCR, SHIFT, Product FROM [Zarplats].[dbo].[SmenZadView] WHERE Operation=@operationname AND OperationNum=@operationnum AND Product=@productname AND NUM=@prp", new SqlParameter("operationname", SelectedOperation.OperationName), new SqlParameter("operationnum", SelectedOperation.OperationNum), new SqlParameter("productname", ProductName.ProductWithSpace), new SqlParameter("@prp", SelectedDetail.PrP)).ToList();
            #endregion
            #region список материалов (MTList)
            MTList = Odb.db.Database.SqlQuery<Materials>(@"
                select distinct p.NMP$$$NAM as MatName, trn.EIZ_RASH as EIZ, w.NAM as WRH, trn.DOC
                from SPRUT.OKP.dbo.OKP_TRNDOC as doc
                left join SPRUT.OKP.dbo.OKP_TRN as trn on doc.DOC = trn.DOC
                left join SPRUT.OKP.dbo.OKP_OBJLINKS l on l.S_Type = 6 and l.S_ID = trn.TRN_ID
                left join SPRUT.OKP.dbo.OKP_SReserv r on l.M_Type = 200 and l.M_ID = r.ID
                left join SPRUT.OKP.dbo.OKP_POT p on r.ID = p.TRN_ID
                left join SPRUT.OKP.dbo.OKP_TOZ t on p.Rwc_toz = t.rwc
                LEFT JOIN SPRUT.OKP.dbo.OKP_THO as tho on t.TOP$$$KTO = tho.IDN
                LEFT JOIN SPRUT.OKP.dbo.OKP_WRH as w on doc.WRH = w.WRH_IDN
                LEFT JOIN Cooperation.dbo.DetailsView as dv on t.PPPNUM = dv.Договор
                WHERE t.PRT$$$MNF=@detnode and t.PPPNUM=@numpp and t.NUM=@prp and dv.НомерО=@nop and dv.Операция=@operation",
                new SqlParameter("detnode", SelectedDetail.DetailNode), new SqlParameter("numpp", SelectedDetail.PP), new SqlParameter("prp", SelectedDetail.PrP), new SqlParameter("nop", SelectedOperation.OperationNum), new SqlParameter("operation", SelectedOperation.OperationName))
                .ToList();
            #endregion

            #region проверка на пустые списки
            if (SZList.Count > 0)
                SmenZadaniaBTN.IsEnabled = true;
            else
                SmenZadaniaBTN.IsEnabled = false;

            if (SPList.Count > 0)
                SpecProcessesBtn.IsEnabled = true;
            else
                SpecProcessesBtn.IsEnabled = false;

            if (MTList.Count > 0)
                MaterialsBtn.IsEnabled = true;
            else
                MaterialsBtn.IsEnabled = false;
            #endregion

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void OPSPCatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            OPSPCatalogWindow oPSPCatalogWindow = new OPSPCatalogWindow();
            oPSPCatalogWindow.Show();
        }
    }
}
