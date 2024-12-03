using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Models
{
	public class GroupModel : INotifyPropertyChanged, ISelectable
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


		private string _textColor;
		public string TextColor
		{
			get => _textColor;
			set
			{
				_textColor = value;
				OnPropertyChanged(nameof(TextColor));
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
					OnPropertyChanged(nameof(GroupID));
				}
			}
		}

		private bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					OnPropertyChanged(nameof(IsSelected));
				}
			}
		}

		private string _groupName;
		public string GroupName
		{
			get { return _groupName; }
			set
			{
				if (_groupName != value)
				{
					_groupName = value;
					OnPropertyChanged(nameof(GroupName));
				}
			}
		}
		private string? _groupMember;
		public string? GroupMember
		{
			get { return _groupMember; }
			set
			{
				if (_groupMember != value)
				{
					_groupMember = value;
					OnPropertyChanged(nameof(GroupMember));
				}
			}
		}

		private string? _groupCensor;
		public string? GroupCensor
		{
			get { return _groupCensor; }
			set
			{
				if (_groupCensor != value)
				{
					_groupCensor = value;
					OnPropertyChanged(nameof(GroupCensor));
				}
			}
		}

		private string? _typeGroup;
		public string? TypeGroup
		{
			get { return _typeGroup; }
			set
			{
				if (_typeGroup != value)
				{
					_typeGroup = value;
					OnPropertyChanged(nameof(TypeGroup));
				}
			}
		}

		private string? _groupStatus;
		public string? GroupStatus
		{
			get { return _groupStatus; }
			set
			{
				if (_groupStatus != value)
				{
					_groupStatus = value;
					OnPropertyChanged(nameof(GroupStatus));
				}
			}
		}

		private string _groupFolderName;
		public string GroupFolderName
		{
			get { return _groupFolderName; }
			set
			{
				if (_groupFolderName != value)
				{
					_groupFolderName = value;
					OnPropertyChanged(nameof(GroupFolderName));
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
