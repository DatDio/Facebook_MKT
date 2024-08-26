using Facebook_MKT.WPF.Commands;
using Facebook_MKT.WPF.State.Navigators;
using Facebook_MKT.WPF.ViewModels.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Facebook_MKT.WPF.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		private readonly IViewModelFactory _viewModelFactory;
		private readonly INavigator _navigator;
		public BaseViewModel CurrentViewModel => _navigator.CurrentViewModel;
		public ICommand UpdateCurrentViewModelCommand { get; }
		public MainViewModel(IViewModelFactory viewModelFactory,
							INavigator navigator)
		{
			_viewModelFactory = viewModelFactory;
			_navigator = navigator;

			_navigator.StateChanged += Navigator_StateChanged;

			UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, _viewModelFactory);
			UpdateCurrentViewModelCommand.Execute(ViewType.Page);
		}

		private ViewType _selectedViewType;
		public ViewType SelectedViewType
		{
			get
			{
				if (_selectedViewType == null)
				{
					return _selectedViewType = ViewType.Page;
				}
				return _selectedViewType;
			}

			set
			{
				_selectedViewType = value;
				OnPropertyChanged(nameof(SelectedViewType));
			}
		}

		private void Navigator_StateChanged()
		{
			
			OnPropertyChanged(nameof(CurrentViewModel));
		}
		public override void Dispose()
		{
			_navigator.StateChanged -= Navigator_StateChanged;

			base.Dispose();
		}
		//public INavigator navigator { get; set; } = new Navigator();
	}
}
