﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Models
{
	public class GroupModel
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



		private string _groupID;
		public string GroupID
		{
			get { return _groupID; }
			set
			{
				if (_groupID != value)
				{
					_groupID = value;
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
