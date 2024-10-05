using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.DataService.Pages
{
	public interface IPageDataService
	{
		Task<List<PageModel>> GetAll();
		Task<List<PageModel>> GetByFolderIdKey(int fodlerId);
		Task<bool> Create(PageModel model);
		Task<bool> Update(string id, PageModel model);
		Task<bool> Delete(string id);
		Task<PageModel> GetByID(string id);
	}
}
