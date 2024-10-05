using AutoMapper;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Faceebook_MKT.Domain.Systems;

namespace Facebok_MKT.Service.DataService.Pages
{
	public class PageDataService : IPageDataService
	{
		private readonly FBDataContext _context;
		private readonly IMapper _mapper;

		public PageDataService(FBDataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// Tạo mới Page
		public async Task<bool> Create(PageModel model)
		{
			try
			{
				var pageEntity = _mapper.Map<Page>(model);

				// Kiểm tra xem có tồn tại trong cơ sở dữ liệu hay không
				var existingPage = await _context.Pages
					.AsNoTracking() // Không theo dõi thực thể
					.FirstOrDefaultAsync(p => p.PageID == pageEntity.PageID);

				if (existingPage != null)
				{
					return false; // Nếu đã tồn tại, không tạo mới
				}

				// Nếu không tồn tại, ngắt theo dõi trước khi thêm vào lại
				var trackedPage = _context.ChangeTracker.Entries<Page>()
					.FirstOrDefault(e => e.Entity.PageID == pageEntity.PageID);

				if (trackedPage != null)
				{
					// Ngắt theo dõi thực thể cũ nếu đang theo dõi
					_context.Entry(trackedPage.Entity).State = EntityState.Detached;
				}

				// Thêm thực thể mới
				await _context.Pages.AddAsync(pageEntity);
				await _context.SaveChangesAsync();

				return true;
			}
			catch
			{
				return false;
			}
		}


		// Xóa Page theo ID
		public async Task<bool> Delete(string id)
		{
			try
			{
				var pageEntity = await _context.Pages.FindAsync(id);
				if (pageEntity == null)
					return false;

				 _context.Pages.Remove(pageEntity);
				await _context.SaveChangesAsync();

				// Ngắt theo dõi thực thể sau khi xóa
				_context.Entry(pageEntity).State = EntityState.Detached;
				var path = Path.GetFullPath($"{pageEntity.PageFolderVideo}");
				if (Directory.Exists(Path.GetFullPath($"{pageEntity.PageFolderVideo}")))
				{
					Directory.Delete(Path.GetFullPath($"{pageEntity.PageFolderVideo}"));
				}
				return true;
			}
			catch
			{
				return false;
			}
		}


		// Lấy tất cả các Page
		public async Task<List<PageModel>> GetAll()
		{
			try
			{
				var pages = await _context.Pages
					.Include(p => p.FolderPage) // Tải FolderPage
					.ToListAsync();

				// Ánh xạ từ Page sang PageModel
				var pageModels = _mapper.Map<List<PageModel>>(pages);
				return pageModels;
			}
			catch
			{
				return null;
			}
		}

		// Lấy danh sách Page theo FolderIdKey
		public async Task<List<PageModel>> GetByFolderIdKey(int folderId)
		{
			try
			{
				var pages = await _context.Pages
					.Where(p => p.FolderIdKey == folderId)
					.Include(p => p.FolderPage) // Tải FolderPage
					.ToListAsync();

				// Sử dụng AutoMapper để ánh xạ danh sách
				return _mapper.Map<List<PageModel>>(pages);
			}
			catch
			{
				return null;
			}
		}

		// Lấy thông tin Page theo ID
		public async Task<PageModel> GetByID(string id)
		{
			try
			{
				var page = await _context.Pages.FindAsync(id);
				return page != null ? _mapper.Map<PageModel>(page) : null;
			}
			catch
			{
				return null;
			}
		}

		// Cập nhật Page theo ID
		public async Task<bool> Update(string id, PageModel model)
		{
			try
			{
				// Tìm kiếm thực thể theo ID
				var pageEntity = await _context.Pages.FindAsync(id);
				if (pageEntity == null)
					return false;

				// Ánh xạ từ model sang thực thể
				_mapper.Map(model, pageEntity);

				// Đảm bảo thực thể được theo dõi
				_context.Pages.Attach(pageEntity); // Đảm bảo thực thể được theo dõi trước khi cập nhật

				// Cập nhật thực thể
				_context.Pages.Update(pageEntity);
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
