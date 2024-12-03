using Facebook_MKT.WPF.ViewModels.DataGrid;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Facebook_MKT.WPF.Controls.DataGrid.Groups
{
    /// <summary>
    /// Interaction logic for DataGridGroup.xaml
    /// </summary>
    public partial class DataGridGroup : UserControl
    {
        public DataGridGroup()
        {
            InitializeComponent();
        }
		private void myDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			// Thiết lập Header của từng hàng để hiển thị số thứ tự
			e.Row.Header = (e.Row.GetIndex() + 1).ToString();
		}
	}
}
