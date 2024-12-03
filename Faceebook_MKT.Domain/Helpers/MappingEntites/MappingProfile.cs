using AutoMapper;
using Facebook_MKT.Data.Entities;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Helpers.MappingEntites
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Account, AccountModel>()
	.ForMember(dest => dest.AccountFolderName, opt => opt.MapFrom(src => src.Folder.FolderName)) // Ánh xạ từ Folder.FolderName
	.AfterMap((src, dest) =>
	{
		// Kiểm tra nếu UserAgent là null và gán giá trị mặc định
		if (string.IsNullOrEmpty(dest.UserAgent))
		{
			dest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36";
		}
	})
	.ReverseMap() // Ánh xạ ngược từ AccountModel về Account
	.ForMember(dest => dest.Folder, opt => opt.Ignore()); // Bỏ qua ánh xạ thuộc tính Folder trong trường hợp ánh xạ ngược



			CreateMap<Page, PageModel>()
	.ForMember(dest => dest.PageFolderName, opt => opt.MapFrom(src => src.FolderPage.FolderName)) // Ánh xạ PageFolderName
	.ReverseMap()
	.ForMember(dest => dest.FolderPage, opt => opt.Ignore()); // Bỏ qua ánh xạ FolderPage

			CreateMap<Group, GroupModel>()
	.ForMember(dest => dest.GroupFolderName, opt => opt.MapFrom(src => src.FolderGroup.FolderName)) 
	.ReverseMap()
	.ForMember(dest => dest.FolderGroup, opt => opt.Ignore()); 

			CreateMap<Folder, FolderModel>().ReverseMap();

			CreateMap<FolderPage, FolderPageModel>().ReverseMap();

			CreateMap<FolderGroup, FolderGroupModel>().ReverseMap();
		}
	}
}
