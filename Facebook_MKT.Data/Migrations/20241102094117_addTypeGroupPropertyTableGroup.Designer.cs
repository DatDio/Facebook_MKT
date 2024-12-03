﻿// <auto-generated />
using Facebook_MKT.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Facebook_MKT.Data.Migrations
{
    [DbContext(typeof(FBDataContext))]
    [Migration("20241102094117_addTypeGroupPropertyTableGroup")]
    partial class addTypeGroupPropertyTableGroup
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("Facebook_MKT.Data.Entities.Account", b =>
                {
                    b.Property<int>("AccountIDKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("C_2FA")
                        .HasColumnType("TEXT");

                    b.Property<string>("Cookie")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email1")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email1Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email2")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email2Password")
                        .HasColumnType("TEXT");

                    b.Property<int>("FolderIdKey")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GPMID")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("Proxy")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.Property<string>("Token")
                        .HasColumnType("TEXT");

                    b.Property<string>("UID")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserAgent")
                        .HasColumnType("TEXT");

                    b.HasKey("AccountIDKey");

                    b.HasIndex("FolderIdKey");

                    b.HasIndex("UID")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.Folder", b =>
                {
                    b.Property<int>("FolderIdKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FolderName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("FolderIdKey");

                    b.HasIndex("FolderName")
                        .IsUnique();

                    b.ToTable("Folders");

                    b.HasData(
                        new
                        {
                            FolderIdKey = 1,
                            FolderName = "All"
                        });
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.FolderGroup", b =>
                {
                    b.Property<int>("FolderIdKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FolderName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("FolderIdKey");

                    b.HasIndex("FolderName")
                        .IsUnique();

                    b.ToTable("FolderGroup");

                    b.HasData(
                        new
                        {
                            FolderIdKey = 1,
                            FolderName = "All"
                        });
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.FolderPage", b =>
                {
                    b.Property<int>("FolderIdKey")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FolderName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("FolderIdKey");

                    b.HasIndex("FolderName")
                        .IsUnique();

                    b.ToTable("FolderPage");

                    b.HasData(
                        new
                        {
                            FolderIdKey = 1,
                            FolderName = "All"
                        });
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.Group", b =>
                {
                    b.Property<string>("GroupID")
                        .HasColumnType("TEXT");

                    b.Property<int>("FolderGroupFolderIdKey")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FolderIdKey")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GroupCensor")
                        .HasColumnType("TEXT");

                    b.Property<string>("GroupMember")
                        .HasColumnType("TEXT");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GroupStatus")
                        .HasColumnType("TEXT");

                    b.Property<string>("TypeGroup")
                        .HasColumnType("TEXT");

                    b.HasKey("GroupID");

                    b.HasIndex("FolderGroupFolderIdKey");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.Page", b =>
                {
                    b.Property<string>("PageID")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccountIDKey")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FolderIdKey")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PageFolderVideo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PageFollow")
                        .HasColumnType("TEXT");

                    b.Property<string>("PageLike")
                        .HasColumnType("TEXT");

                    b.Property<string>("PageName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PageStatus")
                        .HasColumnType("TEXT");

                    b.Property<string>("ViewVideo1")
                        .HasColumnType("TEXT");

                    b.Property<string>("ViewVideo2")
                        .HasColumnType("TEXT");

                    b.Property<string>("ViewVideo3")
                        .HasColumnType("TEXT");

                    b.Property<string>("ViewVideo4")
                        .HasColumnType("TEXT");

                    b.Property<string>("ViewVideo5")
                        .HasColumnType("TEXT");

                    b.HasKey("PageID");

                    b.HasIndex("AccountIDKey");

                    b.HasIndex("FolderIdKey");

                    b.ToTable("Pages");
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.Account", b =>
                {
                    b.HasOne("Facebook_MKT.Data.Entities.Folder", "Folder")
                        .WithMany("Accounts")
                        .HasForeignKey("FolderIdKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.Group", b =>
                {
                    b.HasOne("Facebook_MKT.Data.Entities.FolderGroup", "FolderGroup")
                        .WithMany("Groups")
                        .HasForeignKey("FolderGroupFolderIdKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FolderGroup");
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.Page", b =>
                {
                    b.HasOne("Facebook_MKT.Data.Entities.Account", "Account")
                        .WithMany("Pages")
                        .HasForeignKey("AccountIDKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Facebook_MKT.Data.Entities.FolderPage", "FolderPage")
                        .WithMany("Pages")
                        .HasForeignKey("FolderIdKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("FolderPage");
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.Account", b =>
                {
                    b.Navigation("Pages");
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.Folder", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.FolderGroup", b =>
                {
                    b.Navigation("Groups");
                });

            modelBuilder.Entity("Facebook_MKT.Data.Entities.FolderPage", b =>
                {
                    b.Navigation("Pages");
                });
#pragma warning restore 612, 618
        }
    }
}
