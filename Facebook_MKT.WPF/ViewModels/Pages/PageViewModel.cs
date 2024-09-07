using AutoMapper;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Facebook_MKT.WPF.Commands;
using Facebook_MKT.WPF.Commands.LoadDataGrid;
using Facebook_MKT.WPF.Helppers;
using Facebook_MKT.WPF.ViewModels.Combobox;
using Facebook_MKT.WPF.ViewModels.DataGrid;
using Facebook_MKT.WPF.ViewModels.General_settings;
using Faceebook_MKT.Domain.Helpers.ConvertToModel;
using Faceebook_MKT.Domain.Models;
using Faceebook_MKT.Domain.Services.BrowserService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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


		private List<string> _proxyList;

		#region Folder, Datagrid, BrowserService
		public FolderDataViewModel<Folder> FolderAccountsViewModel { get; }
		public FolderDataViewModel<FolderPage> FolderPagesViewModel { get; }
		public DataGridAccountViewModel DataGridAccountViewModel { get; }
		private BrowserService BrowserService { get; set; }
		#endregion

		public ICommand StartCommand { get; set; }

		public ObservableCollection<AccountModel> AccountsSeleted { get; set; } = new ObservableCollection<AccountModel>();

		private readonly IDataService<Account> _accountDataService;
		private readonly IMapper _mapper;
		public PageViewModel(GeneralSettingsViewModel generalSettings,
							IDataService<Account> accountDataService,
							IDataService<Folder> folderAccountService,
							IDataService<FolderPage> folderPageService,
								//IEntityToModelConverter<Account, AccountModel> accountToModelConverter
								IMapper mapper
							)
		{
			_generalSettings = generalSettings;
			int _scale = _generalSettings.Scale;
			string _apiGPMUrl = _generalSettings.APIURL;
			_proxyList = new List<string>();
			_mapper = mapper;
			_accountDataService = accountDataService;
			#region Hiện Folder
			FolderAccountsViewModel = new FolderDataViewModel<Folder>(folderAccountService);
			FolderPagesViewModel = new FolderDataViewModel<FolderPage>(folderPageService);
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
					Parallel.ForEach(AccountsSeleted, parallelOptions, async account =>
					{
						string position = BrowserPositionHelper.GetNewPosition(800, 800, _scale);
						BrowserService = new BrowserService(account, _accountDataService, mapper);

						for (int i = 0; i < 100; i++)
						{
							account.Status = $"Test update UI {i}";
							Thread.Sleep(2000);
						}
						// Khởi động ChromeDriver cho account
						account.Driver = await BrowserService.OpenChromeGpm(_apiGPMUrl,
																   account.GPMID, account.UID,
																   account.UserAgent, scale: _scale,
																   account.Proxy, position: position);
						// Kiểm tra nếu Driver được khởi động thành công
						if (account.Driver != null)
						{
							// Thực hiện các thao tác với account


							// Thao tác xử lý account
							var t = "ádggsdgsd";
							MessageBox.Show("sghjdfgfd");


							// Giải phóng tài nguyên
							account.Driver.Quit();
							account.Driver = null; // Đặt lại Driver để giải phóng tài nguyên
						}
						else
						{
							MessageBox.Show($"Không thể khởi động trình duyệt cho tài khoản {account.UID}");
						}
					});
				});

				MessageBox.Show("Tool đã dừng!");
			});
			#endregion
		}





		private async void OnFolderChanged(int folderIdKey)
		{
			var sgdsgf = AccountsSeleted;
			// Thực hiện lệnh LoadDataToDataGridCommand với FolderIdKey
			DataGridAccountViewModel.LoadDataGridAccountCommand.Execute(folderIdKey);
		}
	}
}
