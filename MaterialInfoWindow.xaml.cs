using ac.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ac
{
    public partial class MaterialInfoWindow : Window
    {
        List<Materials> devsnList = new List<Materials>();
        List<Materials> materialsNamesList = new List<Materials>();

        private Materials SelectedMaterial { get; set; }
        private DetailsView SelectedDetail { get; set; }

        public MaterialInfoWindow(Materials selectedMaterial, DetailsView selectedDetail)
        {
            InitializeComponent();

            SelectedMaterial = selectedMaterial;
            SelectedDetail = selectedDetail;

            devsnList = Odb.db.Database.SqlQuery<Materials>(@"

                select distinct
                    DEV_SN
                    from  SPRUT.OKP.dbo.OKP_POT as pot
                    INNER JOIN SPRUT.OKP.dbo.OKP_INV as inv on pot.PRTIDN = inv.PRTIDN
                    INNER JOIN SPRUT.OKP.dbo.OKP_NOM as nom on pot.PRTIDN = nom.PRT$$$IDN
                    INNER JOIN SPRUT.OKP.dbo.OKP_WRH as wrh on nom.WRH_IDN = wrh.WRH_IDN
                    INNER JOIN SPRUT.OKP.dbo.OKP_EIZ as eiz on inv.UOMPEIZ = eiz.UOMIDN
                    INNER JOIN dsl_sp.dbo.SP_SS as spss on pot.CPLNUM = spss.NUM_PAR collate Cyrillic_General_CI_AS
                    INNER JOIN dsl_sp.dbo.SS_DEV_NUM as devnum on spss.SS_ID = devnum.SS_ID
                    where pot.CPLNUM=@prp AND pot.PRTIDN=@prtidn AND (QTYMFC-QTY) <> 0 AND inv.QTY != '0'",

                new SqlParameter("prp", SelectedMaterial.PrP), new SqlParameter("prtidn", SelectedMaterial.PrintIDN))
                .ToList();

            MtrlsNames();
        }

        private void MtrlsNames()
        {
            materialsNamesList = Odb.db.Database.SqlQuery<Materials>(@"
                select distinct
                    pot.NMP$$$NAM as MatName
                    from  SPRUT.OKP.dbo.OKP_POT as pot
                    INNER JOIN SPRUT.OKP.dbo.OKP_INV as inv on pot.PRTIDN = inv.PRTIDN
                    INNER JOIN SPRUT.OKP.dbo.OKP_NOM as nom on pot.PRTIDN = nom.PRT$$$IDN
                    INNER JOIN SPRUT.OKP.dbo.OKP_WRH as wrh on nom.WRH_IDN = wrh.WRH_IDN
                    INNER JOIN SPRUT.OKP.dbo.OKP_EIZ as eiz on inv.UOMPEIZ = eiz.UOMIDN
                    INNER JOIN dsl_sp.dbo.SP_SS as spss on pot.CPLNUM = spss.NUM_PAR collate Cyrillic_General_CI_AS
                    INNER JOIN dsl_sp.dbo.SS_DEV_NUM as devnum on spss.SS_ID = devnum.SS_ID
                    where pot.CPLNUM=@prp AND (QTYMFC-QTY) <> 0 AND inv.QTY != 0",
                new SqlParameter("prp", SelectedDetail.PrP)
            ).ToList();

            MaterialsItemsControl.ItemsSource = materialsNamesList;
        }

        private void MaterialED_Loaded(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;
            expander.Header = SelectedMaterial.MatName;

            DataGrid dataGrid = FindDataGrid(expander);
            if (dataGrid != null)
                dataGrid.ItemsSource = devsnList;
        }

        private DataGrid FindDataGrid(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is DataGrid)
                    return child as DataGrid;

                DataGrid dataGrid = FindDataGrid(child);

                if (dataGrid != null)
                    return dataGrid;
            }
            return null;
        }
    }
}
