using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.Data.Entities
{
	public class Folder 
	{
		public Folder()
		{
			Accounts = new HashSet<Account>();
			//Pages = new HashSet<Page>();
		}
		public int FolderIdKey { get; set; }
		public string FolderName { get; set; }
		public virtual ICollection<Account> Accounts { get; set; }
		//public virtual ICollection<Page> Pages { get; set; }
	}
}
