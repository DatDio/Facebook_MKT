using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.Data.Entities
{
	public class Group
	{
		//public int GroupIdKey { get; set; }
		public string GroupID { get; set; }
		//public int AccountID { get; set; }
		public string GroupName { get; set; }
		public string? GroupMember { get; set; }
		public string? GroupCensor { get; set; }
		public string? TypeGroup { get; set; }
		public string? GroupStatus { get; set; }
		public int FolderIdKey { get; set; }
		public virtual FolderGroup FolderGroup { get; set; }
		//public virtual Account Account { get; set; }
	}
}
