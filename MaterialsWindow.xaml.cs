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

                SELECT
                    pot.PRTIDN as PrintIDN,
                    pot.NMP$$$NAM as MatName,
                    eiz.NAENAM as EIZ,
                    SUM(QTY) as AmountOnWRHs,
                    SUM(QTYMFC) as RezervOnWRHs,
                    CASE
                        WHEN (SUM(QTYMFC) - SUM(QTY)) < 0 THEN 0
                        ELSE (SUM(QTYMFC) - SUM(QTY))
                    END as DeficitOnWRHs,
                    pot.QTYPOT as PlanPotreb,
                    pot.QTYTQY as CurrentPotreb,
                    QTYRQY as Norma,
                    NAM as WRH
                FROM SPRUT.OKP.dbo.OKP_POT as pot
                INNER JOIN SPRUT.OKP.dbo.OKP_INV as inv on pot.PRTIDN = inv.PRTIDN
                INNER JOIN SPRUT.OKP.dbo.OKP_NOM as nom on pot.PRTIDN = nom.PRT$$$IDN
                INNER JOIN SPRUT.OKP.dbo.OKP_WRH as wrh on nom.WRH_IDN = wrh.WRH_IDN
                INNER JOIN SPRUT.OKP.dbo.OKP_EIZ as eiz on inv.UOMPEIZ = eiz.UOMIDN
                WHERE CPLNUM=@prp
                GROUP BY pot.PRTIDN, pot.NMP$$$NAM, eiz.NAENAM, pot.QTYPOT, pot.QTYTQY, QTYRQY, NAM;",

                new SqlParameter("prp", SelectedDetail.PrP))
                .ToList();
            MaterialsDG.ItemsSource = materialsList;
        }
    }
}
