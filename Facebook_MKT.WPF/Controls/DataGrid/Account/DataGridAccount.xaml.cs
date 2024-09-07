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
using static MaterialDesignThemes.Wpf.Theme;

namespace Facebook_MKT.WPF.Controls.DataGrid.Account
{
	/// <summary>
	/// Interaction logic for DataGridAccount.xaml
	/// </summary>
	public partial class DataGridAccount : UserControl
	{
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
	}
}
