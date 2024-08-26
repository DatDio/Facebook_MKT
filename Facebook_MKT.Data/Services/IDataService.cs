using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.Data.Services
{
	public interface IDataService<T>
	{
		Task<List<T>> GetAll();
		Task<List<T>> GetByFolderIdKey(int id);
		Task<T> Create(T entity);
		Task<T> Update(int id, T entity);
		Task<bool> Delete(int id);
		Task<T> GetById(int id);
	}
}
