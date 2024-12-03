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
using Faceebook_MKT.Domain.Models;
using Faceebook_MKT.Domain.Systems;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Facebook_MKT.WPF.Window.SetupPostWindow;
using Facebok_MKT.Service.DataService.Groups;
using Facebok_MKT.Service.DataService.Folders.FolderGroups;
using Faceebook_MKT.Domain.Helpers;
using Facebok_MKT.Service.Controller.FacebookAPIController;
namespace Facebook_MKT.WPF.ViewModels.Groups
{
	public class GroupViewModel : BaseViewModel
	{
		private ObservableCollection<string> _listKeyWordToScan = new ObservableCollection<string>();
		public ObservableCollection<string> ListKeyWordToScan
		{
			get => _listKeyWordToScan;
			set
			{
				_listKeyWordToScan = value;
				OnPropertyChanged(nameof(ListKeyWordToScan));
				UpdateTextFromCollection();
			}
		}
		private bool _scanGroup;
		public bool ScanGroup
		{
			get
			{
				return _scanGroup;
			}
			set
			{
				_scanGroup = value;
				OnPropertyChanged(nameof(ScanGroup));
			}
		}
		private string _listKeyWordToScanText;
		public string ListKeyWordToScanText
		{
			get { return _listKeyWordToScanText; }
			set
			{
				_listKeyWordToScanText = value;
				OnPropertyChanged(nameof(ListKeyWordToScanText));
				UpdateCollectionFromText();
			}
		}
		private object _lockPosition;
		public ObservableCollection<TaskModel> TaskList { get; set; }
		public ObservableCollection<TaskModel> GroupTasks { get; set; }
		public IDropTarget TaskDropHandler { get; }

		#region Folder, Datagrid, BrowserService
		public FolderAccountViewModel FolderAccountsViewModel { get; }
		public FolderGroupViewModel FolderGroupViewModel { get; }
		public FolderPageViewModel FolderPageViewModel { get; }
		public DataGridAccountViewModel DataGridAccountViewModel { get; }
		public DataGridGroupViewModel DataGridGroupViewModel { get; }
		public FacebookGroupAPI FBGroupAPI { get; set; }
		private BrowserService BrowserService { get; set; }
		private FacebookBrowserPageController FBBrowserController { get; set; }
		#endregion

		public ICommand StartCommand { get; set; }
		public ICommand BrowseCommand { get; set; }
		public ICommand RemoveTaskCommand { get; set; }
		public ICommand ScanGroupCommand { get; set; }

		public ObservableCollection<AccountModel> AccountsSeleted { get; set; } = new ObservableCollection<AccountModel>();
		public ObservableCollection<GroupModel> GroupsSeleted { get; set; } = new ObservableCollection<GroupModel>();

		private GeneralSettingsViewModel _generalSettings;
		private readonly IAccountDataService _accountDataService;
		private readonly IGroupDataService _groupDataService;
		private readonly IPageDataService _pageDataService;
		//private readonly IMapper _mapper;
		public GroupViewModel(GeneralSettingsViewModel generalSettings,
							IAccountDataService accountDataService,
							IGroupDataService groupDataService,
							IFolderDataService folderAccountService,
							IFolderGroupDataService folderGroupService)
		{
			_lockPosition = new object();
			GroupTasks = new ObservableCollection<TaskModel>();
			TaskList = new ObservableCollection<TaskModel>();
			TaskDropHandler = new TaskDragDropHandler(TaskList);

			_generalSettings = generalSettings;
			_generalSettings.PropertyChanged += OnGeneralSettingsChanged;
			_scale = _generalSettings.Scale;
			_apiGPMUrl = _generalSettings.APIURL;
			//_proxyList = new List<string>();
			_accountDataService = accountDataService;
			_groupDataService = groupDataService;
			#region Hiện Folder
			FolderAccountsViewModel = new FolderAccountViewModel(folderAccountService);
			FolderGroupViewModel = new FolderGroupViewModel(folderGroupService);
			FolderAccountsViewModel.FolderChanged += OnFolderAccountChanged;
			FolderGroupViewModel.FolderChanged += OnFolderGroupChanged;
			#endregion

			DataGridGroupViewModel = new DataGridGroupViewModel(_groupDataService,
										GroupsSeleted,
										FolderGroupViewModel);

			DataGridAccountViewModel = new DataGridAccountViewModel(
										accountDataService,
										_pageDataService,
										AccountsSeleted,
										FolderAccountsViewModel,
										FolderPageViewModel,
										_generalSettings,
										this);

			LoadInitialGroupTasks();

			#region StartCommand
			StartCommand = new RelayCommand<object>((a) =>
			{
				if (TaskList != null && TaskList.Count > 0
					&& GroupsSeleted.Count > 0)
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
				if (_pauseEvent != null && GroupsSeleted.Count > 0)
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

			#region ScanGroupCommand
			ScanGroupCommand = new RelayCommand<TaskModel>((a) =>
			{
				if (AccountsSeleted.Count <= 0) return false;
				return true;
			}, async (a) =>
			{
				_cancellationTokenSource = new CancellationTokenSource();
				_pauseEvent = new ManualResetEventSlim(true);
				UpdateCollectionFromText();
				List<Task> tasks = new List<Task>();
				for (int i = 0; i < MaxParallelTasks; i++)
				{
					Task task = Task.Run(() => OneThreadScanGroupAPI(_cancellationTokenSource.Token));
					tasks.Add(task);
				}

				await Task.WhenAll(tasks);
				_pauseEvent.Dispose();
				_pauseEvent = null;
				App.Current.Dispatcher.Invoke(() =>
				{
					MessageBox.Show("Đã quét xong!");
				});
			});
			#endregion
		}

		private async Task OneThread(CancellationToken token)
		{
			var taskListForThread = new ObservableCollection<TaskModel>(TaskList);
			string position = "";
			AccountModel accountModel = null;
			GroupModel groupModel = null;
			ResultModel result;
			BrowserService browserService = null;
			lock (_lockPosition)
			{
				position = BrowserPositionHelper.GetNewPosition(800, 800, _scale);
			}

			try
			{
				while (!_cancellationTokenSource.Token.IsCancellationRequested)
				{
					lock (AccountsSeleted)
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
					FacebookBrowserGroupController(accountModel,
					_accountDataService, _groupDataService,
					_pauseEvent, _cancellationTokenSource, null);

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
					for (int i = 0; i <= 5; i++)
					{
						lock (GroupsSeleted)
						{
							if (GroupsSeleted.Count == 0)
							{
								break;
							}
							groupModel = GroupsSeleted[0];
							GroupsSeleted.RemoveAt(0);
						}
						groupModel.TextColor = SystemContants.RowColorRunning;
						fbBrowserController = new FacebookBrowserGroupController(accountModel,
										_accountDataService, _groupDataService,
										_pauseEvent, _cancellationTokenSource, groupModel);
						ResultStatus = await fbBrowserController.CheckLoginedAndGotoGroup();

						if (ResultStatus == ResultModel.Fail || ResultStatus == ResultModel.CheckPoint)
						{
							accountModel.IsSelected = false;
							accountModel.TextColor = SystemContants.RowColorFail;
							groupModel.TextColor = SystemContants.RowColorFail;
							break;
						}
						foreach (var task in taskListForThread)
						{
							_pauseEvent.Wait();
							_cancellationTokenSource.Token.ThrowIfCancellationRequested();

							

							result = await fbBrowserController.ExecuteTask(task);

							if (result == ResultModel.CheckPoint)
							{
								accountModel.IsSelected = false;
								accountModel.TextColor = SystemContants.RowColorFail;
								groupModel.TextColor = SystemContants.RowColorFail;
								break;
							}
							else
							{
								accountModel.TextColor = SystemContants.RowColorSuccess;
								groupModel.TextColor = SystemContants.RowColorSuccess;
							}

						}
						groupModel.IsSelected = false;
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

		private async Task OneThreadScanGroupAPI(CancellationToken token)
		{
			AccountModel accountModel = null;
			BrowserService browserService = null;
			string keyWord = "";
			try
			{
				while (!_cancellationTokenSource.Token.IsCancellationRequested)
				{
					lock (AccountsSeleted)
					{
						if (AccountsSeleted.Count == 0)
						{
							break;
						}
						accountModel = AccountsSeleted[0];
						AccountsSeleted.RemoveAt(0);
					}
					lock (ListKeyWordToScan)
					{
						if (ListKeyWordToScan.Count == 0)
						{
							break;
						}
						keyWord = ListKeyWordToScan[0];
						ListKeyWordToScan.RemoveAt(0);
					}
					accountModel.TextColor = SystemContants.RowColorRunning;
					FBGroupAPI = new FacebookGroupAPI(accountModel, _accountDataService, null,
						null, null, _groupDataService, FolderGroupViewModel.SelectedItem);
					FBGroupAPI.OnGroupFound += OnGroupFoundHandler;
					await FBGroupAPI.ScanGroups(keyWord);
				}
			finish:
				accountModel.TextColor = SystemContants.RowColorSuccess;
				accountModel.IsSelected = false;
			}
			catch (OperationCanceledException)
			{
				// Xử lý khi có yêu cầu hủy tác vụ
				//Console.WriteLine("Tác vụ đã bị hủy.");
			}
			catch
			{

			}

		}
		private void OnGroupFoundHandler(GroupModel groupModel)
		{
			// Cập nhật DataGrid trên luồng UI
			App.Current.Dispatcher.Invoke(() =>
			{
				DataGridGroupViewModel.Groups.Add(groupModel);
			});
		}
		private async void OnFolderAccountChanged(int folderIdKey)
		{
			//var sgdsgf = AccountsSeleted;
			// Thực hiện lệnh LoadDataToDataGridCommand với FolderIdKey
			DataGridAccountViewModel.LoadDataGridAccountCommand.Execute(folderIdKey);

		}

		private async void OnFolderGroupChanged(int folderIdKey)
		{
			// Thực hiện lệnh LoadDataToDataGridCommand với FolderIdKey
			DataGridGroupViewModel.LoadDataGridGroupCommand.Execute(folderIdKey);
		}

		private void LoadInitialGroupTasks()
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
					TaskName = "Đọc tin nhắn",
				},
				new TaskModel
				{

					TaskName = "Đọc thông báo",
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
						TaskName = "Tham gia nhóm theo groupID",
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
				GroupTasks.Add(task);
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
		private void UpdateCollectionFromText()
		{
			if (!string.IsNullOrWhiteSpace(ListKeyWordToScanText))
			{
				var keywords = ListKeyWordToScanText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				ListKeyWordToScan.Clear();
				foreach (var keyword in keywords)
				{
					ListKeyWordToScan.Add(keyword.Trim());
				}
			}
		}
		private void UpdateTextFromCollection()
		{
			ListKeyWordToScanText = string.Join("\n", ListKeyWordToScan);
		}
	}
}
