using Facebook_MKT.WPF.State.Navigators;
using Facebook_MKT.WPF.ViewModels;
using Facebook_MKT.WPF.ViewModels.Factories;
using Facebook_MKT.WPF.ViewModels.Groups;
using Facebook_MKT.WPF.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Facebook_MKT.WPF.Commands
{
	public class UpdateCurrentViewModelCommand : ICommand
	{
		public event EventHandler? CanExecuteChanged;
		private readonly INavigator _navigation;
		private readonly IViewModelFactory _viewModelFactory;
		public readonly ViewType _seletedViewType;
		private readonly MainViewModel _mainViewModel;
		public UpdateCurrentViewModelCommand(INavigator navigation, 
			IViewModelFactory viewModelFactory,
			ViewType seletedViewType,
			MainViewModel mainViewModel)
		{
			_navigation = navigation;
			_viewModelFactory = viewModelFactory;
			_seletedViewType = seletedViewType;
		}

		public bool CanExecute(object? parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			if (parameter is ViewType)
			{
				ViewType viewType = (ViewType)parameter;
				//_seletedViewType = viewType;
				_mainViewModel.SelectedViewType = viewType;
				_navigation.CurrentViewModel = _viewModelFactory.CreateViewModel(viewType);
			}
		}
	}
}
