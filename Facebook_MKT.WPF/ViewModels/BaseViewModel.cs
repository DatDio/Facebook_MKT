﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Facebook_MKT.WPF.ViewModels
{
	public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : BaseViewModel;
	public class BaseViewModel : INotifyPropertyChanged
	{
		public virtual void Dispose() { }

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	class RelayCommand<T> : ICommand
	{
		private readonly Predicate<T> _canExecute;
		private readonly Action<T> _execute;

		public RelayCommand(Predicate<T> canExecute, Action<T> execute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");
			_canExecute = canExecute;
			_execute = execute;
		}

		public bool CanExecute(object parameter)
		{
			try
			{
				return _canExecute == null ? true : _canExecute((T)parameter);
			}
			catch
			{
				return true;
			}
		}

		public async void Execute(object parameter)
		{
			_execute((T)parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}
