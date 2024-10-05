using Facebook_MKT.WPF.State.Navigators;
using Facebook_MKT.WPF.ViewModels;
using System.Text;
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

namespace Facebook_MKT.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : System.Windows.Window
	{
		//public MainWindow()
		//{
		//	InitializeComponent();
		//}
		public MainWindow(object dataContext)
		{
			InitializeComponent();

			DataContext = dataContext;
		}

		
	}
}