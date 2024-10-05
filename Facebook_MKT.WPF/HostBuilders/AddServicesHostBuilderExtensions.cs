using Facebok_MKT.Service.DataService.Accounts;
using Facebok_MKT.Service.DataService.Folders.FolderAccounts;
using Facebok_MKT.Service.DataService.Folders.FolderPages;
using Facebok_MKT.Service.DataService.Pages;
using Facebook_MKT.Data;
using Facebook_MKT.Data.Entities;
using Faceebook_MKT.Domain.Helpers.MappingEntites;
using Faceebook_MKT.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.WPF.HostBuilders
{
	public static class AddServicesHostBuilderExtensions
	{
		public static IHostBuilder AddServices(this IHostBuilder host)
		{
			host.ConfigureServices(services =>
			{
				services.AddScoped<IAccountDataService, AccountDataService>();
				services.AddScoped<IPageDataService, PageDataService>();
				services.AddScoped<IFolderDataService, FolderAccountDataService>();
				services.AddScoped<IFolderPageDataService, FolderPageDataService>();
				services.AddScoped<DbContext, FBDataContext>();


				services.AddAutoMapper(typeof(MappingProfile));
			});

			return host;
		}
	}
}
