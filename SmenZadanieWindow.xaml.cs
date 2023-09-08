using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ac.Models;

namespace ac
{
    public partial class SmenZadanieWindow : Window
    {
        List<SmenZadanie> SZList = new List<SmenZadanie>();

        public SmenZadanieWindow()
        {
            InitializeComponent();

            SZList = Odb.db.Database.SqlQuery<SmenZadanie>("SELECT DISTINCT id_Tabel, DTE, Cost, FIO, NUM, Detail, OrderNum, OperationNum, Operation, Status AS StatusBool, Count, DEP$$$DEP, WCR$$$WCR, SHIFT, Product FROM [Zarplats].[dbo].[SmenZadView]").ToList();
            SmenZadanieDG.ItemsSource = SZList;
        }
    }
}
