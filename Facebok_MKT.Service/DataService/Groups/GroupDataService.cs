using AutoMapper;
using Facebook_MKT.Data;
using Faceebook_MKT.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook_MKT.Data.Entities;

namespace Facebok_MKT.Service.DataService.Groups
{
	public class GroupDataService : IGroupDataService
	{
		private readonly FBDataContext _context;
		private readonly IMapper _mapper;

		public GroupDataService(FBDataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// Tạo mới Group
		public async Task<bool> Create(GroupModel model)
		{
			try
			{
				var groupEntity = _mapper.Map<Group>(model);

				// Kiểm tra xem nhóm đã tồn tại chưa
				var existingGroup = await _context.Groups
					.AsNoTracking()
					.FirstOrDefaultAsync(g => g.GroupID == groupEntity.GroupID);

				if (existingGroup != null)
				{
					return false; // Nhóm đã tồn tại
				}

				// Ngắt theo dõi thực thể cũ nếu cần
				var trackedGroup = _context.ChangeTracker.Entries<Group>()
					.FirstOrDefault(e => e.Entity.GroupID == groupEntity.GroupID);

				if (trackedGroup != null)
				{
					_context.Entry(trackedGroup.Entity).State = EntityState.Detached;
				}

				// Thêm nhóm mới
				await _context.Groups.AddAsync(groupEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		// Xóa Group theo ID
		public async Task<bool> Delete(string id)
		{
			try
			{
				var groupEntity = await _context.Groups.FindAsync(id);
				if (groupEntity == null)
					return false;

				_context.Groups.Remove(groupEntity);
				await _context.SaveChangesAsync();

				// Ngắt theo dõi sau khi xóa
				_context.Entry(groupEntity).State = EntityState.Detached;
				return true;
			}
			catch
			{
				return false;
			}
		}

	

		// Lấy tất cả các Group
		public async Task<List<GroupModel>> GetAll()
		{
			try
			{
				var groups = await _context.Groups
					.Include(g => g.FolderGroup) // Nếu cần tải thêm dữ liệu liên quan
					.ToListAsync();

				// Ánh xạ từ Group sang GroupModel
				var groupModels = _mapper.Map<List<GroupModel>>(groups);
				return groupModels;
			}
			catch
			{
				return null;
			}
		}

		public async Task<List<GroupModel>> GetByFolderIdKey(int folderId)
		{
			try
			{
				var groups = await _context.Groups
					.Where(p => p.FolderIdKey == folderId)
					.Include(p => p.FolderGroup) 
					.ToListAsync();

				// Sử dụng AutoMapper để ánh xạ danh sách
				return _mapper.Map<List<GroupModel>>(groups);
			}
			catch
			{
				return null;
			}
		}

		// Lấy thông tin Group theo ID
		public async Task<GroupModel> GetByID(string id)
		{
			try
			{
				var group = await _context.Groups.FindAsync(id);
				return group != null ? _mapper.Map<GroupModel>(group) : null;
			}
			catch
			{
				return null;
			}
		}

		// Cập nhật Group theo ID
		public async Task<bool> Update(string id, GroupModel model)
		{
			try
			{
				// Tìm kiếm thực thể theo ID
				var groupEntity = await _context.Groups.FindAsync(id);
				if (groupEntity == null)
					return false;

				// Ánh xạ từ model sang thực thể
				_mapper.Map(model, groupEntity);

				// Đảm bảo thực thể được theo dõi
				_context.Groups.Attach(groupEntity);

				// Cập nhật thực thể
				_context.Groups.Update(groupEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
