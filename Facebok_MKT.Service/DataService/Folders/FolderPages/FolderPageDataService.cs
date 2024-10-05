using AutoMapper;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Facebok_MKT.Service.DataService.Folders.FolderPages
{
	public class FolderPageDataService : IFolderPageDataService
	{
		private readonly FBDataContext _context;
		private readonly IMapper _mapper;

		public FolderPageDataService(FBDataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// Tạo mới một FolderPage
		public async Task<bool> Create(FolderPageModel model)
		{
			try
			{
				var folderPageEntity = _mapper.Map<FolderPage>(model);
				await _context.FolderPage.AddAsync(folderPageEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return false;
			}
		}

		// Xóa một FolderPage theo ID
		public async Task<bool> Delete(int id)
		{
			try
			{
				var folderPageEntity = await _context.FolderPage.FindAsync(id);
				if (folderPageEntity == null)
					return false;

				_context.FolderPage.Remove(folderPageEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return false;
			}
		}

		// Lấy tất cả FolderPage
		public async Task<List<FolderPageModel>> GetAll()
		{
			try
			{
				var folderPageEntities = await _context.FolderPage.ToListAsync();
				var folderPageModels = _mapper.Map<List<FolderPageModel>>(folderPageEntities);
				return folderPageModels;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return new List<FolderPageModel>();
			}
		}

		// Cập nhật một FolderPage theo ID
		public async Task<bool> Update(int id, FolderPageModel model)
		{
			try
			{
				var folderPageEntity = await _context.FolderPage.FindAsync(id);
				if (folderPageEntity == null)
					return false;

				_mapper.Map(model, folderPageEntity); // Cập nhật dữ liệu từ model vào entity
				_context.FolderPage.Update(folderPageEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return false;
			}
		}
	}
}
