using Facebook_MKT.WPF.State.Navigators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.WPF.HostBuilders
{
	public static class AddStoresHostBuilderExtensions
	{
		public static IHostBuilder AddStores(this IHostBuilder host)
		{
			host.ConfigureServices(services =>
			{
				services.AddSingleton<INavigator, Navigator>();
				//services.AddSingleton<IAuthenticator, Authenticator>();
				//services.AddSingleton<IAccountStore, AccountStore>();
				//services.AddSingleton<AssetStore>();
			});

			return host;
		}
	}
}
