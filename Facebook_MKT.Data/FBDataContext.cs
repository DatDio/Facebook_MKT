using Facebook_MKT.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.Data
{
	public class FBDataContext : DbContext
	{
		public FBDataContext(DbContextOptions<FBDataContext> options)
		   : base(options)
		{
		}
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Page> Pages { get; set; }
		public DbSet<Folder> Folders { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var path = Path.GetFullPath("FacebookMKT.db");
			optionsBuilder.UseSqlite($"Data Source={Path.GetFullPath("FacebookMKT.db")}");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Cấu hình cho Account
			modelBuilder.Entity<Account>(entity =>
			{
				entity.HasKey(a => a.AccountIDKey);
				entity.Property(a => a.AccountIDKey).ValueGeneratedOnAdd();
				entity.Property(a => a.UID);
				entity.Property(a => a.Password);
				entity.Property(a => a.C_2FA);
				entity.Property(a => a.Email1);
				entity.Property(a => a.Email1Password);
				entity.Property(a => a.Email2);
				entity.Property(a => a.Email2Password);
				entity.Property(a => a.Token);
				entity.Property(a => a.Cookie);
				entity.Property(a => a.Status);
				entity.Property(a => a.Proxy);
				entity.Property(a => a.UserAgent);
				entity.Property(a => a.GPMID);
				entity.HasMany(a => a.Pages);

				entity.HasOne(a => a.Folder)
						.WithMany(f => f.Accounts)
						.HasForeignKey(a => a.FolderIdKey);
				entity.HasData(
					new Account
					{
						AccountIDKey = 1,
						UID="47812389",
						Password="qưerfuhsdiuvsd",
						C_2FA="eghjdsjkgsdhg",
						FolderIdKey = 1


					},
					new Account
					{
						AccountIDKey = 2,
						UID = "47812389sdfgsdg",
						Password = "passcfb",
						C_2FA = "eghjdsjksgsdggsdhg",
						FolderIdKey = 1
					}
					);
			});

			// Cấu hình cho Page
			modelBuilder.Entity<Page>(entity =>
			{
				entity.HasKey(p => p.PageIdKey);
				entity.Property(p => p.PageIdKey).ValueGeneratedOnAdd();
				entity.Property(p => p.PageID);
				entity.Property(p => p.PageName);
				entity.Property(p => p.PageFollow);
				entity.Property(p => p.PageLike);
				entity.Property(p => p.PageStatus);

				// Thiết lập mối quan hệ và hành vi xóa
				entity.HasOne(p => p.Account)
					.WithMany(a => a.Pages)
					.HasForeignKey(p => p.AccountID);

				entity.HasOne(p => p.Folder)
						.WithMany(f => f.Pages)
						.HasForeignKey(p => p.FolderIdKey);

			});

			//Cấu hình cho Folder
			modelBuilder.Entity<Folder>(entity =>
			{
				entity.HasKey(f => f.FolderIdKey);
				entity.Property(f => f.FolderIdKey).ValueGeneratedOnAdd();
				entity.Property(f => f.FolderName);
				entity.HasData(
						   new Folder { FolderIdKey = 1, FolderName = "All" },
						   new Folder { FolderIdKey = 2, FolderName = "Test" }
	   );
			});
		}
	}
}
