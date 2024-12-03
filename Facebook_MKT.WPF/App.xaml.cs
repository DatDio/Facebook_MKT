using Facebook_MKT.Data;
using Facebook_MKT.WPF.HostBuilders;
using Facebook_MKT.WPF.ViewModels;
using Facebook_MKT.WPF.ViewModels.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Facebook_MKT.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private IHost _host;

		public App()
		{
			_host = CreateHostBuilder().Build();


		}
		public static IHostBuilder CreateHostBuilder(string[] args = null)
		{
			//services.AddSingleton<MainWindow>(s => new MainWindow(s.GetRequiredService<MainViewModel>()));
			return Host.CreateDefaultBuilder(args)
				.AddDbContext()
				.AddServices()
				.AddStores()
				.AddViewModels()
				.AddViews();
		}

		protected override void OnStartup(StartupEventArgs e)
		{

			if (_host == null)
			{
				throw new InvalidOperationException("Host has not been initialized.");
			}

			_host.Start();

			using (var scope = _host.Services.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<FBDataContext>();
				//context.Database.EnsureDeleted(); // Xóa cơ sở dữ liệu
				//context.Database.EnsureCreated();
				context.Database.Migrate();
			}

			// Lấy MainWindow từ DI container và hiển thị nó nếu cần thiết
			var window = _host.Services.GetRequiredService<MainWindow>();
			window.Show();

			base.OnStartup(e);


		}

		protected override async void OnExit(ExitEventArgs e)
		{
			await _host.StopAsync();
			_host.Dispose();

			base.OnExit(e);
		}
	}

}
