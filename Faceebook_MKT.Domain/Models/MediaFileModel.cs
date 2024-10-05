using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Models
{
	public class MediaFileModel
	{
		public string FilePath { get; set; }
		public bool IsImage { get; set; }
		public bool IsVideo { get; set; }
		public string ThumbnailPath { get; set; }
	}
}
