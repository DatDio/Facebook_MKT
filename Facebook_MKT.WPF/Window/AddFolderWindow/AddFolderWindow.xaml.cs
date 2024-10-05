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

namespace Facebook_MKT.WPF.Window.AddFolderWindow
{
    /// <summary>
    /// Interaction logic for AddFolderWindow.xaml
    /// </summary>
    public partial class AddFolderWindow : System.Windows.Window
    {
		public string FolderName => FolderNameTextBox.Text;
		public AddFolderWindow()
        {
            InitializeComponent();
        }
		private void AddFolder_Click(object sender, RoutedEventArgs e)
		{
			// Đóng cửa sổ và trả về true nếu người dùng nhấn "Thêm"
			DialogResult = true;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			// Đóng cửa sổ và trả về false nếu người dùng nhấn "Hủy"
			DialogResult = false;
		}
	}
}
