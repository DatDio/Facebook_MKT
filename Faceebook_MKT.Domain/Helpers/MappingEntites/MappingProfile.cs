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
					.ForMember(dest => dest.AccountFolderName, opt => opt.MapFrom(src => src.Folder.FolderName))
					.ReverseMap();

			CreateMap<Page, PageModel>()
					.ForMember(dest => dest.PageFolderName, opt => opt.MapFrom(src => src.FolderPage.FolderName)) // Ánh xạ PageFolderName
					.ReverseMap();

			CreateMap<Folder, FolderModel>().ReverseMap();

			CreateMap<FolderPage, FolderPageModel>().ReverseMap();

			//CreateMap<FolderGroup, FolderGrModel>().ReverseMap();
		}
	}
}
