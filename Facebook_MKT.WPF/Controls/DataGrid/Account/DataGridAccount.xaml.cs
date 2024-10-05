using Facebook_MKT.WPF.ViewModels.DataGrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MaterialDesignThemes.Wpf.Theme;

namespace Facebook_MKT.WPF.Controls.DataGrid.Account
{
	/// <summary>
	/// Interaction logic for DataGridAccount.xaml
	/// </summary>
	public partial class DataGridAccount : UserControl
	{
		private bool isDoubleClick = false;
		public DataGridAccount()
		{
			InitializeComponent();
			//MessageBox.Show(this.DataContext.ToString());
			//Console.WriteLine("sdjkghdsi");
		}
		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (DataContext is DataGridAccountViewModel viewModel)
			{
				viewModel.IsAllItemsSelected = true;
			}
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			if (DataContext is DataGridAccountViewModel viewModel)
			{
				viewModel.IsAllItemsSelected = false;
			}
		}

		private void DataGridCell_Selected(object sender, RoutedEventArgs e)
		{
			// Lookup for the source to be DataGridCell
			if (e.OriginalSource.GetType() == typeof(DataGridCell))
			{
				// Starts the Edit on the row;
				System.Windows.Controls.DataGrid grd = (System.Windows.Controls.DataGrid)sender;
				grd.BeginEdit(e);
			}
		}

		private void MyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{ // Lấy DataGrid từ sender
			if (sender.GetType() == typeof(DataGridCell))
			{
				DataGridCell cell = sender as DataGridCell;
				cell.IsEditing = true;
			}
		}
	}

}



