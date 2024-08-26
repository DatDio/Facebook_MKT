using Facebook_MKT.WPF.State.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.WPF.ViewModels.Factories
{
	public interface IViewModelFactory
	{
		BaseViewModel CreateViewModel(ViewType viewType);
	}
}
