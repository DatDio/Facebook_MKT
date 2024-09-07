using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.WPF.ViewModels.General_settings
{
	public class GeneralSettingsViewModel : BaseViewModel
	{

		public GeneralSettingsViewModel()
		{

		}

		private int _scale = 1; // Giá trị mặc định
		public int Scale
		{
			get { return _scale; }
			set
			{
				_scale = value;
				OnPropertyChanged(nameof(Scale));
			}
		}
		//http://127.0.0.1:19995
		private string _apiURL = "http://127.0.0.1:19995"; // Giá trị mặc định
		public string APIURL
		{
			get { return _apiURL; }
			set
			{
				_apiURL = value;
				OnPropertyChanged(nameof(APIURL));
			}
		}

	}
}
