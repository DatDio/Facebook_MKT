using Facebook_MKT.WPF.State.Navigators;
using Facebook_MKT.WPF.ViewModels.Groups;
using Facebook_MKT.WPF.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Facebook_MKT.WPF.ViewModels.Factories
{
	public class ViewModelFactory : IViewModelFactory
	{
		private readonly CreateViewModel<PageViewModel> _createPageViewModel;
		private readonly CreateViewModel<PagePostViewModel> _createPagePostViewModel;
		private readonly CreateViewModel<PageInteractViewModel> _createPageInteractViewModel;
		private readonly CreateViewModel<GroupViewModel> _createGroupViewModel;
		//private readonly CreateViewModel<SellViewModel> _createSellViewModel;
		public ViewModelFactory(CreateViewModel<PageViewModel> createPageViewModel,
								CreateViewModel<PagePostViewModel> createPagePostViewModel,
								CreateViewModel<PageInteractViewModel> createPageInteractViewMode,
								CreateViewModel<GroupViewModel> createGroupViewModel)
		{
			_createPageViewModel = createPageViewModel;
			_createPagePostViewModel = createPagePostViewModel;
			_createPageInteractViewModel = createPageInteractViewMode;
			_createGroupViewModel = createGroupViewModel;

		}
		public BaseViewModel CreateViewModel(ViewType viewType)
		{
			switch (viewType)
			{
				case ViewType.Page:
					
					return _createPageViewModel();
				case ViewType.PagePost:
					return _createPagePostViewModel();
				case ViewType.PageInteract:
					return _createPageInteractViewModel();
				case ViewType.Group:
					return _createGroupViewModel();
				default:
					//throw new Exception("Lỗi xảy ra");
					throw new ArgumentException("The ViewType does not have a ViewModel.", "viewType");
			}
		}
	}
}
