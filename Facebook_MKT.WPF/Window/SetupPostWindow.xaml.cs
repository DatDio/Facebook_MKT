using Faceebook_MKT.Domain.Models;
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
using System.IO;
using Microsoft.Win32;
namespace Facebook_MKT.WPF
{
    /// <summary>
    /// Interaction logic for SetupPostWindow.xaml
    /// </summary>
    public partial class SetupPostWindow : Window
	{
		private TaskField _taskField;
		public SetupPostWindow(TaskField taskField)
        {
            InitializeComponent();
			_taskField = taskField;
		}
		private void ShowFileDialog()
		{
			var dialog = new OpenFileDialog
			{
				Filter = "All Files|*.*|Image Files|*.jpg;*.jpeg;*.png;*.bmp|Video Files|*.mp4;*.avi;*.mkv",
				Multiselect = true // Cho phép chọn nhiều tệp
			};

			bool? result = dialog.ShowDialog();

			if (result == true)
			{
				HandleSelectedFiles(dialog.FileNames);
			}
		}

		private void HandleSelectedFiles(string[] files)
		{
			// Cập nhật nội dung RichTextBox
			foreach (var file in files)
			{
				ContentRichTextBox.AppendText(file + "\n");
			}

			// Lưu danh sách tệp đã chọn vào thuộc tính Value của TaskField
			_taskField.Value = string.Join(", ", files);
			 // Đóng cửa sổ
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ShowFileDialog();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true; // Đặt DialogResult để xác nhận
			Close();
		}
	}
}
