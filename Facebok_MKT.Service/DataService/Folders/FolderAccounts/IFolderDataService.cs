using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.DataService.Folders.FolderAccounts
{
    public interface IFolderDataService
    {
        Task<List<FolderModel>> GetAll();
        Task<bool> Create(FolderModel model);
        Task<bool> Update(int id, FolderModel model);
        Task<bool> Delete(int id);
    }
}
