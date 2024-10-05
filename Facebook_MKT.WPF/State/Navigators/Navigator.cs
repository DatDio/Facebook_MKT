using Facebook_MKT.WPF.Commands;
using Facebook_MKT.WPF.Models;
using Facebook_MKT.WPF.ViewModels;
using Facebook_MKT.WPF.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Facebook_MKT.WPF.State.Navigators
{
	public class Navigator : ObverableObject, INavigator
	{
		private BaseViewModel _currentViewModel;

		public BaseViewModel CurrentViewModel
		{
			get
			{
				return _currentViewModel;
			}
			set
			{
				_currentViewModel?.Dispose();
			
				_currentViewModel = value;
				StateChanged?.Invoke();
			}
		}

		public event Action StateChanged;

	}
}
