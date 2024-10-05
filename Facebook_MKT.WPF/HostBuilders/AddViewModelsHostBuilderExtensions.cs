using Facebok_MKT.Service.Controller.BrowserController;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.WPF.State.Navigators;
using Facebook_MKT.WPF.ViewModels;
using Facebook_MKT.WPF.ViewModels.Accounts;
using Facebook_MKT.WPF.ViewModels.Combobox;
using Facebook_MKT.WPF.ViewModels.DataGrid;
using Facebook_MKT.WPF.ViewModels.Factories;
using Facebook_MKT.WPF.ViewModels.General_settings;
using Facebook_MKT.WPF.ViewModels.Groups;
using Facebook_MKT.WPF.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.WPF.HostBuilders
{
	public static class AddViewModelsHostBuilderExtensions
	{
		public static IHostBuilder AddViewModels(this IHostBuilder host)
		{
			host.ConfigureServices(services =>
			{
				services.AddScoped<PageViewModel>();
				services.AddScoped<PagePostViewModel>();
				services.AddScoped<PageInteractViewModel>();
				services.AddScoped<GroupViewModel>();
				services.AddScoped<MainViewModel>();
				services.AddScoped<AccountInteractViewModel>();
				services.AddScoped<GeneralSettingsViewModel>();
				//services.AddSingleton<FolderAccountViewModel>();
				//services.AddSingleton<FolderPageViewModel>();

				services.AddScoped<CreateViewModel<BaseViewModel>>(services => () => services.GetRequiredService<BaseViewModel>());

				services.AddScoped<CreateViewModel<PageViewModel>>(services => () => services.GetRequiredService<PageViewModel>());
				services.AddScoped<CreateViewModel<PagePostViewModel>>(services => () => services.GetRequiredService<PagePostViewModel>());
				services.AddScoped<CreateViewModel<PageInteractViewModel>>(services => () => services.GetRequiredService<PageInteractViewModel>());
				services.AddScoped<CreateViewModel<GroupViewModel>>(services => () => services.GetRequiredService<GroupViewModel>());
				services.AddScoped<CreateViewModel<AccountInteractViewModel>>(services => () => services.GetRequiredService<AccountInteractViewModel>());
				services.AddScoped<CreateViewModel<GeneralSettingsViewModel>>(services => () => services.GetRequiredService<GeneralSettingsViewModel>());
				
				services.AddSingleton<IViewModelFactory,ViewModelFactory>();

				//services.AddSingleton<ViewModelDelegateRenavigator<PageViewModel>>();
			});

			return host;
		}
		
	}
}
