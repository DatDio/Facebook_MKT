using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.DataService.Folders.FolderGroups
{
	public interface IFolderGroupDataService
	{
		Task<List<FolderGroupModel>> GetAll();
		Task<bool> Create(FolderGroupModel model);
		Task<bool> Update(int id, FolderGroupModel model);
		Task<bool> Delete(int id);
	}
}
