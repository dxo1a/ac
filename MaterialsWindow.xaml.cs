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

        public MaterialsWindow(DetailsView selectedDetail)
        {
            InitializeComponent();

            SelectedDetail = selectedDetail;

            materialsList = Odb.db.Database.SqlQuery<Materials>("select CPLNUM as PrP, NMP$$$NAM as Matname, ZAGSIZES as Size, EditDTE from SPRUT.OKP.dbo.OKP_POT WHERE CPLNUM=@prp", new SqlParameter("prp", SelectedDetail.PrP)).ToList();
            MaterialsDG.ItemsSource = materialsList;
        }
    }
}
