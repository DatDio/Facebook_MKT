using Facebook_MKT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.WPF.HostBuilders
{
	public static class AddDbContextHostBuilderExtensions
	{
		public static IHostBuilder AddDbContext(this IHostBuilder host)
		{
			host.ConfigureServices((context, services) =>
			{
				services.AddDbContext<FBDataContext>(options =>
						options.UseSqlite("Data Source=FacebookMKT.db"));

			});

			return host;
		}
	}
}
