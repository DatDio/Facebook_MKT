using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using System.ComponentModel;

public class PageModel : INotifyPropertyChanged, ISelectable
{
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



	private string _pageID;
	public string PageID
	{
		get { return _pageID; }
		set
		{
			if (_pageID != value)
			{
				_pageID = value;
				OnPropertyChanged(nameof(PageID));
			}
		}
	}

	public int AccountIDKey { get; set; }
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
	private string _pageName;
	public string PageName
	{
		get { return _pageName; }
		set
		{
			if (_pageName != value)
			{
				_pageName = value;
				OnPropertyChanged(nameof(PageName));
			}
		}
	}

	private string _pageFollow;
	public string PageFollow
	{
		get { return _pageFollow; }
		set
		{
			if (_pageFollow != value)
			{
				_pageFollow = value;
				OnPropertyChanged(nameof(PageFollow));
			}
		}
	}

	private string _pageFolderVideo;
	public string PageFolderVideo
	{
		get { return _pageFolderVideo; }
		set
		{
			if (_pageFolderVideo != value)
			{
				_pageFolderVideo = value;
				OnPropertyChanged(nameof(PageFolderVideo));
			}
		}
	}

	// Thuộc tính mới để đếm số video
	private int _videoCount;
	public int VideoCount
	{
		get
		{
			UpdateVideoCount();
			return _videoCount;
		}
		set
		{
			if (_videoCount != value)
			{
				_videoCount = value;
				OnPropertyChanged(nameof(VideoCount));
				UpdateVideoCount();
			}
		}
	}

	private void UpdateVideoCount()
	{
		// Logic để tính số lượng video trong folder PageFolderVideo
		// Giả sử bạn có một phương thức GetVideoCountFromFolder trả về số lượng video
		VideoCount = FolderHelper.GetVideoCountFromFolder(PageFolderVideo);
	}

	private string _pageFolderName;
	public string PageFolderName
	{
		get { return _pageFolderName; }
		set
		{
			if (_pageFolderName != value)
			{
				_pageFolderName = value;
				OnPropertyChanged(nameof(PageFolderName));
			}
		}
	}
	private string _pageLike;
	public string PageLike
	{
		get { return _pageLike; }
		set
		{
			if (_pageLike != value)
			{
				_pageLike = value;
				OnPropertyChanged(nameof(PageLike));
			}
		}
	}

	private string _pageStatus;
	public string PageStatus
	{
		get { return _pageStatus; }
		set
		{
			if (_pageStatus != value)
			{
				_pageStatus = value;
				OnPropertyChanged(nameof(PageStatus));
			}
		}
	}

	public int FolderIdKey { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

}
