using Facebook_MKT.Data.Entities;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Helpers.ConvertToModel
{
	public class AccountToModelConverter : IEntityToModelConverter<Account, AccountModel>
	{
		public AccountModel Convert(Account entity)
		{
			return new AccountModel
			{
				AccountIDKey = entity.AccountIDKey,
				UID = entity.UID,
				Password = entity.Password,
				Email1 = entity.Email1,
				Email1Password = entity.Email1Password,
				C_2FA = entity.C_2FA,
				Email2 = entity.Email2,
				Email2Password = entity.Email2Password,
				Cookie = entity.Cookie,
				FolderIdKey = entity.FolderIdKey,
				GPMID = entity.GPMID,
				Proxy = entity.Proxy,
				Status = entity.Status,
				Token = entity.Token,
				UserAgent = entity.UserAgent,
				IsSelected = false // Default property
			};
		}
	}
}
