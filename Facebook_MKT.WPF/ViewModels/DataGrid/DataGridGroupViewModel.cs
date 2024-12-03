using Facebok_MKT.Service.DataService.Pages;
using Facebook_MKT.WPF.ViewModels.Combobox;
using Facebook_MKT.WPF.Window.ChangeFolderWindow;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using System.Windows;
using System.Windows.Input;
using Facebok_MKT.Service.DataService.Groups;

namespace Facebook_MKT.WPF.ViewModels.DataGrid
{
	public class DataGridGroupViewModel : BaseSelectableViewModel<GroupModel>
	{
		public static ObservableCollection<GroupModel> Groups { get; set; }

		private readonly IGroupDataService _groupDataService;

		private readonly FolderGroupViewModel _folderGroupViewModel;

		//private readonly ObservableCollection<AccountModel> _AccountsSeleted;

		#region Command
		public ICommand DeleteGroupCommand { get; set; }
		public ICommand SaveGroupCommand { get; set; }
		public ICommand SelectedOrUnSelectedCommand { get; }
		public ICommand LoadDataGridGroupCommand { get; set; }
		public ICommand ChangeFolderCommand { get; set; }
		#endregion

		public DataGridGroupViewModel(IGroupDataService groupDataService,
									ObservableCollection<GroupModel> GroupsSeleted,
									FolderGroupViewModel folderGroupViewModel)

		{
			_groupDataService = groupDataService;
			_folderGroupViewModel = folderGroupViewModel;
			ItemsSelected = GroupsSeleted;
			Groups = Items;

			#region LoadDataGridGroupCommand
			LoadDataGridGroupCommand = new RelayCommand<object>((p) =>
			{
				return true;

			}, async (p) =>
			{
				// Lấy FolderIdKey từ SelectedItem trong ViewModel
				var folderidKey = _folderGroupViewModel.SelectedItem?.FolderIdKey;

				if (folderidKey.HasValue)
				{
					List<GroupModel> groupModels = null;
					if (folderidKey == 1)
					{
						groupModels = await _groupDataService.GetAll();

					}
					else
					{
						groupModels = await _groupDataService.GetByFolderIdKey(folderidKey.Value);
					}
					// Xóa dữ liệu cũ
					Groups.Clear();
					GroupsSeleted.Clear();
					// Thêm các model vào danh sách Accounts
					foreach (var group in groupModels)
					{
						Groups.Add(group);
					}
				}
			});

			LoadDataGridGroupCommand.Execute(1);

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
					var selectedGroups = dataGrid.SelectedItems.Cast<GroupModel>().ToList();

					// Cập nhật trạng thái IsSelected
					foreach (var group in selectedGroups)
					{
						group.IsSelected = !group.IsSelected; // Đổi trạng thái
					}

					// Thông báo rằng danh sách đã thay đổi
					OnPropertyChanged(nameof(Groups));
				}
			}
			);
			#endregion

			#region DeleteGroupCommand
			DeleteGroupCommand = new RelayCommand<object>((p) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (p) =>
			{
				var result = MessageBox.Show("Bạn có chắc muốn xóa groups này?", "Cảnh Báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					foreach (var groupModel in ItemsSelected)
					{
						await _groupDataService.Delete(groupModel.GroupID);
					}
					LoadDataGridGroupCommand.Execute(_folderGroupViewModel.SelectedItem.FolderIdKey);
				}
			});
			#endregion

			#region SaveGroupCommand
			SaveGroupCommand = new RelayCommand<object>((p) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (p) =>
			{
				foreach (var groupModel in ItemsSelected)
				{
					await _groupDataService.Update(groupModel.GroupID, groupModel);
				}
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
				ChangeFolderViewModel<FolderGroupModel,
				GroupModel, IGroupDataService>(_folderGroupViewModel.Items,
												ItemsSelected,
												_groupDataService);

				// Gán DataContext của window cho ViewModel
				changeFolderWindow.DataContext = _changeFolderViewModel;

				// Mở cửa sổ ChangeFolderWindow dưới dạng dialog
				changeFolderWindow.ShowDialog();
			});
			#endregion
		
		}
	}
}
