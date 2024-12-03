using Facebok_MKT.Service.DataService.Accounts;
using Facebok_MKT.Service.DataService.Folders.FolderAccounts;
using Facebok_MKT.Service.DataService.Folders.FolderPages;
using Facebok_MKT.Service.DataService.Pages;
using Facebook_MKT.WPF.ViewModels.General_settings;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Facebook_MKT.WPF.Window.SetupPostWindow;
namespace Facebook_MKT.WPF.ViewModels.Pages
{
	public class PagePostViewModel : PageViewModel
	{
		public ICommand SetUpPostCommand { get; set; }
		public PagePostViewModel(GeneralSettingsViewModel generalSettings,
			IAccountDataService accountDataService,
			IPageDataService pageDataService,
			IFolderDataService folderAccountService,
			IFolderPageDataService folderPageService) :
			base(generalSettings, accountDataService, pageDataService, folderAccountService, folderPageService)
		{

			SetUpPostCommand = new RelayCommand<object>((taskModel) => true,
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
		}
	}
}
