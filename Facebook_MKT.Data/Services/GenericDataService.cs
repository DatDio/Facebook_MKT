using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.Data.Services
{
	public class GenericDataService<T> : IDataService<T> where T : class
	{
		private readonly FBDataContext _context;

		public GenericDataService(FBDataContext context)
		{
			_context = context;
		}

		public async Task<List<T>> GetAll()
		{
			var results = await _context.Set<T>().ToListAsync();
			return await _context.Set<T>().ToListAsync();
		}

		public async Task<T> Create(T entity)
		{
			_context.Set<T>().Add(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public async Task<T> Update(int id, T entity)
		{
			var existingEntity = await _context.Set<T>().FindAsync(id);
			if (existingEntity == null)
			{
				return null;
			}

			_context.Entry(existingEntity).CurrentValues.SetValues(entity);
			await _context.SaveChangesAsync();
			return existingEntity;
		}

		public async Task<bool> Delete(int id)
		{
			var entity = await _context.Set<T>().FindAsync(id);
			if (entity == null)
			{
				return false;
			}

			_context.Set<T>().Remove(entity);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<T> GetById(int id)
		{
			var results = await _context.Set<T>().FindAsync(id);
			return await _context.Set<T>().FindAsync(id);
		}

		public async Task<List<T>> GetByFolderIdKey(int folderIdKey)
		{
			// Giả sử rằng T là một thực thể có thuộc tính FolderIdKey
			// Sử dụng LINQ để lọc các thực thể theo FolderIdKey
			var results = await _context.Set<T>()
				.Where(e => EF.Property<int>(e, "FolderIdKey") == folderIdKey)
				.ToListAsync();

			return results;
		}
	}

}
