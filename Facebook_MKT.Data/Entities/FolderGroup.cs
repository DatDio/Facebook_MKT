using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Facebook_MKT.Data.Entities
{
	public class FolderGroup
	{
		public FolderGroup()
		{
			Groups = new HashSet<Group>();
		}
		public int FolderIdKey { get; set; }
		public string FolderName { get; set; }
		//public virtual ICollection<Account> Accounts { get; set; }
		public virtual ICollection<Group> Groups { get; set; }
	}

}
