using ac.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ac
{
    public partial class MaterialsWindow : Window
    {
        List<Materials> MaterialsList = new List<Materials>();
        List<MaterialsTRNModel> trns = new List<MaterialsTRNModel>();
        List<MaterialsTRNModel> snList = new List<MaterialsTRNModel>();

        private DetailsView SelectedDetail { get; set; }
        private Materials SelectedMaterial { get; set; }
        private Operations SelectedOperation { get; set; }

        string prp;

        public MaterialsWindow(DetailsView selectedDetail, Operations selectedOperation, List<Materials> materialsList)
        {
            InitializeComponent();

            SelectedDetail = selectedDetail;
            SelectedOperation = selectedOperation;
            MaterialsList = materialsList;

            this.Title = SelectedDetail.Detail + " | Материалы";

            MaterialsDG.ItemsSource = MaterialsList;
            prp = SelectedDetail.PrP;
        }

        private void MaterialsDG_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedMaterial = (Materials)MaterialsDG.SelectedItem;
            if (MaterialsDG.SelectedItem is Materials selectedMaterial)
            {
                snList = Odb.db.Database.SqlQuery<MaterialsTRNModel>(@"
                    select distinct devnum.DEV_SN as SN, dv.Изделие as Product, dv.Договор as PP
                    from sprut.okp.dbo.okp_ppp as ppp
                    INNER join Cooperation.dbo.DetailsView as dv on ppp.PPPNUM = dv.Договор
                    INNER join dsl_sp.dbo.SP_SS as spss on spss.PROD_PP = dv.Договор collate Cyrillic_General_100_CI_AI
                    INNER join dsl_sp.dbo.SS_DEV_NUM as devnum on spss.SS_ID = devnum.SS_ID
                    INNER join sprut.okp.dbo.okp_toz as toz on dv.НомерД = toz.PRT$$$MNF
                    INNER join sprut.okp.dbo.okp_pot as pot on toz.rwc = pot.Rwc_toz
                    where pot.rwc=@potrwc and DEV_SN is not null
                ", new SqlParameter("potrwc", SelectedMaterial.rwc), new SqlParameter("doc", SelectedMaterial.DOC)).ToList();

                trns = Odb.db.Database.SqlQuery<MaterialsTRNModel>(
                    @"
                    select distinct p.NMP$$$NAM as Material, p.PRTIDN as PRTIDN, w.NAM as WRH, EIZ_RASH as EIZ, IDN, doc.DTA as DTAPostav, doc.DOC
                    from SPRUT.OKP.dbo.OKP_TRNDOC as doc
                    left join SPRUT.OKP.dbo.OKP_TRN as trn on doc.DOC = trn.DOC
                    left join SPRUT.OKP.dbo.OKP_OBJLINKS l on l.S_Type = 6 and l.S_ID = trn.TRN_ID
                    left join SPRUT.OKP.dbo.OKP_SReserv r on l.M_Type = 200 and l.M_ID = r.ID
                    left join SPRUT.OKP.dbo.OKP_POT p on r.ID = p.TRN_ID
                    INNER JOIN SPRUT.OKP.dbo.OKP_WRH as w on trn.WRH_IDN = w.WRH_IDN
                    INNER JOIN dsl_sp.dbo.SP_SS as spss on p.PPPNUM = spss.NUM_PP collate Cyrillic_General_CI_AS
                    INNER JOIN dsl_sp.dbo.SS_DEV_NUM as devnum on spss.SS_ID = devnum.SS_ID
                    INNER JOIN SPRUT.OKP.dbo.OKP_WRH_SUBJECTS as sbj on trn.SUB_ID = sbj.ID
                    INNER JOIN SPRUT.OKP.dbo.OKP_UKIM as ukim on sbj.S_ID = ukim.ID
                    where doc.DOC = @doc and p.rwc = @potrwc AND DEV_SN IS NOT NULL
                ",
                new SqlParameter("potrwc", SelectedMaterial.rwc), new SqlParameter("doc", SelectedMaterial.DOC)).ToList();
                //MessageBox.Show($"RWC: {SelectedMaterial.rwc}, DOC: {SelectedMaterial.DOC}");

                if (trns.Count > 0)
                {
                    MaterialsTRN materialsTRN = new MaterialsTRN(trns, snList);
                    materialsTRN.Show();
                }
                else
                {
                    Console.WriteLine($"[Поставка]: Список поставок материала [{SelectedMaterial.MatName}] ({SelectedMaterial.DOC}, {SelectedMaterial.rwc}) пуст.");
                }
            }
        }
    }
}
