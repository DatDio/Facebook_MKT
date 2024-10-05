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

namespace Facebook_MKT.WPF.Controls.DataGrid.Pages
{
    /// <summary>
    /// Interaction logic for DataGridPage.xaml
    /// </summary>
    public partial class DataGridPage : UserControl
    {
        public DataGridPage()
        {
            InitializeComponent();
        }
		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (DataContext is DataGridPageViewModel viewModel)
			{
				viewModel.IsAllItemsSelected = true;
			}
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			if (DataContext is DataGridPageViewModel viewModel)
			{
				viewModel.IsAllItemsSelected = false;
			}
		}
	}
}
