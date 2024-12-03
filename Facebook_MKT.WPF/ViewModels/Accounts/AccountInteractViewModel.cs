using AutoMapper;
using Facebok_MKT.Service.BrowserService;
using Facebok_MKT.Service.Controller.BrowserController;
using Facebok_MKT.Service.DataService.Accounts;
using Facebok_MKT.Service.DataService.Folders.FolderAccounts;
using Facebok_MKT.Service.DataService.Folders.FolderPages;
using Facebok_MKT.Service.DataService.Pages;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.WPF.Helppers;
using Facebook_MKT.WPF.Models;
using Facebook_MKT.WPF.ViewModels.Combobox;
using Facebook_MKT.WPF.ViewModels.DataGrid;
using Facebook_MKT.WPF.ViewModels.General_settings;
using Facebook_MKT.WPF.Window.SetupPostWindow;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using Faceebook_MKT.Domain.Systems;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using static Leaf.xNet.Services.Cloudflare.CloudflareBypass;

namespace Facebook_MKT.WPF.ViewModels.Accounts
{
	public class AccountInteractViewModel : BaseViewModel
	{
		private readonly GeneralSettingsViewModel _generalSettings;
		//public ObservableCollection<MediaFileModel> MediaFiles { get; set; }
		public ObservableCollection<TaskModel> AccountTasks { get; set; }
		public ObservableCollection<TaskModel> TaskList { get; set; }

		public IDropTarget TaskDropHandler { get; }
		private List<string> _proxyList;

		#region Folder, Datagrid, BrowserService
		public FolderAccountViewModel FolderAccountsViewModel { get; }
		public FolderPageViewModel FolderPageViewModel { get; }
		public DataGridAccountViewModel DataGridAccountViewModel { get; }
		private BrowserService BrowserService { get; set; }

		#endregion

		public ICommand StartCommand { get; set; }
		public ICommand RemoveTaskCommand { get; }
		public ICommand BrowseCommand { get; set; }
		public ObservableCollection<AccountModel> AccountsSeleted { get; set; } = new ObservableCollection<AccountModel>();

		private readonly IAccountDataService _accountDataService;
		private readonly IPageDataService _pageDataService;
		private object _lockPosition = new object();
		private object _lockAccountModel = new object();
		public AccountInteractViewModel(GeneralSettingsViewModel generalSettings,
							IAccountDataService accountDataService,
							IPageDataService pageDataService,
							IFolderDataService folderAccountDataService,
						IFolderPageDataService folderPageDataService
							)
		{
			AccountTasks = new ObservableCollection<TaskModel>();
			TaskList = new ObservableCollection<TaskModel>();
			// Thêm dữ liệu mẫu vào AccountTasks
			LoadInitialAccountTasks();

			TaskDropHandler = new TaskDragDropHandler(TaskList);

			#region Get Instances value
			_generalSettings = generalSettings;
			_scale = _generalSettings.Scale;
			_apiGPMUrl = _generalSettings.APIURL;
			_generalSettings.PropertyChanged += OnGeneralSettingsChanged;
			_proxyList = new List<string>();
			_accountDataService = accountDataService;
			_pageDataService = pageDataService;
			#endregion

			#region Hiện Folder
			FolderAccountsViewModel = new FolderAccountViewModel(folderAccountDataService);
			FolderAccountsViewModel.FolderChanged += OnFolderChanged;
			#endregion

			DataGridAccountViewModel = new DataGridAccountViewModel(accountDataService,
										_pageDataService,
										AccountsSeleted,
										FolderAccountsViewModel,
										FolderPageViewModel,
										_generalSettings,
										this);

			#region StartCommand
			StartCommand = new RelayCommand<object>((a) =>
			{
				if (TaskList != null && TaskList.Count > 0
				&& AccountsSeleted.Count > 0)
					return true;
				return false;
			}, async (a) =>
			{
				IsRunning = true;
				_cancellationTokenSource = new CancellationTokenSource();
				_pauseEvent = new ManualResetEventSlim(true);
				List<Task> tasks = new List<Task>();
				for (int i = 0; i < MaxParallelTasks; i++)
				{
					Task task = Task.Run(() => OneThread(_cancellationTokenSource.Token));
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
				return;



			});

			#endregion

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

			StopCommand = new RelayCommand<object>((a) =>
			{
				return IsRunning;
			},
			async (a) =>
			{
				_cancellationTokenSource.Cancel();
				//_cancellationTokenSource.Dispose();
			});

			#region PauseCommand
			PauseCommand = new RelayCommand<object>((a) =>
			{
				if (_pauseEvent != null)
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

		}
		private async Task OneThread(CancellationToken token)
		{
			var taskListForThread = new ObservableCollection<TaskModel>(TaskList);
			string position = "";
			AccountModel accountModel = null;
			BrowserService browserService = null;
			lock (_lockPosition)
			{
				position = BrowserPositionHelper.GetNewPosition(800, 800, _scale);
			}

			try
			{
				while (!_cancellationTokenSource.Token.IsCancellationRequested)
				{
					lock (_lockAccountModel)
					{
						if (AccountsSeleted.Count == 0)
						{
							break;
						}
						accountModel = AccountsSeleted[0];
						AccountsSeleted.RemoveAt(0);
					}
					browserService = new BrowserService(accountModel, _accountDataService);
					accountModel.TextColor = SystemContants.RowColorRunning;
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
						accountModel.IsSelected = false;
						accountModel.TextColor = SystemContants.RowColorFail;
						continue;
					}
					_pauseEvent.Wait();
					_cancellationTokenSource.Token.ThrowIfCancellationRequested();

					var fbBrowserController = new
					FacebookBrowserAccountController(accountModel,
					_accountDataService,
					_pauseEvent, _cancellationTokenSource);


					var ResultStatus = await fbBrowserController.Initialization();

					if (ResultStatus == ResultModel.Fail)
					{
						accountModel.IsSelected = false;
						accountModel.TextColor = SystemContants.RowColorFail;
						continue;
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

						if (ResultStatus == ResultModel.Fail || ResultStatus == ResultModel.CheckPoint)
						{
							accountModel.IsSelected = false;
							accountModel.TextColor = SystemContants.RowColorFail;
							break;
						}

						var result = await fbBrowserController.ExecuteTask(task);

						if (result == ResultModel.CheckPoint)
						{
							accountModel.IsSelected = false;
							accountModel.TextColor = SystemContants.RowColorFail;
							break;
						}
						else
						{
							accountModel.TextColor = SystemContants.RowColorSuccess;

						}

					}
					accountModel.IsSelected = false;
					browserService.CloseChrome();
				}

			}
			catch (OperationCanceledException)
			{
				// Xử lý khi có yêu cầu hủy tác vụ
				//Console.WriteLine("Tác vụ đã bị hủy.");
			}
			catch
			{

			}
			try
			{
				//accountModel.IsSelected = false;
				browserService.CloseChrome();
			}
			catch
			{

			}

		}

		private void LoadInitialAccountTasks()
		{
			// Tạo danh sách TaskModel mới
			var newTasks = new List<TaskModel>
{
				//new TaskModel
				//{

				//	TaskName = "Đăng nhập"

				//},

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
				new TaskModel
				{
					TaskName = "Đọc tin nhắn",
				},
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
						TaskName = "Kết bạn theo gợi ý",
						Fields =
						{
							new TaskField("Số người:",TaskFieldType.Number,5)
						}
					},
					new TaskModel
					{
						TaskName = "Chấp nhận lời mời kết bạn",
						Fields =
						{
							new TaskField("Số người:",TaskFieldType.Number,5)
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
						TaskName = "Tham gia nhóm theo từ khóa",
						Fields =
						{
							new TaskField("", TaskFieldType.Text, ""),
							new TaskField("Số nhóm:",TaskFieldType.Number,5)
						}
					},
};
			int Index = 1;
			// Thêm danh sách TaskModel vào AccountTasks
			foreach (var task in newTasks)
			{
				task.Index = Index++;
				AccountTasks.Add(task);
			}

		}


		private async void OnFolderChanged(int folderIdKey)
		{
			var sgdsgf = AccountsSeleted;
			// Thực hiện lệnh LoadDataToDataGridCommand với FolderIdKey
			DataGridAccountViewModel.LoadDataGridAccountCommand.Execute(folderIdKey);
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
