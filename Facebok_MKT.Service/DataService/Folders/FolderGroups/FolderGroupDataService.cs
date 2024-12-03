using AutoMapper;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.DataService.Folders.FolderGroups
{
	public class FolderGroupDataService : IFolderGroupDataService
	{
		private readonly FBDataContext _context;
		private readonly IMapper _mapper;

		public FolderGroupDataService(FBDataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// Tạo mới một FolderGroup
		public async Task<bool> Create(FolderGroupModel model)
		{
			try
			{
				var folderGroupEntity = _mapper.Map<FolderGroup>(model);
				await _context.FolderGroup.AddAsync(folderGroupEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return false;
			}
		}

		// Xóa một FolderGroup theo ID
		public async Task<bool> Delete(int id)
		{
			try
			{
				var folderGroupEntity = await _context.FolderGroup.FindAsync(id);
				if (folderGroupEntity == null)
					return false;

				_context.FolderGroup.Remove(folderGroupEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return false;
			}
		}

		// Lấy tất cả FolderGroup
		public async Task<List<FolderGroupModel>> GetAll()
		{
			try
			{
				var folderGroupEntities = await _context.FolderGroup.ToListAsync();
				var folderGroupModels = _mapper.Map<List<FolderGroupModel>>(folderGroupEntities);
				return folderGroupModels;
			}
			catch (Exception ex)
			{
				// Log exception (optional)
				return new List<FolderGroupModel>();
			}
		}

		// Cập nhật một FolderGroup theo ID
		public async Task<bool> Update(int id, FolderGroupModel model)
		{
			try
			{
				var folderGroupEntity = await _context.FolderGroup.FindAsync(id);
				if (folderGroupEntity == null)
					return false;

				_mapper.Map(model, folderGroupEntity); // Cập nhật dữ liệu từ model vào entity
				_context.FolderGroup.Update(folderGroupEntity);
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
