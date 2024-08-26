using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.Data.Entities
{
	public class Page
	{
		public int PageIdKey { get; set; }
		public string PageID { get; set; }
		public int AccountID { get; set; }
		public string PageName { get; set; }
		public string PageFollow { get; set; }
		public string PageLike { get; set; }
		public string PageStatus { get; set; }
		public int FolderIdKey { get; set; }
		public virtual Folder Folder { get; set; }
		public virtual Account Account { get; set; }
	}
}
