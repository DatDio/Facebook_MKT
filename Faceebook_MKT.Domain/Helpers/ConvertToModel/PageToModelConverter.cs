using Facebook_MKT.Data.Entities;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Helpers.ConvertToModel
{
	public class PageToModelConverter : IEntityToModelConverter<Page, PageModel>
	{
		public PageModel Convert(Page entity)
		{
			return new PageModel
			{
				PageIdKey = entity.PageIdKey,
				PageID = entity.PageID,
				AccountID = entity.AccountID,
				PageName = entity.PageName,
				PageFollow = entity.PageFollow,
				PageLike = entity.PageLike,
				PageStatus = entity.PageStatus,
				FolderIdKey = entity.FolderIdKey
			};
		}
	}
}
