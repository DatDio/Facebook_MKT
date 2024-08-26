using Facebook_MKT.Data;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Microsoft.EntityFrameworkCore;


var optionsBuilder = new DbContextOptionsBuilder<FBDataContext>();
var path = Path.GetFullPath("Facebook_MKT\\Facebook_MKT.DataFacebookMKT.db");
optionsBuilder.UseSqlite($"Data Source={Path.GetFullPath("Facebook_MKT\\Facebook_MKT.DataFacebookMKT.db")}");

// Khởi tạo FBDataContext với DbContextOptions
var context = new FBDataContext(optionsBuilder.Options);

IDataService<Account> _accountService = new GenericDataService<Account>(context);

var account = new Account
{
	UID = "2345324",
	Email1 = "test@gmail.com",
	Pages = new List<Page>
	{
		new Page
		{
			PageID = "Page1",
			PageName = "My Page 1",
			PageFollow = "1000",
			PageLike = "500",
			PageStatus = "Active"
		},
		new Page
		{
			PageID = "Page2",
			PageName = "My Page 2",
			PageFollow = "2000",
			PageLike = "1500",
			PageStatus = "Inactive"
		}
	}
};

_accountService.Create(account);

