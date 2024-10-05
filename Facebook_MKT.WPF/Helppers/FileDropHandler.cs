using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Facebook_MKT.WPF.Helppers
{
	public class FileDropHandler : IDropTarget
	{
		public void DragOver(IDropInfo dropInfo)
		{
			// Xác định hành động kéo thả hợp lệ
			if (dropInfo.Data is DataObject data && data.ContainsFileDropList())
			{
				dropInfo.Effects = DragDropEffects.Copy;
			}
			else
			{
				dropInfo.Effects = DragDropEffects.None;
			}
		}

		public void Drop(IDropInfo dropInfo)
		{
			if (dropInfo.Data is DataObject data && data.ContainsFileDropList())
			{
				var files = data.GetFileDropList();

				foreach (string file in files)
				{
					string extension = System.IO.Path.GetExtension(file).ToLower();

					if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp")
					{
						// Tạo Image để hiển thị ảnh
						Image image = new Image();
						image.Source = new BitmapImage(new Uri(file));
						image.Width = 100; // Tùy chỉnh kích thước
						image.Height = 100;
						image.Margin = new Thickness(10);

						// Thêm ảnh vào StackPanel
						((Panel)dropInfo.VisualTarget).Children.Add(image);
					}
					else if (extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".wmv")
					{
						// Tạo MediaElement để phát video
						MediaElement mediaElement = new MediaElement();
						mediaElement.Source = new Uri(file);
						mediaElement.Width = 200; // Tùy chỉnh kích thước video
						mediaElement.Height = 150;
						mediaElement.LoadedBehavior = MediaState.Manual;
						mediaElement.Margin = new Thickness(10);

						// Tự động phát video
						mediaElement.Play();

						// Thêm video vào StackPanel
						((Panel)dropInfo.VisualTarget).Children.Add(mediaElement);
					}
				}
			}
		}
	}
}
