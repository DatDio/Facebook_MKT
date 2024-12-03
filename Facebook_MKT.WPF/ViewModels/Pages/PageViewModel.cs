using AutoMapper;
using Facebok_MKT.Service.BrowserService;
using Facebok_MKT.Service.Controller.BrowserController;
using Facebok_MKT.Service.DataService.Accounts;
using Facebok_MKT.Service.DataService.Folders.FolderAccounts;
using Facebok_MKT.Service.DataService.Folders.FolderPages;
using Facebok_MKT.Service.DataService.Pages;
using Facebook_MKT.WPF.Helppers;
using Facebook_MKT.WPF.ViewModels.Combobox;
using Facebook_MKT.WPF.ViewModels.DataGrid;
using Facebook_MKT.WPF.ViewModels.General_settings;
using Faceebook_MKT.Domain.AnditectBrowserController;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using Faceebook_MKT.Domain.Systems;
using GongSolutions.Wpf.DragDrop;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using Facebook_MKT.WPF.Window.SetupPostWindow;
namespace Facebook_MKT.WPF.ViewModels.Pages
{
	public enum AddAccountType
	{
		UID_Pass_2FA,
		UID_Pass_2FA_Cookie,
		UID_Pass_2FA_Cookie_Token,
		UID_Pass_2FA_Cookie_Token_Email_PassEmail,
	}
	public class PageViewModel : BaseViewModel
	{
		ConcurrentQueue<(int AccountIDKey, List<PageModel> Pages)> _pagesQueue;
		private object _lockPosition;
		public ObservableCollection<TaskModel> TaskList { get; set; }
		public ObservableCollection<TaskModel> PageTasks { get; set; }
		public IDropTarget TaskDropHandler { get; }
		//private List<string> _proxyList;

		#region Folder, Datagrid, BrowserService
		public FolderAccountViewModel FolderAccountsViewModel { get; }
		public FolderPageViewModel FolderPagesViewModel { get; }
		public DataGridAccountViewModel DataGridAccountViewModel { get; }
		public DataGridPageViewModel DataGridPageViewModel { get; }
		private BrowserService BrowserService { get; set; }
		private FacebookBrowserPageController FBBrowserController { get; set; }
		#endregion

		public ICommand StartCommand { get; set; }
		//BrowseCommand là command của task script
		public ICommand BrowseCommand { get; set; }
		public ICommand SetUpPostApiCommand { get; set; }
		public ICommand RemoveTaskCommand { get; set; }
		//public ICommand StopCommand { get; set; }
		//public ICommand PauseCommand { get; set; }
		public ObservableCollection<AccountModel> AllAccountModels { get; set; } = new ObservableCollection<AccountModel>();
		public ObservableCollection<AccountModel> AccountsSeleted { get; set; } = new ObservableCollection<AccountModel>();
		public ObservableCollection<PageModel> PagesSeleted { get; set; } = new ObservableCollection<PageModel>();

		private GeneralSettingsViewModel _generalSettings;
		private readonly IAccountDataService _accountDataService;
		private readonly IPageDataService _pageDataService;
		//private readonly IMapper _mapper;
		public PageViewModel(GeneralSettingsViewModel generalSettings,
							IAccountDataService accountDataService,
							IPageDataService pageDataService,
							IFolderDataService folderAccountService,
							IFolderPageDataService folderPageService)
		{
			_lockPosition = new object();
			PageTasks = new ObservableCollection<TaskModel>();
			TaskList = new ObservableCollection<TaskModel>();
			TaskDropHandler = new TaskDragDropHandler(TaskList);

			_generalSettings = generalSettings;
			_generalSettings.PropertyChanged += OnGeneralSettingsChanged;
			_scale = _generalSettings.Scale;
			_apiGPMUrl = _generalSettings.APIURL;
			//_proxyList = new List<string>();
			_accountDataService = accountDataService;
			_pageDataService = pageDataService;
			#region Hiện Folder
			FolderAccountsViewModel = new FolderAccountViewModel(folderAccountService);
			FolderPagesViewModel = new FolderPageViewModel(folderPageService);
			FolderAccountsViewModel.FolderChanged += OnFolderAccountChanged;
			FolderPagesViewModel.FolderChanged += OnFolderPageChanged;
			#endregion

			DataGridPageViewModel = new DataGridPageViewModel(_pageDataService,
										PagesSeleted,
										FolderPagesViewModel);

			DataGridAccountViewModel = new DataGridAccountViewModel(
										accountDataService,
										_pageDataService,
										AccountsSeleted,
										FolderAccountsViewModel,
										FolderPagesViewModel,
										_generalSettings,
										this);

			LoadInitialPageTasks();

			#region StartCommand
			StartCommand = new RelayCommand<object>((a) =>
			{
				if (TaskList != null && TaskList.Count > 0
					&& PagesSeleted.Count > 0)
					return true;
				if (IsRunning == true) return false;
				return false;
			}, async (a) =>
			{
				IsRunning = true;

				// Tải tất cả các account một lần trước khi bắt đầu các luồng
				LoadAllAccounts();

				var allAccountModels = new Dictionary<int, AccountModel>();

				foreach (var acc in AllAccountModels)
				{
					if (!allAccountModels.ContainsKey(acc.AccountIDKey))
					{
						allAccountModels.Add(acc.AccountIDKey, acc);
					}
				}

				_cancellationTokenSource = new CancellationTokenSource();
				_pauseEvent = new ManualResetEventSlim(true);

				// Tạo queue để chia đều các pagesGroupedByAccount cho các luồng
				_pagesQueue = new ConcurrentQueue<(int AccountIDKey, List<PageModel>)>(
				PagesSeleted.GroupBy(page => page.AccountIDKey)
				.Select(group => (AccountIDKey: group.Key, Pages: group.ToList())));

				List<Task> tasks = new List<Task>();
				for (int i = 0; i < MaxParallelTasks; i++)
				{
					Task task = Task.Run(() => OneThread(_cancellationTokenSource.Token, allAccountModels));
					tasks.Add(task);
				}

				await Task.WhenAll(tasks);
				IsRunning = false;
				_pauseEvent.Dispose();
				_pauseEvent = null;
				App.Current.Dispatcher.Invoke(() =>
				{
					MessageBox.Show("Tool đã dừng!");
				});

			});
			#endregion

			#region StopCommand
			StopCommand = new RelayCommand<object>((a) =>
			{
				return IsRunning;
			},
			async (a) =>
			{
				_cancellationTokenSource.Cancel();
				//_cancellationTokenSource.Dispose();
			});
			#endregion

			#region PauseCommand
			PauseCommand = new RelayCommand<object>((a) =>
			{
				if (_pauseEvent != null && PagesSeleted.Count > 0)
					return true; // Chỉ có thể tạm dừng khi có luồng đang chạy
				else
					return false;
			},
			(a) =>
			{
				if (IsRunning)
				{
					_pauseEvent.Reset();  // Tạm dừng các luồng
					IsRunning = false;
				}
				else
				{
					_pauseEvent.Set();  // Tiếp tục các luồng
					IsRunning = true;      // Đổi trạng thái thành "Resume"
				}
			});
			#endregion

			#region SetUpPost
			BrowseCommand = new RelayCommand<object>((taskModel) => true,
			async (taskModel) =>
			{
				var t = TaskList;
				var taskmodel = taskModel as TaskModel;

				var SetupPostWindow = new SetupPostWindow();
				var SetupPostViewModel = new SetupPostViewModel(taskmodel);
				SetupPostWindow.DataContext = SetupPostViewModel;
				bool? result = SetupPostWindow.ShowDialog();

				// Kiểm tra kết quả nếu cần
				//if (result == true)
				//{

				//}
			});
			#endregion

			SetUpPostApiCommand = new RelayCommand<object>((taskModel) => true,
			async (taskModel) =>
			{
				var t = TaskList;
				var taskmodel = new TaskModel
				{
					TaskName = "Đăng bài",
					Fields = new List<TaskField>
					{
						new TaskField("",TaskFieldType.Media),
						new TaskField("", TaskFieldType.Label, ""),
						new TaskField("", TaskFieldType.File),
					}
				};

				var SetupPostWindow = new SetupPostWindow();
				var SetupPostViewModel = new SetupPostViewModel(taskmodel);
				SetupPostWindow.DataContext = SetupPostViewModel;
				bool? result = SetupPostWindow.ShowDialog();

				// Kiểm tra kết quả nếu cần
				//if (result == true)
				//{

				//}
				var tsg = TaskList;
			});

			#region RemoveTaskCommand
			RemoveTaskCommand = new RelayCommand<TaskModel>((a) =>
			{
				if (IsRunning) return false;
				return true;
			}, async (a) =>
			{
				if (a != null && TaskList.Contains(a))
				{
					TaskList.Remove(a);
				}
			});
			#endregion
		}

		private async Task OneThread(CancellationToken token,
			Dictionary<int, AccountModel> allAccountModels)
		{
			// Tạo bản sao của TaskList cho luồng này
			var taskListForThread = new ObservableCollection<TaskModel>(TaskList);
			string position = "";
			AccountModel accountModel = null;
			FacebookBrowserPageController fbBrowserController = null;
			BrowserService browserService = null;
			lock (_lockPosition)
			{
				position = BrowserPositionHelper.GetNewPosition(800, 800, _scale);
			}

			try
			{
				while (!_cancellationTokenSource.Token.IsCancellationRequested)
				{

					if (_pagesQueue.TryDequeue(out var accountGroup))
					{
						var accountIDKey = accountGroup.AccountIDKey;

						// Tìm AccountModel tương ứng từ danh sách allAccountModels đã tải trước đó
						if (!allAccountModels.TryGetValue(accountIDKey, out accountModel))
						{
							Console.WriteLine($"Account với ID {accountIDKey} không tồn tại.");
							continue;
						}
						browserService = new BrowserService(accountModel, _accountDataService);


						accountModel.Status = "Đang mở trình duyệt...";

						try
						{

							accountModel.Driver = await browserService.OpenChromeGpm(
														_apiGPMUrl,
														accountModel.GPMID, accountModel.UID,
														accountModel.UserAgent, scale: _scale,
														accountModel.Proxy, position: position);
						}
						catch
						{

							accountModel.Status = "Mở GPM lỗi!";
							foreach (var pageModel in accountGroup.Pages)
							{
								pageModel.IsSelected = false;
								pageModel.PageStatus = "Mở GPM lỗi!";
								pageModel.TextColor = SystemContants.RowColorFail;
							}
							continue;
						}

						// Xử lý từng trang liên quan đến account
						foreach (var pageModel in accountGroup.Pages)
						{
							pageModel.TextColor = SystemContants.RowColorRunning;
							_pauseEvent.Wait();
							_cancellationTokenSource.Token.ThrowIfCancellationRequested();

							// Khởi tạo FacebookBrowserController cho mỗi trang
							fbBrowserController = new FacebookBrowserPageController(
								accountModel, _accountDataService, _pageDataService,
								_pauseEvent, _cancellationTokenSource, pageModel
							);

							var ResultStatus = await fbBrowserController.Initialization();

							if (ResultStatus == ResultModel.Fail)
							{
								pageModel.IsSelected = false;
								pageModel.TextColor = SystemContants.RowColorFail;
								continue;
							}
							else if (ResultStatus == ResultModel.CheckPoint)
							{
								pageModel.IsSelected = false;
								pageModel.TextColor = SystemContants.RowColorFail;
								break;
							}

							if (RandomTaskInTaskList)
							{
								FunctionHelper.ShuffleTaskList(taskListForThread);
							}
							foreach (var task in taskListForThread)
							{
								_pauseEvent.Wait();
								_cancellationTokenSource.Token.ThrowIfCancellationRequested();

								ResultStatus = await fbBrowserController.CheckLogined();

								if (ResultStatus == ResultModel.Fail)
								{
									pageModel.IsSelected = false;
									pageModel.TextColor = SystemContants.RowColorFail;
									break;
								}

								try
								{
									var result = await fbBrowserController.ExecuteTask(task);

									if (result == ResultModel.CheckPoint)
									{
										pageModel.IsSelected = false;
										pageModel.TextColor = SystemContants.RowColorFail;
										break;
									}
								}
								catch
								{

								}
								
							}
							pageModel.IsSelected = false;
							pageModel.TextColor = SystemContants.RowColorSuccess;
						}
						browserService.CloseChrome();
					}
					else
					{
						// Không còn account nào để xử lý, thoát khỏi vòng lặp
						break;
					}

				}
			}
			//catch (OperationCanceledException)
			//{

			//}
			catch (Exception ex)
			{
				browserService.CloseChrome();
				//MessageBox.Show(ex.Message);
				Debug.WriteLine(ex.Message);
			}
		}


		private async void OnFolderAccountChanged(int folderIdKey)
		{
			//var sgdsgf = AccountsSeleted;
			// Thực hiện lệnh LoadDataToDataGridCommand với FolderIdKey
			DataGridAccountViewModel.LoadDataGridAccountCommand.Execute(folderIdKey);

		}
		private async void OnFolderPageChanged(int folderIdKey)
		{
			// Thực hiện lệnh LoadDataToDataGridCommand với FolderIdKey
			DataGridPageViewModel.LoadDataGridPageCommand.Execute(folderIdKey);
		}
		private async void LoadAllAccounts()
		{
			AllAccountModels.Clear();
			try
			{
				var listAccountModels = await _accountDataService.GetAll();
				// Map entities to models
				foreach (var accountModel in listAccountModels)
				{
					AllAccountModels.Add(accountModel);
				}
			}
			catch { }
		}
		private void LoadInitialPageTasks()
		{
			var newTasks = new List<TaskModel>
{
				new TaskModel
				{

					TaskName = "Lướt New Feed",
					Fields =
					{
						new TaskField("Từ", TaskFieldType.Number, 100),
						new TaskField("Đến", TaskFieldType.Number, 200),
						new TaskField("s", TaskFieldType.Label)
					}
				},
				new TaskModel
				{

					TaskName = "Nghỉ",
					Fields=
					{
						new TaskField("Từ", TaskFieldType.Number, 10),
						new TaskField("Đến", TaskFieldType.Number, 20),
						new TaskField("s", TaskFieldType.Label)
					}
				},
				//new TaskModel
				//{
				//	TaskName = "Đọc tin nhắn",
				//},
				new TaskModel
				{

					TaskName = "Đọc thông báo",
				},
					new TaskModel
					{
						TaskName = "Like",
						Fields =
						{
							new TaskField("Từ", TaskFieldType.Number, 20),
							new TaskField("Đến", TaskFieldType.Number, 20),
						}
					},
					new TaskModel
					{
						TaskName = "Comment",
						Fields =
						{
							new TaskField("Từ", TaskFieldType.Number, 10),
							new TaskField("Đến", TaskFieldType.Number, 10),
							new TaskField("comment",TaskFieldType.Label)
						}
					},
					new TaskModel
					{
						TaskName = "Đăng bài",
						 Fields = new List<TaskField>
					{
						new TaskField("",TaskFieldType.Media),
						new TaskField("", TaskFieldType.Label, ""),
						new TaskField("", TaskFieldType.File),
					},
					},
					new TaskModel
					{
						TaskName = "Đăng reel",
						 Fields = new List<TaskField>
					{
							new TaskField("Tittle", TaskFieldType.Text, ""),
							new TaskField("Comment", TaskFieldType.Text, ""),
					},
					},
					new TaskModel
					{
						TaskName = "Tham gia nhóm theo từ khóa",
						Fields =
						{
							new TaskField("", TaskFieldType.Text, ""),
							new TaskField("Số nhóm:",TaskFieldType.Number,5)
						}
					},
};
			int Index = 1;
			foreach (var task in newTasks)
			{
				task.Index = Index++;
				PageTasks.Add(task);
			}

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


	}
}
