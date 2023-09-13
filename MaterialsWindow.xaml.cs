using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace ac
{
    public partial class MaterialsWindow : Window
    {
        List<Materials> materialsList = new List<Materials>();

        private DetailsView SelectedDetail { get; set; }
        private Materials SelectedMaterial { get; set; }

        public MaterialsWindow(DetailsView selectedDetail)
        {
            InitializeComponent();

            SelectedDetail = selectedDetail;
            this.Title = SelectedDetail.Detail + " | Материалы";

            materialsList = Odb.db.Database.SqlQuery<Materials>(@"

                select distinct
                    pot.CPLNUM as PrP,
                    pot.PRTIDN as PrintIDN,
                    pot.NMP$$$NAM as MatName,
                    eiz.NAENAM as EIZ,
                    QTY as AmountOnWRHs,
                    QTYMFC as RezervOnWRHs,
                    case
	                    when (inv.QTYMFC-inv.QTY) < 0 THEN 0
	                    else (inv.QTYMFC-inv.QTY)
                    end as DeficitOnWRHs,
                    pot.QTYPOT as PlanPotreb,
                    pot.QTYTQY as CurrentPotreb,
                    QTYRQY as Norma,
                    NAM as WRH
                from SPRUT.OKP.dbo.OKP_POT as pot
                INNER JOIN SPRUT.OKP.dbo.OKP_INV as inv on pot.PRTIDN = inv.PRTIDN
                INNER JOIN SPRUT.OKP.dbo.OKP_NOM as nom on pot.PRTIDN = nom.PRT$$$IDN
                INNER JOIN SPRUT.OKP.dbo.OKP_WRH as wrh on nom.WRH_IDN = wrh.WRH_IDN
                INNER JOIN SPRUT.OKP.dbo.OKP_EIZ as eiz on inv.UOMPEIZ = eiz.UOMIDN
                where CPLNUM=@prp and (QTYMFC-QTY) <> 0 and inv.QTY != 0",

                new SqlParameter("prp", SelectedDetail.PrP))
                .ToList();
            MaterialsDG.ItemsSource = materialsList;
        }

        private void MaterialsDG_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedMaterial = (Materials)MaterialsDG.SelectedItem;
            if (MaterialsDG.SelectedItem is Materials selectedMaterial)
            {
                MaterialInfoWindow materialsInfoWindow = new MaterialInfoWindow(SelectedMaterial, SelectedDetail);
                materialsInfoWindow.ShowDialog();
            }
        }
    }
}
