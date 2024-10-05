using Facebok_MKT.Service.DataService.Folders.FolderAccounts;
using Facebok_MKT.Service.DataService.Folders.FolderPages;
using Facebook_MKT.WPF.Window.AddFolderWindow;
using Faceebook_MKT.Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Facebook_MKT.WPF.ViewModels.Combobox
{
	public class FolderPageViewModel:BaseViewModel
	{
		private readonly IFolderPageDataService _folderPageDataService;
		public ObservableCollection<FolderPageModel> Items { get; private set; } = new ObservableCollection<FolderPageModel>();

		private FolderPageModel _selectedItem;
		public FolderPageModel SelectedItem
		{
			get => _selectedItem;
			set
			{
				_selectedItem = value;
				OnPropertyChanged(nameof(SelectedItem));
				OnSelectedItemChanged();
			}
		}

		public ICommand AddFolderCommand { get; set; }
		public ICommand DeleteFolderCommand { get; set; }
		public event Action<int> FolderChanged;

		public FolderPageViewModel(IFolderPageDataService folderPageDataService)
		{
			_folderPageDataService = folderPageDataService;
			LoadData().ConfigureAwait(false);
			SetDefaultSelection();


			#region AddFolderCommand
			AddFolderCommand = new RelayCommand<object>((item) =>
			{
				return true;
			},
		async item =>
		{
		reAddFolder:
			var addFolderWindow = new AddFolderWindow();

			// Hiển thị cửa sổ dưới dạng hộp thoại
			bool? result = addFolderWindow.ShowDialog();

			// Xử lý kết quả sau khi người dùng đóng cửa sổ
			if (result == true)
			{
				// Lấy tên thư mục từ cửa sổ
				string folderName = addFolderWindow.FolderName;

				if (!string.IsNullOrWhiteSpace(folderName))
				{
					// Gọi phương thức để thêm thư mục
					bool isSuccess = await AddFolderAsync(folderName);

					// Thông báo kết quả
					if (isSuccess)
					{
						MessageBox.Show("Thao tác thành công!");
					}
					else
					{

						goto reAddFolder;
					}
				}
				else
				{
					MessageBox.Show("Tên thư mục không được để trống.");
				}
			}
		});


			#endregion

			#region DeleteFolderCommand
			DeleteFolderCommand = new RelayCommand<object>((item) =>
			{
				return SelectedItem != null;
			},
				async item =>
				{
					var result = MessageBox.Show("Bạn có chắc muốn xóa thư mục này?", "Cảnh Báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes)
					{
						try
						{
							var resultDelete = await DeleteFolderAsync(SelectedItem);
							if (resultDelete)
							{
								await LoadData();
							}
							else
							{
								MessageBox.Show("Có lỗi xảy ra!");
							}

						}
						catch
						{
							MessageBox.Show("Có lỗi xảy ra!");
						}

					}
				});
			#endregion
		}

		public async Task LoadData()
		{
			// Lưu lại SelectedItem hiện tại (nếu có)
			var previousSelectedItem = SelectedItem;

			// Tải dữ liệu mới
			var result = await _folderPageDataService.GetAll();

			// Xóa tất cả các phần tử trong Items
			Items.Clear();

			// Thêm các phần tử mới vào Items
			foreach (var item in result)
			{
				Items.Add(item);
			}

			// Khôi phục lại SelectedItem nếu nó tồn tại trong Items
			if (previousSelectedItem != null)
			{
				var matchingItem = Items.FirstOrDefault(i => i.FolderIdKey == previousSelectedItem.FolderIdKey);
				if (matchingItem != null)
				{
					SelectedItem = matchingItem; // Gán lại giá trị cho SelectedItem
				}
			}
			else if (Items.Any())
			{
				SelectedItem = Items.First(); // Chọn item đầu tiên nếu không có item nào trước đó
			}
		}

		private void SetDefaultSelection()
		{
			//var folderItems = Items.Cast<Folder>().ToList();
			SelectedItem = Items.FirstOrDefault(f => f.FolderName == "All");
		}

		private async Task<bool> AddFolderAsync(string folderName)
		{

			var newFolder = new FolderPageModel()
			{
				FolderName = folderName,
			};

			try
			{
				await _folderPageDataService.Create(newFolder);
				await LoadData();

				return true;
			}
			catch (DbUpdateException dbEx)
			{
				// Kiểm tra xem InnerException có phải là SqliteException không
				if (dbEx.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
				{
					MessageBox.Show("Tên thư mục đã tồn tại. Vui lòng chọn tên khác.");
				}
				else
				{
					MessageBox.Show("Lỗi cập nhật cơ sở dữ liệu, vui lòng thử lại.");
				}
				return false;
			}

			catch
			{
				MessageBox.Show("Thêm thư mục thất bại, vui lòng thử lại.");
				return false;
			}
		}

		private async Task<bool> DeleteFolderAsync(FolderPageModel item)
		{
			var result = await _folderPageDataService.Delete(item.FolderIdKey);
			if (result == true)
			{
				await LoadData();
				MessageBox.Show("Thao tác thành công!");
				return true;
			}
			return false;
		}
		private void OnSelectedItemChanged()
		{
			try
			{
				if (SelectedItem != null)
				{
					FolderChanged?.Invoke(SelectedItem.FolderIdKey);
				}
			}
			catch
			{

			}
		}
	}
}
