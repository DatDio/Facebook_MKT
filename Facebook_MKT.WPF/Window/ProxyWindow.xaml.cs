using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Facebook_MKT.WPF
{
	/// <summary>
	/// Interaction logic for ProxyWindow.xaml
	/// </summary>
	public partial class ProxyWindow : System.Windows.Window
	{
		public List<string> Proxy { get; private set; }

		public ProxyWindow()
		{
			InitializeComponent();
			Proxy = new List<string>(); // Khởi tạo danh sách Proxy

			if (File.Exists("Input/proxyList.txt"))
			{
				string text = File.ReadAllText("Input/proxyList.txt");
				ProxyTextBox.Document.Blocks.Clear();
				ProxyTextBox.Document.Blocks.Add(new Paragraph(new Run(text)));
			}
			else
			{
				if (!Directory.Exists("Input"))
				{
					Directory.CreateDirectory("Input");
				}
				File.Create("Input/proxyList.txt").Dispose(); // Tạo file và đóng nó ngay lập tức
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			TextRange textRange = new TextRange(ProxyTextBox.Document.ContentStart, ProxyTextBox.Document.ContentEnd);
			Proxy = textRange.Text
				.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries) // Tách các dòng và bỏ qua dòng trống
				.Select(proxy => proxy.Trim()) // Loại bỏ khoảng trắng ở đầu và cuối của từng dòng
				.Where(proxy => !string.IsNullOrWhiteSpace(proxy)) // Bỏ các dòng chỉ chứa khoảng trắng
				.ToList();

			// Ghi giá trị vào file
			File.WriteAllText("Input/proxyList.txt", textRange.Text);
			this.DialogResult = true; // Đóng cửa sổ và trả về kết quả
			this.Close();
		}
	}

}
