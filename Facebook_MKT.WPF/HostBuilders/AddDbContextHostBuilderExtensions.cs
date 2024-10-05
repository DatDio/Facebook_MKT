using Facebook_MKT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
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
				// Sử dụng chuỗi kết nối cho SQLite từ file cấu hình hoặc tạo một chuỗi kết nối cứng
				var connectionString = $"Data Source={Path.GetFullPath("FacebookMKT.db")}";


				services.AddDbContext<FBDataContext>(options =>
				{
					options.UseSqlite(connectionString);
					options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // Nếu bạn không cần theo dõi trạng thái đối tượng
				}, ServiceLifetime.Scoped); // Thay đổi ServiceLifetime thành Transient
			});

			return host;
		}
	}

}
