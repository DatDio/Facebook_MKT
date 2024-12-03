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
		public DbSet<Group> Groups { get; set; }
		public DbSet<Folder> Folders { get; set; }
		public DbSet<FolderPage> FolderPage { get; set; }
		public DbSet<FolderGroup> FolderGroup { get; set; }

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
				entity.HasKey(p => p.AccountIDKey);
				entity.Property(p => p.AccountIDKey).ValueGeneratedOnAdd();
				entity.Property(a => a.UID);
				entity.Property(a => a.UID).IsRequired();
				entity.HasIndex(a => a.UID).IsUnique();
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

			});

			// Cấu hình cho Page
			modelBuilder.Entity<Page>(entity =>
			{
				//entity.HasKey(p => p.PageIdKey);
				//entity.Property(p => p.PageIdKey).ValueGeneratedOnAdd();
				entity.HasKey(p => p.PageID);
				entity.Property(p => p.PageName);
				entity.Property(p => p.PageFollow);
				entity.Property(p => p.PageLike);
				entity.Property(p => p.PageStatus);

				// Thiết lập mối quan hệ và hành vi xóa
				entity.HasOne(p => p.Account)
					.WithMany(a => a.Pages)
					.HasForeignKey(p => p.AccountIDKey)
					.OnDelete(DeleteBehavior.Cascade); ;

				entity.HasOne(p => p.FolderPage)
						.WithMany(f => f.Pages)
						.HasForeignKey(p => p.FolderIdKey)
						.OnDelete(DeleteBehavior.Cascade);

			});

			// Cấu hình cho Group
			modelBuilder.Entity<Group>(entity =>
			{
				entity.HasKey(p => p.GroupID);
				//entity.Property(p => p.GroupIdKey).ValueGeneratedOnAdd();
				entity.Property(p => p.GroupName);
				entity.Property(p => p.GroupMember);
				entity.Property(p => p.GroupCensor);
				entity.Property(p => p.TypeGroup);
				entity.Property(p => p.GroupStatus);

				entity.HasOne(a => a.FolderGroup)
						.WithMany(f => f.Groups)
						.HasForeignKey(a => a.FolderIdKey);
			});

			//Cấu hình cho Folder
			modelBuilder.Entity<Folder>(entity =>
			{
				entity.HasKey(f => f.FolderIdKey);
				entity.Property(f => f.FolderIdKey).ValueGeneratedOnAdd();
				entity.Property(f => f.FolderName)
								.IsRequired()
								.HasMaxLength(255);
				entity.HasIndex(f => f.FolderName).IsUnique();
				entity.HasData(
						   new Folder { FolderIdKey = 1, FolderName = "All" });

			});

			//Cấu hình cho FolderPage
			modelBuilder.Entity<FolderPage>(entity =>
			{
				entity.HasKey(f => f.FolderIdKey);

				entity.Property(f => f.FolderName)
								.IsRequired()
								.HasMaxLength(255);
				entity.HasIndex(f => f.FolderName).IsUnique();
				entity.HasData(
						   new FolderPage { FolderIdKey = 1, FolderName = "All" });
				entity.Property(f => f.FolderIdKey).ValueGeneratedOnAdd();
			});

			//Cấu hình cho FolderGroup
			modelBuilder.Entity<FolderGroup>(entity =>
			{
				entity.HasKey(f => f.FolderIdKey);
				entity.Property(f => f.FolderName)
								.IsRequired()
								.HasMaxLength(255);
				entity.HasIndex(f => f.FolderName).IsUnique();
				entity.HasData(
						   new FolderGroup { FolderIdKey = 1, FolderName = "All" });
				entity.Property(f => f.FolderIdKey).ValueGeneratedOnAdd();
			});
		}
	}
}
