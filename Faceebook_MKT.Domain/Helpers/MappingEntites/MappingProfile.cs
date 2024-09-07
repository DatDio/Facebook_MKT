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
	public class MappingProfile:Profile
	{
		public MappingProfile()
		{
			CreateMap<Account, AccountModel>().ReverseMap();
			CreateMap<Page, PageModel>().ReverseMap();
		}
	}
}
