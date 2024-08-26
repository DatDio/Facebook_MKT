using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Facebook_MKT.WPF.ViewModels.Combobox;
using Facebook_MKT.WPF.ViewModels.DataGrid;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Helpers.ConvertToModel;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Facebook_MKT.WPF.Commands.LoadDataGrid
{
	public class LoadDataGridAccountCommand : BaseCommand
	{
		private readonly IDataService<Account> _dataService;
		private readonly FolderDataViewModel<Folder> _folderAccountViewModel;
		private readonly ObservableCollection<AccountModel> _accounts;
		private readonly ObservableCollection<AccountModel> _accountsSelected;
		private readonly IEntityToModelConverter<Account, AccountModel> _converter;
		public LoadDataGridAccountCommand(IDataService<Account> dataService,
			ObservableCollection<AccountModel> accounts,
			ObservableCollection<AccountModel> accountsSelected,
			IEntityToModelConverter<Account, AccountModel> converter,
			FolderDataViewModel<Folder> folderAccountViewModel)
		{
			_dataService = dataService;
			_accounts = accounts;
			_accountsSelected = accountsSelected;
			_converter = converter;
			_folderAccountViewModel = folderAccountViewModel;
		}

		public override async Task ExecuteAsync(object parameter)
		{
			var folderidKey = _folderAccountViewModel._selectedItem.FolderIdKey;
			//var data = await _dataService.GetByFolderIdKey(folderIdKey);
			var data = await _dataService.GetByFolderIdKey(folderidKey);
			_accounts.Clear();
			_accountsSelected.Clear();
			var accountModels = data.Select(account => _converter.Convert(account)).ToList();
			foreach (var accountModel in accountModels)
			{
				_accounts.Add(accountModel);
			}
		}
	}
}
