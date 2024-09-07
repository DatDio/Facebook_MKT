using AutoMapper;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Facebook_MKT.WPF.Helppers;
using Facebook_MKT.WPF.Models;
using Facebook_MKT.WPF.ViewModels.Combobox;
using Facebook_MKT.WPF.ViewModels.DataGrid;
using Facebook_MKT.WPF.ViewModels.General_settings;
using Faceebook_MKT.Domain.BrowserController;
using Faceebook_MKT.Domain.Models;
using Faceebook_MKT.Domain.Services.BrowserService;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static Leaf.xNet.Services.Cloudflare.CloudflareBypass;

namespace Facebook_MKT.WPF.ViewModels.Accounts
{
	public class AccountInteractViewModel : BaseViewModel
	{
		private readonly GeneralSettingsViewModel _generalSettings;
		private int _maxParallelTasks = 5; // Giá trị mặc định
		public int MaxParallelTasks
		{
			get { return _maxParallelTasks; }
			set
			{
				_maxParallelTasks = value;
				OnPropertyChanged(nameof(MaxParallelTasks));
			}
		}

		public ObservableCollection<TaskModel> AccountTasks { get; set; }
		public ObservableCollection<TaskModel> TaskList { get; set; }

		public IDropTarget TaskDropHandler { get; }
		private List<string> _proxyList;

		#region Folder, Datagrid, BrowserService
		public FolderDataViewModel<Folder> FolderAccountsViewModel { get; }
		public DataGridAccountViewModel DataGridAccountViewModel { get; }
		private BrowserService BrowserService { get; set; }
		private FacebookBrowserController FBBrowserController { get; set; }
		#endregion

		public ICommand StartCommand { get; set; }
		public ICommand RemoveTaskCommand { get; }
		public ICommand BrowseCommand { get; set; }
		public ObservableCollection<AccountModel> AccountsSeleted { get; set; } = new ObservableCollection<AccountModel>();

		private readonly IDataService<Account> _accountDataService;
		private readonly IMapper _mapper;
		public AccountInteractViewModel(GeneralSettingsViewModel generalSettings,
							IDataService<Account> accountDataService,
							IDataService<Folder> folderAccountService,
							IDataService<FolderPage> folderPageService,
								//IEntityToModelConverter<Account, AccountModel> accountToModelConverter
								IMapper mapper
							)
		{
			AccountTasks = new ObservableCollection<TaskModel>();
			TaskList = new ObservableCollection<TaskModel>();
			// Thêm dữ liệu mẫu vào AccountTasks
			LoadInitialAccountTasks();

			TaskDropHandler = new TaskDragDropHandler(TaskList);

			#region Get Instances value
			_generalSettings = generalSettings;
			int _scale = _generalSettings.Scale;
			string _apiGPMUrl = _generalSettings.APIURL;
			_proxyList = new List<string>();
			_mapper = mapper;
			_accountDataService = accountDataService;
			#endregion

			#region Hiện Folder
			FolderAccountsViewModel = new FolderDataViewModel<Folder>(folderAccountService);
			FolderAccountsViewModel.FolderChanged += OnFolderChanged;
			#endregion

			DataGridAccountViewModel = new DataGridAccountViewModel(accountDataService,
										AccountsSeleted,
										//accountToModelConverter,
										_mapper,
										FolderAccountsViewModel);

			#region StartCommand
			StartCommand = new RelayCommand<object>((a) =>
			{
				return true;
			}, async (a) =>
			{
				var parallelOptions = new ParallelOptions
				{
					MaxDegreeOfParallelism = MaxParallelTasks // Sử dụng giá trị từ NumericUpDown
				};

				await Task.Run(() =>
				{
					Parallel.ForEach(AccountsSeleted, parallelOptions, async accountModel =>
					{
						string position = BrowserPositionHelper.GetNewPosition(800, 800, _scale);
						BrowserService = new BrowserService(accountModel, _accountDataService, _mapper);
						FBBrowserController = new FacebookBrowserController(accountModel, _accountDataService, _mapper);

						// Khởi động ChromeDriver cho account
						accountModel.Driver = await BrowserService.OpenChromeGpm(_apiGPMUrl,
																   accountModel.GPMID, accountModel.UID,
																   accountModel.UserAgent, scale: _scale,
																   accountModel.Proxy, position: position);

						if (accountModel.Driver == null)
						{
							accountModel.Status = "Mở GPM lỗi!";
						}

						foreach (var task in TaskList)
						{
							for(int i = 0; i <= 2; i++)
							{
								var logined = FBBrowserController.CheckLogined();
								if (logined != ResultModel.LoginFail)
								{
									break;
								}
								if (i == 2)
								{
									return;
								}
							}
							for (int i = 0; i <= 2; i++)
							{
								var switchProfile = FBBrowserController.SwitchToProfile();
								if (switchProfile != ResultModel.Fail)
								{
									break;
								}
								if (i == 2)
								{
									accountModel.Status = "Chuyển về profile thất bại!";
									var accountEntity = _mapper.Map<Account>(accountModel);
									await _accountDataService.Update(accountModel.AccountIDKey, accountEntity);
									return;
								}
							}
							
							var result = FBBrowserController.ExecuteTask(task, accountModel);
							// Dừng nếu gặp lỗi hoặc CheckPoint
							if (result == ResultModel.Fail || result == ResultModel.CheckPoint)
							{
								break;
							}
						}
					});
				});

				MessageBox.Show("Tool đã dừng!");
			});
			#endregion

			#region RemoveTaskCommand
			RemoveTaskCommand = new RelayCommand<TaskModel>((a) =>
			{
				return true;
			}, async (a) =>
			{
				if (a != null && TaskList.Contains(a))
				{
					TaskList.Remove(a);
				}
			});
			#endregion

			BrowseCommand = new RelayCommand<TaskField>((field) => true, 
				async (taskField) =>
			{
				//var dialog = new Microsoft.Win32.OpenFileDialog
				//{
				//	Filter = "All Files|*.*|Image Files|*.jpg;*.jpeg;*.png;*.bmp|Video Files|*.mp4;*.avi;*.mkv",
				//	Multiselect = true // Cho phép chọn nhiều file
				//};

				//bool? result = dialog.ShowDialog();

				//if (result == true)
				//{
				//	// Lưu danh sách file đã chọn vào thuộc tính Value của TaskField
				//	taskField.Value = string.Join(", ", dialog.FileNames);

				//	taskField.OnPropertyChanged(nameof(taskField.Value));
				//}

				var SetupPostWindow = new SetupPostWindow(taskField);
				bool? result = SetupPostWindow.ShowDialog();

				// Kiểm tra kết quả nếu cần
				if (result == true)
				{
					taskField.OnPropertyChanged(nameof(taskField.Value));
				}
			});


		}

		private void LoadInitialAccountTasks()
		{
			// Tạo danh sách TaskModel mới
			var newTasks = new List<TaskModel>
{
				new TaskModel
				{

					TaskName = "Đăng nhập"

				},
				new TaskModel
				{

					TaskName = "Nghỉ",
					Fields=
					{
						new TaskField("Từ", TaskFieldType.Number, 10),
						new TaskField("Đến", TaskFieldType.Number, 10),
					}
				},
				new TaskModel
				{

					TaskName = "Lướt New Feed",
					Fields =
					{
						new TaskField("Từ", TaskFieldType.Number, 10),
						new TaskField("Đến", TaskFieldType.Number, 10),
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
						TaskName = "Kết bạn theo từ khóa:",
						Fields =
						{
							new TaskField("", TaskFieldType.Text),
							//new TaskField("số người:", TaskFieldType.Label),
							new TaskField("Số người:",TaskFieldType.Number,5)
						}
					},
					new TaskModel
					{
						TaskName = "Đăng bài",
						 Fields =
					{
						new TaskField("Chọn ảnh và video:", TaskFieldType.File),
						new TaskField("Nội dung", TaskFieldType.MultiText, "")
					}
					},

					new TaskModel
					{
						TaskName = "Đăng video",
						Fields =
						{
							new TaskField("", TaskFieldType.Text, "")
						}
					},
					new TaskModel
					{
						TaskName = "Tham gia nhóm theo từ khóa:",
						Fields =
						{
							new TaskField("", TaskFieldType.Text, "")
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
	}
}
