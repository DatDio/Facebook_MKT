using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.DataService.Groups
{
	public interface IGroupDataService
	{
		Task<List<GroupModel>> GetAll();
		Task<List<GroupModel>> GetByFolderIdKey(int folderId);
		Task<bool> Create(GroupModel model);
		Task<bool> Update(string id, GroupModel model);
		Task<bool> Delete(string id);
		Task<GroupModel> GetByID(string id);
	}
}
