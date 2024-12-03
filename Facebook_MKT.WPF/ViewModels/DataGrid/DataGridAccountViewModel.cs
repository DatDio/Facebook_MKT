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
using Facebook_MKT.WPF.Window.ChangeFolderWindow;
using Facebook_MKT.WPF.Helppers;
using Facebok_MKT.Service.BrowserService;
using System.Diagnostics;
using Facebook_MKT.WPF.ViewModels.General_settings;
using System.ComponentModel;
using System.Threading;
using System.Windows.Automation;
using Facebok_MKT.Service.DataService.Pages;
using Facebok_MKT.Service.DataService.Accounts;
using Facebok_MKT.Service.Controller.FacebookAPIController;
using Faceebook_MKT.Domain.Systems;
namespace Facebook_MKT.WPF.ViewModels.DataGrid
{
	public class DataGridAccountViewModel : BaseSelectableViewModel<AccountModel>
	{

		//public   ObservableCollection<AccountModel> AccountsSeleted { get; set; }
		// Thuộc tính đếm số lượng dòng
		private double _scale;
		private int _maxParallelTasks;
		private string _apiGPMUrl;
		private object lockPosition;
		private FacebookAccountAPI FbAccountAPI;
		private ObservableCollection<AccountModel> _accounts;
		public ObservableCollection<AccountModel> Accounts
		{
			get => _accounts;
			set
			{
				_accounts = value;
				OnPropertyChanged(nameof(Accounts));
			}
		}
		private readonly IAccountDataService _accountdataService;
		private readonly IPageDataService _pagedataService;
		private readonly FolderAccountViewModel _folderAccountViewModel;
		private readonly FolderPageViewModel _folderPageViewModel;
		private GeneralSettingsViewModel _generalSettings;
		private DataGridPageViewModel _dataGridPageViewModel;
		private IPropertyFileds _propertyFileds;
		//private readonly ObservableCollection<AccountModel> _AccountsSeleted;
		public ICommand AddAccountCommand { get; set; }
		public ICommand DeleteAccountCommand { get; set; }
		public ICommand SaveAccountCommand { get; set; }
		public ICommand OpenBrowserCommand { get; set; }
		public ICommand AddProxyCommand { get; set; }
		public ICommand ChangeFolderCommand { get; set; }
		public ICommand SelectedOrUnSelectedCommand { get; }
		public ICommand LoadDataGridAccountCommand { get; set; }
		public ICommand GetAllPageCommand { get; set; }
		public ICommand CheckLiveUIDCommand { get; set; }
		public ICommand CheckLiveCookieCommand { get; set; }
		public DataGridAccountViewModel(IAccountDataService accountdataService,
										IPageDataService pagedataService,
										ObservableCollection<AccountModel> AccountsSeleted,
										FolderAccountViewModel folderAccountViewModel,
										FolderPageViewModel folderPageViewModel,
										GeneralSettingsViewModel generalSettings,
										IPropertyFileds propertyFileds)

		{
			lockPosition = new object();
			_accountdataService = accountdataService;
			_pagedataService = pagedataService;
			_generalSettings = generalSettings;
			_generalSettings.PropertyChanged += OnGeneralSettingsChanged;
			_scale = generalSettings.Scale;
			_apiGPMUrl = generalSettings.APIURL;
			//_dataGridPageViewModel = dataGridPageViewModel;
			_propertyFileds = propertyFileds;
			//_maxParallelTasks = _propertyFileds.MaxParallelTasks;
			_folderAccountViewModel = folderAccountViewModel;
			_folderPageViewModel = folderPageViewModel;
			ItemsSelected = AccountsSeleted;
			//Lấy ra từ lớp cha
			Accounts = Items;


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
					List<AccountModel> ListaccountModels = null;
					// Lấy dữ liệu dựa trên FolderIdKey
					if (folderidKey == 1)
					{
						ListaccountModels = await _accountdataService.GetAll();
					}
					else
					{
						ListaccountModels = await _accountdataService.GetByFolderIdKey(folderidKey.Value);
					}
					// Xóa dữ liệu cũ
					Accounts.Clear();
					AccountsSeleted.Clear();
					ItemsSelected.Clear();

					// Thêm các model vào danh sách Accounts
					foreach (var accountModel in ListaccountModels)
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
				List<AccountModel> listAccount = new List<AccountModel>();
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

								listAccount.Add(new AccountModel
								{
									UID = item[0],
									Password = item[1].Trim(),
									C_2FA = item[2].Trim(),
									FolderIdKey = _folderAccountViewModel.SelectedItem.FolderIdKey,
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
						for (int i = 0; i < items.Count; i++)
						{
							try
							{
								var item = items[i].Split('|');

								listAccount.Add(new AccountModel
								{
									UID = item[0],
									Password = item[1].Trim(),
									C_2FA = item[2].Trim(),
									Cookie = item[3].Trim(),
									FolderIdKey = _folderAccountViewModel.SelectedItem.FolderIdKey,
									// Thêm các thuộc tính khác nếu cần thiết
								});
							}
							catch
							{
								// Xử lý lỗi nếu có
							}
						}
						break;

					case AddAccountType.UID_Pass_2FA_Cookie_Token:
						for (int i = 0; i < items.Count; i++)
						{
							try
							{
								var item = items[i].Split('|');

								listAccount.Add(new AccountModel
								{
									UID = item[0],
									Password = item[1].Trim(),
									C_2FA = item[2].Trim(),
									Cookie = item[3].Trim(),
									Token = item[4].Trim(),
									FolderIdKey = _folderAccountViewModel.SelectedItem.FolderIdKey,
									// Thêm các thuộc tính khác nếu cần thiết
								});
							}
							catch
							{
								// Xử lý lỗi nếu có
							}
						}
						break;

					case AddAccountType.UID_Pass_2FA_Cookie_Token_Email_PassEmail:
						for (int i = 0; i < items.Count; i++)
						{
							try
							{
								var item = items[i].Split('|');

								listAccount.Add(new AccountModel
								{
									UID = item[0],
									Password = item[1].Trim(),
									C_2FA = item[2].Trim(),
									Cookie = item[3].Trim(),
									Token = item[4].Trim(),
									Email1 = item[5].Trim(),
									Email1Password = item[6].Trim(),
									FolderIdKey = _folderAccountViewModel.SelectedItem.FolderIdKey,
									// Thêm các thuộc tính khác nếu cần thiết
								});
							}
							catch
							{
								// Xử lý lỗi nếu có
							}
						}
						break;

					default:
						MessageBox.Show("Có lỗi xảy ra");
						return;
				}

				foreach (var account in listAccount)
				{
					try
					{
						await _accountdataService.Create(account);
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
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (a) =>
			{
				var result = MessageBox.Show("Bạn có chắc muốn xóa tài khoản này?", "Cảnh Báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (result == MessageBoxResult.Yes)
				{
					foreach (var account in ItemsSelected)
					{
						try
						{
							await _accountdataService.Delete(account.AccountIDKey);
						}
						catch
						{

						}

					}
					LoadDataGridAccountCommand.Execute(this);
				}
			});
			#endregion

			#region SaveAccountCommand
			SaveAccountCommand = new RelayCommand<object>((a) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (a) =>
			{
				foreach (var accountModel in ItemsSelected)
				{
					try
					{

						await _accountdataService.Update(accountModel.AccountIDKey, accountModel);
					}
					catch
					{

					}

				}
				MessageBox.Show("Thao tác thành công");
			});
			#endregion

			#region OpenBrowserCommand
			OpenBrowserCommand = new RelayCommand<object>((a) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (a) =>
			{
				var parallelOptions = new ParallelOptions
				{
					MaxDegreeOfParallelism = ItemsSelected.Count // Sử dụng giá trị từ NumericUpDown
				};

				await Task.Run(() =>
				{
					Parallel.ForEach(AccountsSeleted, parallelOptions, async accountModel =>
					{
						string position;
						lock (lockPosition)
						{
							position = BrowserPositionHelper.GetNewPosition(800, 800, _scale);
						}
						var BrowserService = new BrowserService(accountModel, _accountdataService);

						try
						{
							accountModel.Driver = await BrowserService.OpenChromeGpm(_apiGPMUrl,
																   accountModel.GPMID, accountModel.UID,
																   accountModel.UserAgent, scale: _scale,
																   accountModel.Proxy, position: position);
						}
						catch
						{
							accountModel.Status = "Mở GPM lỗi!";
							accountModel.TextColor = SystemContants.RowColorFail;
						}
						// Khởi động ChromeDriver cho account


						if (accountModel.Driver == null)
						{
							accountModel.Status = "Mở GPM lỗi!";
						}

						//while(accountModel.Driver != null)
						//{
						//	accountModel.Status = "Đang mở trình duyệt!";
						//}
						//accountModel.Status = "Đã tắt trình duyệt!";
					});
				});
			});
			#endregion

			#region AddProxyCommand
			AddProxyCommand = new RelayCommand<object>((a) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
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
						await _accountdataService.Update(accountModel.AccountIDKey, accountModel);
					}
					MessageBox.Show("Thao tác thành công");
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
				ChangeFolderViewModel<FolderModel,
				AccountModel, IAccountDataService>(_folderAccountViewModel.Items,
												ItemsSelected,
												_accountdataService);

				// Gán DataContext của window cho ViewModel
				changeFolderWindow.DataContext = _changeFolderViewModel;

				// Mở cửa sổ ChangeFolderWindow dưới dạng dialog
				changeFolderWindow.ShowDialog();
			});
			#endregion

			#region GetAllPageCommand
			GetAllPageCommand = new RelayCommand<object>((a) =>
			{
				return ItemsSelected != null && ItemsSelected.Any() && _folderPageViewModel != null;
			},
			async (a) =>
			{
				//DataGridPageViewModel dataGridPageViewModel = new DataGridPageViewModel();
				try
				{
					_cancellationTokenSource = new CancellationTokenSource();
					_pauseEvent = new ManualResetEventSlim(true);


					// Sử dụng Parallel.ForEachAsync với giới hạn số luồng
					await Parallel.ForEachAsync(AccountsSeleted, new ParallelOptions
					{
						MaxDegreeOfParallelism = _propertyFileds.MaxParallelTasks, // Số luồng mong muốn
						CancellationToken = _cancellationTokenSource.Token
					}, async (accountModel, cancellationToken) =>
					{
						accountModel.TextColor = SystemContants.RowColorRunning;
						_cancellationTokenSource.Token.ThrowIfCancellationRequested();
						_pauseEvent.Wait();

						FbAccountAPI = new FacebookAccountAPI(accountModel,
							   _accountdataService, _pagedataService,
							   _folderAccountViewModel.SelectedItem,
							   _folderPageViewModel.SelectedItem);
						var result = await FbAccountAPI.GetAllPageRestSharp();
						if (result == null)
						{
							accountModel.TextColor = SystemContants.RowColorFail;
							return;
						}
						foreach (var item in result)
						{
							Application.Current.Dispatcher.Invoke(() =>
							{
								DataGridPageViewModel.Pages.Add(item);
							});
						}
					});
				}
				catch (OperationCanceledException)
				{
					App.Current.Dispatcher.Invoke(() =>
					{
						MessageBox.Show("Tiến trình đã bị hủy.");
					});
				}
				catch
				{

				}
				finally
				{
					IsRunning = false; // Đặt lại khi luồng hoàn thành
					_pauseEvent.Dispose();
					_pauseEvent = null;
					App.Current.Dispatcher.Invoke(() =>
					{
						MessageBox.Show("Tool đã dừng!");
					});
					try
					{

						App.Current.Dispatcher.Invoke(() =>
						{
							// Tạo một bản sao của danh sách trước khi lặp
							var accountsSnapshot = AccountsSeleted.ToList();

							foreach (var accountModel in accountsSnapshot)
							{
								accountModel.IsSelected = false;
							}
						});

					}
					catch
					{

					}

				}
			});
			#endregion

			#region CheckLiveUIDCommand
			CheckLiveUIDCommand = new RelayCommand<object>((a) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (a) =>
			{
				//DataGridPageViewModel dataGridPageViewModel = new DataGridPageViewModel();
				try
				{
					_cancellationTokenSource = new CancellationTokenSource();
					_pauseEvent = new ManualResetEventSlim(true);


					// Sử dụng Parallel.ForEachAsync với giới hạn số luồng
					await Parallel.ForEachAsync(AccountsSeleted, new ParallelOptions
					{
						MaxDegreeOfParallelism = _propertyFileds.MaxParallelTasks, // Số luồng mong muốn
						CancellationToken = _cancellationTokenSource.Token
					}, async (accountModel, cancellationToken) =>
					{
						accountModel.TextColor = SystemContants.RowColorRunning;
						_cancellationTokenSource.Token.ThrowIfCancellationRequested();
						_pauseEvent.Wait();

						FbAccountAPI = new FacebookAccountAPI(accountModel,
							   _accountdataService, _pagedataService,
							   _folderAccountViewModel.SelectedItem);
						var result = await FbAccountAPI.CheckLiveUid();
						await _accountdataService.Update(accountModel.AccountIDKey, accountModel);
						if (result == ResultModel.Success)
						{
							accountModel.TextColor = SystemContants.RowColorSuccess;
						}
						else
						{
							accountModel.TextColor = SystemContants.RowColorFail;
						}
					});
				}
				catch (OperationCanceledException)
				{
					App.Current.Dispatcher.Invoke(() =>
					{
						MessageBox.Show("Tiến trình đã bị hủy.");
					});
				}
				catch
				{

				}
				finally
				{
					IsRunning = false; // Đặt lại khi luồng hoàn thành
					_pauseEvent.Dispose();
					_pauseEvent = null;
					App.Current.Dispatcher.Invoke(() =>
					{
						MessageBox.Show("Tool đã dừng!");
					});
					try
					{

						App.Current.Dispatcher.Invoke(() =>
						{
							// Tạo một bản sao của danh sách trước khi lặp
							var accountsSnapshot = AccountsSeleted.ToList();

							foreach (var accountModel in accountsSnapshot)
							{
								accountModel.IsSelected = false;
							}
						});

					}
					catch
					{

					}

				}
			});
			#endregion

			#region CheckLiveCookieCommand
			CheckLiveCookieCommand = new RelayCommand<object>((a) =>
			{
				return ItemsSelected != null && ItemsSelected.Any();
			},
			async (a) =>
			{
				//DataGridPageViewModel dataGridPageViewModel = new DataGridPageViewModel();
				try
				{
					_cancellationTokenSource = new CancellationTokenSource();
					_pauseEvent = new ManualResetEventSlim(true);


					// Sử dụng Parallel.ForEachAsync với giới hạn số luồng
					await Parallel.ForEachAsync(AccountsSeleted, new ParallelOptions
					{
						MaxDegreeOfParallelism = _propertyFileds.MaxParallelTasks, // Số luồng mong muốn
						CancellationToken = _cancellationTokenSource.Token
					}, async (accountModel, cancellationToken) =>
					{
						accountModel.TextColor = SystemContants.RowColorRunning;
						_cancellationTokenSource.Token.ThrowIfCancellationRequested();
						_pauseEvent.Wait();

						FbAccountAPI = new FacebookAccountAPI(accountModel,
							   _accountdataService, _pagedataService,
							   _folderAccountViewModel.SelectedItem);
						var result = await FbAccountAPI.CheckLiveCookie();
						await _accountdataService.Update(accountModel.AccountIDKey, accountModel);
						if (result == ResultModel.Success)
						{
							accountModel.TextColor = SystemContants.RowColorSuccess;
						}
						else
						{
							accountModel.TextColor = SystemContants.RowColorFail;
						}
					});
				}
				catch (OperationCanceledException)
				{
					App.Current.Dispatcher.Invoke(() =>
					{
						MessageBox.Show("Tiến trình đã bị hủy.");
					});
				}
				catch
				{

				}
				finally
				{
					IsRunning = false; // Đặt lại khi luồng hoàn thành
					_pauseEvent.Dispose();
					_pauseEvent = null;
					App.Current.Dispatcher.Invoke(() =>
					{
						MessageBox.Show("Tool đã dừng!");
					});
					try
					{

						App.Current.Dispatcher.Invoke(() =>
						{
							// Tạo một bản sao của danh sách trước khi lặp
							var accountsSnapshot = AccountsSeleted.ToList();

							foreach (var accountModel in accountsSnapshot)
							{
								accountModel.IsSelected = false;
							}
						});

					}
					catch
					{

					}

				}
			});
			#endregion

			StopCommand = new RelayCommand<object>((a) =>
			{
				return true;
			},
			async (a) =>
			{
				//_cancellationTokenSource.Cancel();
				//_cancellationTokenSource.Dispose();
			});
		}
		private void OnGeneralSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(GeneralSettingsViewModel.Scale))
			{
				_scale = _generalSettings.Scale; // Cập nhật giá trị _scale
			}
			else if (e.PropertyName == nameof(GeneralSettingsViewModel.APIURL))
			{
				_apiGPMUrl = _generalSettings.APIURL; // Cập nhật giá trị _apiGPMUrl
			}
		}
		private void OnIPropertyFiledsChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(IPropertyFileds.MaxParallelTasks))
			{
				_scale = _generalSettings.Scale; // Cập nhật giá trị _scale
			}
			else if (e.PropertyName == nameof(GeneralSettingsViewModel.APIURL))
			{
				_apiGPMUrl = _generalSettings.APIURL; // Cập nhật giá trị _apiGPMUrl
			}
		}
	}
}
