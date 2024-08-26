using Facebook_MKT.WPF.State.Navigators;
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
		public UpdateCurrentViewModelCommand(INavigator navigation, IViewModelFactory viewModelFactory)
		{
			_navigation = navigation;
			_viewModelFactory = viewModelFactory;
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
				//_navigation.SelectedViewType = viewType;
				_navigation.CurrentViewModel = _viewModelFactory.CreateViewModel(viewType);
			}
		}

		//public void Execute(object? parameter)
		//{
		//	if (parameter is ViewType)
		//	{

		//		ViewType viewType = (ViewType)parameter;
		//		_navigation.SelectedViewType = viewType;
		//		switch (viewType)
		//		{
		//			case ViewType.Page:
		//				_navigation.CurrentViewModel = new PageViewModel();
		//				break;
		//			case ViewType.PagePost:
		//				_navigation.CurrentViewModel = new PagePostViewModel();
		//				break;
		//			case ViewType.PageInteract:
		//				_navigation.CurrentViewModel = new PageInteractViewModel();
		//				break;
		//			case ViewType.Group:
		//				_navigation.CurrentViewModel = new GroupViewModel();
		//				break;
		//			default:
		//				//throw new Exception("Lỗi xảy ra");
		//				MessageBox.Show("Có lỗi xảy ra");
		//				break;
		//		}
		//	}
		//}
	}
}
