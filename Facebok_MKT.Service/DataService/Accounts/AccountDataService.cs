using AutoMapper;
using Facebook_MKT.Data;
using Facebook_MKT.Data.Entities;
using Faceebook_MKT.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.DataService.Accounts
{
	public class AccountDataService : IAccountDataService
	{
		private readonly FBDataContext _context;
		private readonly IMapper _mapper;

		public AccountDataService(FBDataContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		// Tạo mới tài khoản
		public async Task<bool> Create(AccountModel model)
		{
			try
			{
				var accountEntity = _mapper.Map<Account>(model);
				await _context.Accounts.AddAsync(accountEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		// Xóa tài khoản theo ID
		public async Task<bool> Delete(int id)
		{
			try
			{
				var accountEntity = await _context.Accounts.FindAsync(id);
				if (accountEntity == null)
					return false;

				_context.Accounts.Remove(accountEntity);
				await _context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		// Lấy tất cả các tài khoản
		public async Task<List<AccountModel>> GetAll()
		{
			try
			{
				var accounts = await _context.Accounts
								.Include(a => a.Folder)
								.ToListAsync();
				return _mapper.Map<List<AccountModel>>(accounts);
			}
			catch
			{
				return null;
			}
		}

		// Lấy tài khoản theo FolderIdKey
		public async Task<List<AccountModel>> GetByFolderIdKey(int folderId)
		{
			try
			{
				var accounts = await _context.Accounts
					.Where(a => a.FolderIdKey == folderId)
					.Include(a => a.Folder)
					.ToListAsync();
				return _mapper.Map<List<AccountModel>>(accounts);
			}
			catch
			{
				return null;
			}
		}

		// Cập nhật tài khoản theo ID
		public async Task<bool> Update(int id, AccountModel model)
		{
			try
			{
				var accountEntity = await _context.Accounts.FindAsync(id);
				if (accountEntity == null)
					return false;

				_mapper.Map(model, accountEntity); // Ánh xạ từ model sang thực thể
				_context.Accounts.Update(accountEntity);
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
