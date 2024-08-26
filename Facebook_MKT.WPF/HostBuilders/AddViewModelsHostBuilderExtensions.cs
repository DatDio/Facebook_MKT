﻿using Facebook_MKT.WPF.State.Navigators;
using Facebook_MKT.WPF.ViewModels;
using Facebook_MKT.WPF.ViewModels.Factories;
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
				//services.AddTransient(CreateHomeViewModel);
				services.AddTransient<PageViewModel>();
				services.AddTransient<PagePostViewModel>();
				services.AddTransient<PageInteractViewModel>();
				services.AddTransient<GroupViewModel>();
				services.AddTransient<MainViewModel>();

				services.AddSingleton<CreateViewModel<PageViewModel>>(services => () => services.GetRequiredService<PageViewModel>());
				services.AddSingleton<CreateViewModel<PagePostViewModel>>(services => () => services.GetRequiredService<PagePostViewModel>());
				services.AddSingleton<CreateViewModel<PageInteractViewModel>>(services => () => services.GetRequiredService<PageInteractViewModel>());
				services.AddSingleton<CreateViewModel<GroupViewModel>>(services => () => services.GetRequiredService<GroupViewModel>());
				
				services.AddSingleton<IViewModelFactory,ViewModelFactory>();

				services.AddSingleton<ViewModelDelegateRenavigator<PageViewModel>>();
				//services.AddSingleton<ViewModelDelegateRenavigator<LoginViewModel>>();
				//services.AddSingleton<ViewModelDelegateRenavigator<RegisterViewModel>>();
			});

			return host;
		}

		//private static PageViewModel CreateHomeViewModel(IServiceProvider services)
		//{
		//	return new PageViewModel(
		//		services.GetRequiredService<AssetSummaryViewModel>(),
		//		MajorIndexListingViewModel.LoadMajorIndexViewModel(services.GetRequiredService<IMajorIndexService>()));
		//}
		
	}
}
