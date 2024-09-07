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
using AutoMapper;
using System.Windows.Documents;
namespace Facebook_MKT.WPF.ViewModels.DataGrid
{

	public class DataGridAccountViewModel : BaseSelectableViewModel<AccountModel>
	{
		


		//public   ObservableCollection<AccountModel> AccountsSeleted { get; set; }

		public ObservableCollection<AccountModel> Accounts { get; set; }

		private readonly IDataService<Account> _dataService;

		private readonly IEntityToModelConverter<Account, AccountModel> _accountToModelConverter;

		private readonly FolderDataViewModel<Folder> _folderAccountViewModel;

		private readonly IMapper _mapper;
		//private readonly ObservableCollection<AccountModel> _AccountsSeleted;
		public ICommand AddAccountCommand { get; set; }
		public ICommand DeleteAccountCommand { get; set; }
		public ICommand SaveAccountCommand { get; set; }
		public ICommand OpenBrowserCommand { get; set; }
		public ICommand AddProxyCommand { get; set; }
		public ICommand SelectedOrUnSelectedCommand { get; }
		public ICommand LoadDataGridAccountCommand { get; set; }
		public DataGridAccountViewModel(IDataService<Account> dataService,
									ObservableCollection<AccountModel> AccountsSeleted,
									//IEntityToModelConverter<Account, AccountModel> accountToModelConverter,
									IMapper mapper,
									FolderDataViewModel<Folder> folderDataViewModel)

		{
			_dataService = dataService;
			//_accountToModelConverter = accountToModelConverter;
			_mapper = mapper;
			_folderAccountViewModel = folderDataViewModel;
			ItemsSelected = AccountsSeleted;
			//Lấy ra từ lớp cha
			Accounts = Items;

			//LoadDataGridAccountCommand = new LoadDataGridAccountCommand(_dataService,
			//							Accounts,
			//							ItemsSelected,
			//							accountToModelConverter,
			//							_folderDataViewModel);
			//LoadDataGridAccountCommand.Execute(1);

			#region LoadDataGridAccountCommand
			LoadDataGridAccountCommand = new RelayCommand<object>((p) =>
			{
				return true;

			}, async (p) =>
			{
				// Lấy FolderIdKey từ SelectedItem trong ViewModel
				var folderidKey = _folderAccountViewModel.SelectedItem?.FolderIdKey;

				if (folderidKey.HasValue)
				{
					// Lấy dữ liệu dựa trên FolderIdKey
					var data = await _dataService.GetByFolderIdKey(folderidKey.Value);

					// Xóa dữ liệu cũ
					Accounts.Clear();
					AccountsSeleted.Clear();

					// Sử dụng AutoMapper để map từ entity sang model
					var accountModels = _mapper.Map<List<AccountModel>>(data);

					// Thêm các model vào danh sách Accounts
					foreach (var accountModel in accountModels)
					{
						Accounts.Add(accountModel);
					}
				}
			});

			LoadDataGridAccountCommand.Execute(1);

			#endregion

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
									FolderIdKey = _folderAccountViewModel._selectedItem.FolderIdKey,
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
				return dataGrid != null && dataGrid.SelectedItems.Count > 0;
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

			#region DeleteAccountCommand
			DeleteAccountCommand = new RelayCommand<object>((a) =>
			{
				return true;
			},
			async (a) =>
			{
				foreach(var account in ItemsSelected)
				{
					await _dataService.Delete(account.AccountIDKey);
				}
			});
			#endregion

			#region SaveAccountCommand
			SaveAccountCommand = new RelayCommand<object>((a) =>
			{
				return true;
			},
			async (a) =>
			{
				foreach (var accountModel in ItemsSelected)
				{
					var accountEntity =  _mapper.Map<Account>(accountModel);
					await _dataService.Update(accountEntity.AccountIDKey, accountEntity);
				}
			});
			#endregion

			#region OpenBrowserCommand
			OpenBrowserCommand = new RelayCommand<object>((a) =>
			{
				return true;
			},
			async (a) =>
			{
				foreach (var accountModel in ItemsSelected)
				{
					var accountEntity = _mapper.Map<Account>(accountModel);
					await _dataService.Update(accountEntity.AccountIDKey, accountEntity);
				}
			});
			#endregion

			#region AddProxyCommand
			AddProxyCommand = new RelayCommand<object>((a) =>
			{
				return true;
			},
			async (a) =>
			{
				// Mở ProxyWindow để người dùng nhập Proxy
				ProxyWindow proxyWindow = new ProxyWindow();
				bool? result = proxyWindow.ShowDialog();
				if (result == true)
				{
					List<string> proxy = proxyWindow.Proxy;

					// Cập nhật Proxy cho từng AccountModel được chọn
					foreach (var accountModel in ItemsSelected)
					{
						accountModel.Proxy = proxy[0];
						proxy.RemoveAt(0);
						proxy.Add(accountModel.Proxy);
						var accountEntity = _mapper.Map<Account>(accountModel);
						await _dataService.Update(accountEntity.AccountIDKey, accountEntity);
					}
					MessageBox.Show("Thao tác thành công");
				}
			});
			#endregion
		}
	}
}
