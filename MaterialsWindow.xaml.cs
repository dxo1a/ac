using ac.Models;
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

        private DetailsView SelectedDetail { get; set; }
        private Materials SelectedMaterial { get; set; }

        string prp;

        public MaterialsWindow(DetailsView selectedDetail, List<Materials> materialsList)
        {
            InitializeComponent();

            SelectedDetail = selectedDetail;
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
                trns = Odb.db.Database.SqlQuery<MaterialsTRNModel>(
                    @"
                    SELECT DISTINCT
                     toz.PRT$$$MNF as Detail,
                     pot.PRTIDN as PRTIDN,
                     pot.NMP$$$NAM as Material,
                     sbj.QTY,
                     EIZ_RASH as EIZ,
                     wrh.NAM as WRH,
                     DEV_SN as SN,
                     IDN,
                     trndoc.DTA
                    FROM SPRUT.OKP.dbo.okp_toz as toz
                    INNER JOIN SPRUT.OKP.dbo.okp_pot as pot on toz.rwc = pot.Rwc_toz
                    INNER JOIN SPRUT.OKP.dbo.okp_uop as uop on toz.rwc = uop.Rwc_toz
                    INNER JOIN dsl_sp.dbo.SP_SS as spss on pot.PPPNUM = spss.NUM_PP collate Cyrillic_General_CI_AS
                    INNER JOIN dsl_sp.dbo.SS_DEV_NUM as devnum on spss.SS_ID = devnum.SS_ID
                    INNER JOIN SPRUT.OKP.dbo.OKP_TRN as trn on pot.TRN_ID = trn.TRN_ID
                    INNER JOIN SPRUT.OKP.dbo.OKP_TRNDOC as trndoc on trn.DOC_ID = trndoc.ID
                    INNER JOIN SPRUT.OKP.dbo.OKP_WRH as wrh on trn.WRH_IDN = wrh.WRH_IDN
                    INNER JOIN SPRUT.OKP.dbo.OKP_WRH_SUBJECTS as sbj on trn.SUB_ID = sbj.ID
                    INNER JOIN SPRUT.OKP.dbo.OKP_UKIM as ukim on sbj.S_ID = ukim.ID
                    LEFT JOIN SPRUT.OKP.dbo.OKP_UOPKIMNUMS as ukn on ukn.UOP_ID = uop.ID
                    WHERE pot.rwc=@potrwc AND trn.DOC=@doc AND DEV_SN IS NOT NULL
                ",
                new SqlParameter("potrwc", SelectedMaterial.rwc), new SqlParameter("doc", SelectedMaterial.DOC)).ToList();

                if (trns.Count > 0)
                {
                    MaterialsTRN materialsTRN = new MaterialsTRN(SelectedMaterial, prp, trns);
                    materialsTRN.Show();
                }
                
            }
        }
    }
}
