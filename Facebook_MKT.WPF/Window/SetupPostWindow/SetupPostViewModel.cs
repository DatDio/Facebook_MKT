using Facebook_MKT.WPF.ViewModels;
using Faceebook_MKT.Domain.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NReco.VideoInfo;
using System.Windows.Controls;
using Vlc.DotNet.Core;
using MediaToolkit.Model;
using MediaToolkit;
using MediaToolkit.Options;
using System.Windows;
namespace Facebook_MKT.WPF.Window.SetupPostWindow
{
	public class SetupPostViewModel : BaseViewModel
	{
		private readonly TaskModel _taskModel;
		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				if (Title != value)
				{
					_title = value;
					OnPropertyChanged(nameof(Title));
					_taskModel.Fields.FirstOrDefault(f => f.FieldType == TaskFieldType.Label).Label = _title;
				}
			}
		}
		public ObservableCollection<MediaFileModel> MediaFiles { get; set; }
		public ICommand SelectFileCommand { get; }
		public ICommand RemoveFileCommand { get; }
		public ICommand ConfirmCommand { get; }
		public SetupPostViewModel(TaskModel taskModel)
		{
			_taskModel = taskModel;
			var mediaField = _taskModel.Fields.FirstOrDefault(f => f.FieldType == TaskFieldType.Media);
			if (mediaField != null)
			{
				// Kiểm tra và khởi tạo giá trị nếu nó chưa được khởi tạo hoặc không phải là ObservableCollection
				if (mediaField.Value == null || !(mediaField.Value is ObservableCollection<MediaFileModel>))
				{
					mediaField.Value = new ObservableCollection<MediaFileModel>();
				}

				// Gán MediaFiles từ TaskField
				this.MediaFiles = (ObservableCollection<MediaFileModel>)mediaField.Value;
			}

			Title = _taskModel.Fields.FirstOrDefault(f => f.FieldType == TaskFieldType.Label).Label.ToString();

			#region SelectFileCommand
			SelectFileCommand = new RelayCommand<object>(
			(a) =>
			{
				return true;
			},
			async (a) =>
			{
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Video files (*.mp4, *.avi, *.mov, *.wmv)|*.mp4;*.avi;*.mov;*.wmv",
					Multiselect = true
				};

				if (openFileDialog.ShowDialog() == true)
				{
					foreach (string file in openFileDialog.FileNames)
					{
						MediaFileModel mediaFile = new MediaFileModel
						{
							FilePath = file,
							IsImage = IsImageFile(file),
							IsVideo = IsVideoFile(file),
							ThumbnailPath = IsImageFile(file) ? file : GetVideoThumbnail(file) // Lấy thumbnail nếu là video
						};
						MediaFiles.Add(mediaFile);
					}
				}
			});
			#endregion

			#region RemoveFileCommand
			RemoveFileCommand = new RelayCommand<MediaFileModel>((a) =>
			{
				return true;
			},
			async (a) =>
			{
				MediaFiles.Remove(a);
			});
			#endregion

			#region ConfirmCommand
			ConfirmCommand = new RelayCommand<object>(
			(a) =>
			{
				if( MediaFiles != null && MediaFiles.Count > 0 && !String.IsNullOrEmpty(_title))
				return true;
				else return false;
			},
			async (a) =>
			{
				var filePaths = MediaFiles
	.Where(m => !string.IsNullOrEmpty(m.FilePath)) // Lọc các giá trị FilePath không null hoặc rỗng
	.Select(m => m.FilePath)                       // Lấy thuộc tính FilePath
	.ToArray();                                    // Chuyển sang mảng để sử dụng với string.Join

				// Sử dụng string.Join để kết hợp các FilePath thành một chuỗi, cách nhau bởi dấu phẩy
				var combinedFilePaths = string.Join("\n", filePaths);

				_taskModel.Fields.FirstOrDefault(f => f.FieldType == TaskFieldType.File).Value = combinedFilePaths;
				
				_taskModel.Fields.FirstOrDefault(f => f.FieldType == TaskFieldType.Label).Label = _title;
				var window = Application.Current.Windows.OfType<SetupPostWindow>().FirstOrDefault();
				window?.Close();
			});
			#endregion
		}

		private bool IsImageFile(string filePath)
		{
			string extension = System.IO.Path.GetExtension(filePath).ToLower();
			return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp";
		}
		private void RemoveFile(MediaFileModel mediaFile)
		{
			MediaFiles.Remove(mediaFile);
		}
		private bool IsVideoFile(string filePath)
		{
			string extension = System.IO.Path.GetExtension(filePath).ToLower();
			return extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".wmv";
		}
		private string GetVideoThumbnail(string filePath)
		{
			// Đường dẫn để lưu thumbnail
			string thumbnailPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");

			var inputFile = new MediaFile { Filename = filePath };
			var outputFile = new MediaFile { Filename = thumbnailPath };

			using (var engine = new Engine())
			{
				// Tạo đối tượng ConversionOptions
				var options = new ConversionOptions
				{
					Seek = TimeSpan.FromSeconds(1) // Lấy thumbnail tại giây thứ 1
				};

				// Lấy thumbnail từ video
				engine.GetThumbnail(inputFile, outputFile, options);
			}

			return thumbnailPath; // Trả về đường dẫn tới thumbnail
		}
	}
}
