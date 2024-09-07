using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Facebook_MKT.WPF.Commands.LoadDataGrid;
using Faceebook_MKT.Domain.Helpers.ConvertToModel;
using Faceebook_MKT.Domain.Helpers.MappingEntites;
using Faceebook_MKT.Domain.Models;
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
				services.AddScoped<IDataService<Account>, GenericDataService<Account>>();
				services.AddScoped<IDataService<Page>, GenericDataService<Page>>();
				services.AddScoped<IDataService<Folder>, GenericDataService<Folder>>();
				services.AddScoped<IDataService<FolderPage>, GenericDataService<FolderPage>>();


				services.AddAutoMapper(typeof(MappingProfile));

				//services.AddSingleton<ObservableCollection<AccountModel>>();
				//services.AddSingleton<IEntityToModelConverter<Account, AccountModel>, AccountToModelConverter>();
				//services.AddSingleton<IEntityToModelConverter<Page, PageModel>, PageToModelConverter>();
			});

			return host;
		}
	}
}
