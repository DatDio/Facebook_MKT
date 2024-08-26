using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Facebook_MKT.WPF.State.Navigators;
using Facebook_MKT.WPF.ViewModels.Pages;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Facebook_MKT.WPF.Commands
{
	public class AddCommand : BaseCommand 
	{
		private readonly IDataService<Account> _dataService;
		public AddCommand(IDataService<Account> dataService)
		{
			_dataService = dataService;
		}
		public override async Task ExecuteAsync(object parameter)
		{
			List<Account> listAccount = new List<Account>();
			var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
			if (items.Count==0){
				MessageBox.Show("Không có dữ liệu trong clipboard!");
				return;
			}
			if (parameter is AddAccountType)
			{
				AddAccountType addAccountType = (AddAccountType)parameter;
				switch (addAccountType)
				{
					case AddAccountType.UID_Pass_2FA:
						for (int i = 0; i < items.Count; i++)
						{
							try
							{
								var item = items[i].Split('|');

								listAccount.Add(new Account
								{
									UID = item[0].Trim(),
									Password = item[1].Trim(),
									C_2FA = item[2].Trim(),
									//C_Folder = cbbFolderManageAcc.Text
								});
							}
							catch
							{
							}
						}
						break;
					case AddAccountType.UID_Pass_2FA_Cookie:
						break;
					case AddAccountType.UID_Pass_2FA_Cookie_Token:
						break;
					case AddAccountType.UID_Pass_2FA_Cookie_Token_Email_PassEmail:
						break;
					default:
						throw new NotImplementedException();
				}
				foreach (var account in listAccount)
				{
					await _dataService.Create(account);
				}
			}
			else
			{
				MessageBox.Show("Có lỗi xảy ra!");
			}
		}
	}
}
