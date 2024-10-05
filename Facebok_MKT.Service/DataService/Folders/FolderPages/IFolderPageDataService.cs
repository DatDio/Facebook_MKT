using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.DataService.Folders.FolderPages
{
	public interface IFolderPageDataService
	{
		Task<List<FolderPageModel>> GetAll();
		Task<bool> Create(FolderPageModel model);
		Task<bool> Update(int id, FolderPageModel model);
		Task<bool> Delete(int id);
	}
}
