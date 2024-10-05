using Facebook_MKT.WPF.Commands;
using Facebook_MKT.WPF.State.Navigators;
using Facebook_MKT.WPF.ViewModels.Factories;
using Faceebook_MKT.Domain.Models;
using Faceebook_MKT.Domain.Systems;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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

		private ViewType _selectedViewType;
		public ViewType SelectedViewType
		{
			get
			{

				return _selectedViewType;
			}

			set
			{
				_selectedViewType = value;
				OnPropertyChanged(nameof(SelectedViewType));
			}
		}
		public MainViewModel(IViewModelFactory viewModelFactory,
							INavigator navigator)
		{
			if (!Directory.Exists(SystemContants.FolderVideoPage))
			{
				Directory.CreateDirectory(SystemContants.FolderVideoPage);
			}
			_viewModelFactory = viewModelFactory;
			_navigator = navigator;

			_navigator.StateChanged += Navigator_StateChanged;
			#region UpdateCurrentViewModelCommand
			UpdateCurrentViewModelCommand = new RelayCommand<ViewType>((p) =>
			{
				return true;

			}, async (p) =>
			{
				if (p is ViewType)
				{
					ViewType viewType = (ViewType)p;
					SelectedViewType = viewType;
					_navigator.CurrentViewModel = _viewModelFactory.CreateViewModel(viewType);
				}
			});
			UpdateCurrentViewModelCommand.Execute(ViewType.Page);
			#endregion
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
	}
}
