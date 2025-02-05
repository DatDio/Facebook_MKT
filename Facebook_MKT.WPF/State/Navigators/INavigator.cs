﻿using Facebook_MKT.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Facebook_MKT.WPF.State.Navigators
{
	public enum ViewType
	{
		Page,
		PagePost,
		PageInteract,
		Group,
		AccountInteract,
		GeneralSettings,
		Other
	}

	public interface INavigator
	{
		BaseViewModel CurrentViewModel { get; set; }
		event Action StateChanged;

	}
}
