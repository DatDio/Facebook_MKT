using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Facebook_MKT.WPF.Commands;
using Facebook_MKT.WPF.Commands.LoadDataGrid;
using Facebook_MKT.WPF.ViewModels.Combobox;
using Facebook_MKT.WPF.ViewModels.Pages;
using Faceebook_MKT.Domain.Helpers.ConvertToModel;
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
namespace Facebook_MKT.WPF.ViewModels.DataGrid
{

	public class DataGridAccountViewModel : BaseSelectableViewModel<AccountModel>
	{
		private bool _isAllItemsSelected = true;
		public bool IsAllItemsSelected
		{
			get { return _isAllItemsSelected; }
			set
			{
				_isAllItemsSelected = value;
				OnPropertyChanged(nameof(IsAllItemsSelected));

				// Cập nhật tất cả các AccountModel
				foreach (var account in Items)
				{
					account.IsSelected = _isAllItemsSelected;
				}
			}
		}


		//public   ObservableCollection<AccountModel> AccountsSeleted { get; set; }

		public ObservableCollection<AccountModel> Accounts { get; set; }

		private readonly IDataService<Account> _dataService;

		private readonly IEntityToModelConverter<Account, AccountModel> _accountToModelConverter;

		private readonly FolderDataViewModel<Folder> _folderDataViewModel;

		public ICommand AddAccountCommand { get; set; }
		public ICommand SelectedOrUnSelectedCommand { get; }
		public BaseCommand LoadDataGridAccountCommand { get; set; }


		public DataGridAccountViewModel(IDataService<Account> dataService,
									ObservableCollection<AccountModel> AccountsSeleted,
									IEntityToModelConverter<Account, AccountModel> accountToModelConverter,
									FolderDataViewModel<Folder> folderDataViewModel)

		{
			_dataService = dataService;
			_accountToModelConverter = accountToModelConverter;
			_folderDataViewModel = folderDataViewModel;
			ItemsSelected = AccountsSeleted;
			//Lấy ra từ lớp cha
			Accounts = Items;

			LoadDataGridAccountCommand = new LoadDataGridAccountCommand(_dataService,
										Accounts,
										ItemsSelected,
										accountToModelConverter,
										_folderDataViewModel);
			LoadDataGridAccountCommand.Execute(1);


			#region AddAccountCommand
			AddAccountCommand = new RelayCommand<AddAccountType>((a) =>
			{
				var clipboardText = Clipboard.GetText();
				return !string.IsNullOrEmpty(clipboardText);
			},
			async (a) =>
			{
				List<Account> listAccount = new List<Account>();
				var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();

				if (items.Count == 0)
				{
					MessageBox.Show("Không có dữ liệu trong clipboard!");
					return;
				}

				switch (a)
				{
					case AddAccountType.UID_Pass_2FA:
						for (int i = 0; i < items.Count; i++)
						{
							try
							{
								var item = items[i].Split('|');

								listAccount.Add(new Account
								{
									UID = item[0].Trim(),
									Password = item[1].Trim(),
									C_2FA = item[2].Trim(),
									FolderIdKey = _folderDataViewModel._selectedItem.FolderIdKey,
									// Thêm các thuộc tính khác nếu cần thiết
								});
							}
							catch
							{
								// Xử lý lỗi nếu có
							}
						}
						break;

					case AddAccountType.UID_Pass_2FA_Cookie:
						// Thêm logic cho loại này
						break;

					case AddAccountType.UID_Pass_2FA_Cookie_Token:
						// Thêm logic cho loại này
						break;

					case AddAccountType.UID_Pass_2FA_Cookie_Token_Email_PassEmail:
						// Thêm logic cho loại này
						break;

					default:
						throw new NotImplementedException();
				}

				foreach (var account in listAccount)
				{
					try
					{
						await _dataService.Create(account);
					}
					catch { }

				}
				LoadDataGridAccountCommand.Execute(this);
			}
			);
			#endregion



			#region SelectedOrUnSelectedCommand
			SelectedOrUnSelectedCommand = new RelayCommand<System.Windows.Controls.DataGrid>((dataGrid) =>
			{
				return true;
			},
			(dataGrid) =>
			{
				if (dataGrid != null && dataGrid.SelectedItems.Count > 0)
				{
					// Lấy các tài khoản đang được bôi đen
					var selectedAccounts = dataGrid.SelectedItems.Cast<AccountModel>().ToList();

					// Cập nhật trạng thái IsSelected
					foreach (var account in selectedAccounts)
					{
						account.IsSelected = !account.IsSelected; // Đổi trạng thái
					}

					// Thông báo rằng danh sách đã thay đổi
					OnPropertyChanged(nameof(Accounts)); // Giả sử Accounts là ObservableCollection<AccountModel>
				}
			}
			);
			#endregion




			//SelectedOrUnSelectedCommand = new RelayCommand<System.Windows.Controls.DataGrid>(ExecuteSelectedOrUnSelectedCommand);
			//LoadDataToDataGridCommand = new RelayCommand<int>(LoadData);
		}



		#region SelectedOrUnSelectedCommand
		private void ExecuteSelectedOrUnSelectedCommand(System.Windows.Controls.DataGrid dataGrid)
		{
			if (dataGrid != null && dataGrid.SelectedItems.Count > 0)
			{
				// Lấy các tài khoản đang được bôi đen
				var selectedAccounts = dataGrid.SelectedItems.Cast<AccountModel>().ToList();

				// Cập nhật trạng thái IsSelected
				foreach (var account in selectedAccounts)
				{
					account.IsSelected = !account.IsSelected; // Đổi trạng thái
				}

				// Thông báo rằng danh sách đã thay đổi
				OnPropertyChanged(nameof(Accounts)); // Giả sử Accounts là ObservableCollection<AccountModel>
			}
		}
		#endregion

	}
}
