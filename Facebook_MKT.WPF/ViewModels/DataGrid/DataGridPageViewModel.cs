using Facebook_MKT.Data.Entities;
using Facebook_MKT.WPF.Commands;
using Facebook_MKT.WPF.ViewModels.Combobox;
using Facebook_MKT.WPF.ViewModels.Pages;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using static MaterialDesignThemes.Wpf.Theme;
using AutoMapper;
using System.Windows.Documents;
using Facebok_MKT.Service.DataService.Pages;
using Facebok_MKT.Service.DataService.Accounts;
using Facebook_MKT.WPF.Window.ChangeFolderWindow;
using System.Diagnostics;
using System.IO;
using Faceebook_MKT.Domain.EditVideoController;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Systems;
namespace Facebook_MKT.WPF.ViewModels.DataGrid
{

	public class DataGridPageViewModel : BaseSelectableViewModel<PageModel>
	{
		public static ObservableCollection<PageModel> Pages { get; set; }

		private readonly IPageDataService _pagedataService;

		private readonly FolderPageViewModel _folderPageViewModel;

		//private readonly ObservableCollection<AccountModel> _AccountsSeleted;

		#region Command
		public ICommand DeletePageCommand { get; set; }
		public ICommand SavePageCommand { get; set; }
		public ICommand OpenBrowserCommand { get; set; }
		public ICommand OpenFolderVideoCommand { get; set; }
		public ICommand SelectedOrUnSelectedCommand { get; }
		public ICommand LoadDataGridPageCommand { get; set; }
		public ICommand ChangeFolderCommand { get; set; }
		public ICommand StatisticalCommand { get; set; }
		public ICommand EditVideoCommand { get; set; }
		#endregion

		public DataGridPageViewModel(IPageDataService pagedataService,
									ObservableCollection<PageModel> PagesSeleted,
									FolderPageViewModel folderPageViewModel)

		{
			_pagedataService = pagedataService;
			_folderPageViewModel = folderPageViewModel;
			ItemsSelected = PagesSeleted;

			//Lấy ra từ lớp cha
			Pages = Items;

			#region LoadDataGridPageCommand
			LoadDataGridPageCommand = new RelayCommand<object>((p) =>
			{
				return true;

			}, async (p) =>
			{
				// Lấy FolderIdKey từ SelectedItem trong ViewModel
				var folderidKey = _folderPageViewModel.SelectedItem?.FolderIdKey;

				if (folderidKey.HasValue)
				{
					List<PageModel> pageModels = null;
					if (folderidKey == 1)
					{
						pageModels = await _pagedataService.GetAll();

					}
					else
					{
						pageModels = await _pagedataService.GetByFolderIdKey(folderidKey.Value);
					}
					// Xóa dữ liệu cũ
					Pages.Clear();
					PagesSeleted.Clear();
					// Thêm các model vào danh sách Accounts
					foreach (var page in pageModels)
					{
						Pages.Add(page);
					}
				}
			});

			LoadDataGridPageCommand.Execute(1);

			#endregion

			#region SelectedOrUnSelectedCommand
			SelectedOrUnSelectedCommand = new RelayCommand<System.Windows.Controls.DataGrid>((dataGrid) =>
			{
				return dataGrid != null && dataGrid.SelectedItems.Count > 0;
				return true;
			},
			(dataGrid) =>
			{
				if (dataGrid != null && dataGrid.SelectedItems.Count > 0)
				{
					// Lấy các tài khoản đang được bôi đen
					var selectedPages = dataGrid.SelectedItems.Cast<PageModel>().ToList();

					// Cập nhật trạng thái IsSelected
					foreach (var page in selectedPages)
					{
						page.IsSelected = !page.IsSelected; // Đổi trạng thái
					}

					// Thông báo rằng danh sách đã thay đổi
					OnPropertyChanged(nameof(Pages)); // Giả sử Accounts là ObservableCollection<AccountModel>
				}
			}
			);
			#endregion

			#region DeleteAccountCommand
			DeletePageCommand = new RelayCommand<object>((p) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (p) =>
			{
				var result = MessageBox.Show("Bạn có chắc muốn xóa page này?", "Cảnh Báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					foreach (var pageModel in ItemsSelected)
					{
						await _pagedataService.Delete(pageModel.PageID);
					}
					LoadDataGridPageCommand.Execute(_folderPageViewModel.SelectedItem.FolderIdKey);
				}
			});
			#endregion
			#region OpenFolderVideoCommand
			OpenFolderVideoCommand = new RelayCommand<object>((p) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (p) =>
			{
				foreach (var pageModel in ItemsSelected)
				{
					// Kiểm tra nếu thư mục chứa video của trang đó tồn tại
					if (!string.IsNullOrEmpty(pageModel.PageFolderVideo)
					&& Directory.Exists(Path.GetFullPath(pageModel.PageFolderVideo)))
					{
						// Mở thư mục video của trang đó
						Process.Start(new ProcessStartInfo()
						{
							FileName = Path.GetFullPath(pageModel.PageFolderVideo),
							UseShellExecute = true, // Mở bằng trình quản lý tệp mặc định (File Explorer)
							Verb = "open"
						});
					}
				}

			});
			#endregion

			#region SavePageCommand
			SavePageCommand = new RelayCommand<object>((p) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (p) =>
			{
				foreach (var pageModel in ItemsSelected)
				{
					await _pagedataService.Update(pageModel.PageID, pageModel);
				}
			});
			#endregion

			#region OpenBrowserCommand
			OpenBrowserCommand = new RelayCommand<object>((a) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (a) =>
			{

			});
			#endregion


			#region StatisticalCommand
			StatisticalCommand = new RelayCommand<object>((a) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (a) =>
			{

			});
			#endregion

			#region ChangeFolderCommand
			ChangeFolderCommand = new RelayCommand<object>((a) =>
			{
				// Chỉ bật lệnh khi có tài khoản được chọn
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (a) =>
			{
				// Mở cửa sổ ChangeFolderWindow để chọn thư mục mới
				var changeFolderWindow = new ChangeFolderWindow();
				changeFolderWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				// Khởi tạo ChangeFolderViewModel với danh sách thư mục và tài khoản đã chọn
				var _changeFolderViewModel = new
				ChangeFolderViewModel<FolderPageModel,
				PageModel, IPageDataService>(_folderPageViewModel.Items,
												ItemsSelected,
												_pagedataService);

				// Gán DataContext của window cho ViewModel
				changeFolderWindow.DataContext = _changeFolderViewModel;

				// Mở cửa sổ ChangeFolderWindow dưới dạng dialog
				changeFolderWindow.ShowDialog();
			});
			#endregion

			#region EditVideoCommand
			EditVideoCommand = new RelayCommand<object>((p) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
async (p) =>
{
	await Task.Run(async () =>
	{

		foreach (var pageModel in ItemsSelected)
		{
			pageModel.TextColor = SystemContants.RowColorRunning;
			var allSubFolder = FolderHelper.GetSubdirectories(pageModel.PageFolderVideo);
			int stt = 0;
			foreach (var subfolder in allSubFolder)
			{
				stt++;
				pageModel.PageStatus = $"Đang edit video {stt}/{allSubFolder.Count}";
				var videoPath = FolderHelper.GetFilesInSubFolders(subfolder, "mp4");
				if (!String.IsNullOrEmpty(videoPath))
				{
					var fileName = Path.GetFileNameWithoutExtension(videoPath);
					if (fileName.Contains("DONE")) continue;
					var outputPath = Path.GetFullPath(Path.Combine(subfolder, fileName + "DONE.mp4"));
					
					if (FFmpegController.Add2LayerAndAddText(videoPath, outputPath))
					{
						File.Delete(videoPath);
					}
				}
			}

			// Sau khi hoàn thành xử lý video, cập nhật lại trạng thái trên UI thread
			// Đảm bảo cập nhật UI trên chính UI thread
			Application.Current.Dispatcher.Invoke(() =>
			{
				pageModel.PageStatus = "Đã edit xong!";
				_pagedataService.Update(pageModel.PageID, pageModel);
				pageModel.TextColor = SystemContants.RowColorSuccess;
			});
		}
	});
});

			#endregion
		}
	}
}
