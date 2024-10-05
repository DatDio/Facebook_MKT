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

namespace Facebok_MKT.Service.DataService.Folders.FolderAccounts
{
	public class FolderAccountDataService : IFolderDataService
	{
		private readonly FBDataContext _context;
		private readonly IMapper _mapper;

		public FolderAccountDataService(FBDataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// Tạo mới một Folder
		public async Task<bool> Create(FolderModel model)
		{
			try
			{
				var folderEntity = _mapper.Map<Folder>(model);
				await _context.Folders.AddAsync(folderEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return false;
			}
		}

		// Xóa một Folder theo ID
		public async Task<bool> Delete(int id)
		{
			try
			{
				var folderEntity = await _context.Folders.FindAsync(id);
				if (folderEntity == null)
					return false;

				_context.Folders.Remove(folderEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return false;
			}
		}

		// Lấy tất cả các Folder
		public async Task<List<FolderModel>> GetAll()
		{
			try
			{
				var folderEntities = await _context.Folders.ToListAsync();
				var folderModels = _mapper.Map<List<FolderModel>>(folderEntities);
				return folderModels;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return new List<FolderModel>();
			}
		}

		// Cập nhật thông tin của một Folder theo ID
		public async Task<bool> Update(int id, FolderModel model)
		{
			try
			{
				var folderEntity = await _context.Folders.FindAsync(id);
				if (folderEntity == null)
					return false;

				_mapper.Map(model, folderEntity); // Cập nhật dữ liệu từ model vào entity
				_context.Folders.Update(folderEntity);
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
