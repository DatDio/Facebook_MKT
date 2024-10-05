using AutoMapper;
using Facebok_MKT.Service.DataService.Accounts;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.WPF.ViewModels;
using Faceebook_MKT.Domain.Models;
using Faceebook_MKT.Domain.Systems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace Facebook_MKT.WPF.Window.ChangeFolderWindow
{
	public class ChangeFolderViewModel<TFolder, TModel, TDataService> : BaseViewModel
	where TFolder : class
	where TModel : class
	where TDataService : class
	{
		private readonly TDataService _dataService;
		public ObservableCollection<TFolder> _itemsFolder { get; set; }
		public readonly ObservableCollection<TModel> _itemSelectedToChange;
		public ICommand ChangeFolderCommand { get; set; }
		private TFolder _selectedItem;
		public TFolder SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
			}
		}

		public ChangeFolderViewModel(ObservableCollection<TFolder> ItemsFolder,
							ObservableCollection<TModel> ItemSelectedToChange,
								TDataService dataService
							 )
		{
			_dataService = dataService;
			_itemsFolder = ItemsFolder;
			_itemSelectedToChange = ItemSelectedToChange;
			#region ChangeFolderCommand
			ChangeFolderCommand = new RelayCommand<object>((item) =>
			{
				return SelectedItem != null; // Chỉ cho phép áp dụng khi đã chọn một thư mục
			},
			async item =>
			{
				if (typeof(TFolder) == typeof(FolderModel))
				{
					// Cập nhật thư mục cho từng mục đã chọn
					foreach (var selectedItem in _itemSelectedToChange)
					{
						if (selectedItem is AccountModel accountModel)
						{
							accountModel.FolderIdKey = (SelectedItem as FolderModel).FolderIdKey;
							// Gọi service để lưu lại thay đổi vào cơ sở dữ liệu
							try
							{
								//await _dataService.Update(accountModel.AccountIDKey, accountModel);

								var updateMethod = typeof(TDataService).GetMethod("Update");
								await (Task<bool>)updateMethod.Invoke(_dataService, new object[] { accountModel.AccountIDKey, accountModel });
								accountModel.Status = $"Đã chuyển đến thư mục {(SelectedItem as FolderModel).FolderName}";
							}
							catch
							{

							}
						}
					}

					MessageBox.Show("Thao tác thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
					var window = Application.Current.Windows.OfType<ChangeFolderWindow>().FirstOrDefault();
					window?.Close();

				}

				if (typeof(TFolder) == typeof(FolderPageModel))
				{
					// Cập nhật thư mục cho từng mục đã chọn
					foreach (var selectedItem in _itemSelectedToChange)
					{
						if (selectedItem is PageModel pageModel)
						{
							// Giả sử mỗi mục có thuộc tính FolderIdKey
							pageModel.FolderIdKey = (SelectedItem as FolderPageModel).FolderIdKey;
							// Gọi service để lưu lại thay đổi vào cơ sở dữ liệu
							try
							{
								//await _dataService.Update(accountModel.AccountIDKey, accountModel);

								var updateMethod = typeof(TDataService).GetMethod("Update");
								var status= await (Task<bool>)updateMethod.Invoke(_dataService, new object[] { pageModel.PageID, pageModel });
								if (status)
								{
									pageModel.PageStatus = $"Đã chuyển đến thư mục {(SelectedItem as FolderPageModel).FolderName}";
									pageModel.TextColor=SystemContants.RowColorSuccess;
								}
								else
								{
									pageModel.PageStatus = $"Có lỗi xảy ra khi chuyển thư mục";
									pageModel.TextColor = SystemContants.RowColorFail;
								}
							}
							catch
							{

							}

						}
					}

					MessageBox.Show("Thao tác thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
					var window = Application.Current.Windows.OfType<ChangeFolderWindow>().FirstOrDefault();
					window?.Close();

				}
			});
			#endregion
		}


	}
}
