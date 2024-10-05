using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.Data.Entities
{
	public class Page
	{
		//public int PageIdKey { get; set; }
		public string PageID { get; set; }
		public int AccountIDKey { get; set; }
		public string PageName { get; set; }
		public string? PageFollow { get; set; }
		public string? PageLike { get; set; }
		public string? PageStatus { get; set; }
		public string  PageFolderVideo { get; set; }
		public string? ViewVideo1 { get; set; }
		public string? ViewVideo2 { get; set; }
		public string? ViewVideo3 { get; set; }
		public string? ViewVideo4 { get; set; }
		public string? ViewVideo5 { get; set; }
		public int FolderIdKey { get; set; }
		//public string PageFolderVideo { get; set; }
		public virtual FolderPage FolderPage { get; set; }
		public virtual Account Account { get; set; }
	}
}
