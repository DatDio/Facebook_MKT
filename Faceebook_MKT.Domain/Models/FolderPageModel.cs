using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Models
{
	public class FolderPageModel
	{
		private int _folderIdKey;
		public int FolderIdKey
		{
			get { return _folderIdKey; }
			set
			{
				if (_folderIdKey != value)
				{
					_folderIdKey = value;
					OnPropertyChanged(nameof(FolderIdKey));
				}
			}
		}

		private string _folderName;
		public string FolderName
		{
			get { return _folderName; }
			set
			{
				if (_folderName != value)
				{
					_folderName = value;
					OnPropertyChanged(nameof(FolderIdKey));
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
