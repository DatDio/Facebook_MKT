using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.Data
{
	public class FBDataContextFactory : IDesignTimeDbContextFactory<FBDataContext>
	{
		public FBDataContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<FBDataContext>();
			var path = Path.GetFullPath("FacebookMKT.db");
			optionsBuilder.UseSqlite($"Data Source={Path.GetFullPath("FacebookMKT.db")}");

			return new FBDataContext(optionsBuilder.Options);
		}
	}
}
