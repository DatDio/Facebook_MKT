using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.Data.Entities
{
	public class Account
	{
		public Account()
		{
			Pages = new HashSet<Page>();
		}
		public int AccountIDKey { get; set; }
		public string? UID { get; set; }
		public string? Password { get; set; }
		public string? C_2FA { get; set; }

		public string? Email1 { get; set; }
		public string? Email1Password { get; set; }
		public string? Email2 { get; set; }
		public string? Email2Password { get; set; }
		public string? Token { get; set; }
		public string? Cookie { get; set; }
		public string? Status { get; set; }
		public string? Proxy { get; set; }
		public string? UserAgent { get; set; }
		public string? GPMID { get; set; }
		public int FolderIdKey { get; set; }
		public virtual Folder Folder { get; set; }
		public virtual ICollection<Page>? Pages { get; set; }
	}
}
