using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Models
{
	public class AccountModel : INotifyPropertyChanged, ISelectable
	{
		private bool _isSelected;
		public int AccountIDKey { get; set; }
		private ChromeDriver _driver;
		private string? _uid;
		private string? _password;
		private string? _c_2fa;
		private string? _email1;
		private string? _email1Password;
		private string? _email2;
		private string? _email2Password;
		private string? _token;
		private string? _cookie;
		private string? _status;
		private string? _accountFolderName;
		private string? _proxy;
		private string? _userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36";
		private string? _gpmId;
		private int _folderIdKey;
		private List<PageModel> _listPageModels;
		public List<PageModel> Pages
		{
			get
			{
				return _listPageModels;
			}
			set
			{
				_listPageModels = value;
				OnPropertyChanged(nameof(Pages));
			}
		}
		public ChromeDriver Driver
		{
			get { return _driver; }
			set
			{
				if (_driver != value)
				{
					_driver = value;
					OnPropertyChanged(nameof(Driver));
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
		//private Color _rowColor;
		//public Color RowColor
		//{
		//	get => _rowColor;
		//	set
		//	{
		//		_rowColor = value;
		//		OnPropertyChanged(nameof(RowColor));
		//	}
		//}

		public string? UID
		{
			get { return _uid; }
			set
			{
				if (_uid != value)
				{
					_uid = value;
					OnPropertyChanged(nameof(UID));
				}
			}
		}

		public string? Password
		{
			get { return _password; }
			set
			{
				if (_password != value)
				{
					_password = value;
					OnPropertyChanged(nameof(Password));
				}
			}
		}
		public string? AccountFolderName
		{
			get { return _accountFolderName; }
			set
			{
				if (_accountFolderName != value)
				{
					_accountFolderName = value;
					OnPropertyChanged(nameof(AccountFolderName));
				}
			}
		}

		public string? C_2FA
		{
			get { return _c_2fa; }
			set
			{
				if (_c_2fa != value)
				{
					_c_2fa = value;
					OnPropertyChanged(nameof(C_2FA));
				}
			}
		}

		public string? Email1
		{
			get { return _email1; }
			set
			{
				if (_email1 != value)
				{
					_email1 = value;
					OnPropertyChanged(nameof(Email1));
				}
			}
		}

		public string? Email1Password
		{
			get { return _email1Password; }
			set
			{
				if (_email1Password != value)
				{
					_email1Password = value;
					OnPropertyChanged(nameof(Email1Password));
				}
			}
		}

		public string? Email2
		{
			get { return _email2; }
			set
			{
				if (_email2 != value)
				{
					_email2 = value;
					OnPropertyChanged(nameof(Email2));
				}
			}
		}

		public string? Email2Password
		{
			get { return _email2Password; }
			set
			{
				if (_email2Password != value)
				{
					_email2Password = value;
					OnPropertyChanged(nameof(Email2Password));
				}
			}
		}

		public string? Token
		{
			get { return _token; }
			set
			{
				if (_token != value)
				{
					_token = value;
					OnPropertyChanged(nameof(Token));
				}
			}
		}

		public string? Cookie
		{
			get { return _cookie; }
			set
			{
				if (_cookie != value)
				{
					_cookie = value;
					OnPropertyChanged(nameof(Cookie));
				}
			}
		}

		public string? Status
		{
			get { return _status; }
			set
			{
				if (_status != value)
				{
					_status = value;
					OnPropertyChanged(nameof(Status));
				}
			}
		}

		public string? Proxy
		{
			get { return _proxy; }
			set
			{
				if (_proxy != value)
				{
					_proxy = value;
					OnPropertyChanged(nameof(Proxy));
				}
			}
		}

		public string? UserAgent
		{
			get { return _userAgent; }
			set
			{
				if (_userAgent != value)
				{
					_userAgent = value;
					OnPropertyChanged(nameof(UserAgent));
				}
			}
		}

		public string? GPMID
		{
			get { return _gpmId; }
			set
			{
				if (_gpmId != value)
				{
					_gpmId = value;
					OnPropertyChanged(nameof(GPMID));
				}
			}
		}

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


		public List<PageModel> GetSelectedPages()
		{
			var pageSelected = Pages.Where(page => page.IsSelected).ToList();
			return Pages.Where(page => page.IsSelected).ToList();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
