using Facebook_MKT.Data.Entities;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.DataService.Accounts
{
	public interface IAccountDataService
	{
		Task<List<AccountModel>> GetAll();
		Task<List<AccountModel>> GetByFolderIdKey(int fodlerId);
		Task<bool> Create(AccountModel model);
		Task<bool> Update(int id, AccountModel model);
		Task<bool> Delete(int id);
	}
}
