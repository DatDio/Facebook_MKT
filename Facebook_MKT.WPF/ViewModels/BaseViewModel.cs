﻿using AutoMapper;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.WPF.ViewModels.General_settings;
using Faceebook_MKT.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Facebook_MKT.WPF.ViewModels
{
	public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : BaseViewModel;
	public class BaseViewModel : INotifyPropertyChanged, IPropertyFileds
	{
		public static object lockFolder;
		public BaseViewModel()
		{
			lockFolder = new object();
			CloseAllChromeCommand = new RelayCommand<object>((a) =>
			{
				return true;
			},
			async (a) =>
			{
				var Processs = new List<Process>();
				Processs.AddRange(Process.GetProcessesByName("chromedriver"));
				Processs.AddRange(Process.GetProcessesByName("chrome"));
				foreach (var pr in Processs)
				{
					try
					{
						pr.Kill();
					}
					catch
					{

					}
				}
			});
		}
		protected ResultModel ResultStatus { get; set; }
		protected double _scale;
		protected string _apiGPMUrl;


		//Chạy mỗi luồng random task trong tasklist đã chọn
		private bool randomTaskInTaskList = false;
		public bool RandomTaskInTaskList
		{
			get { return randomTaskInTaskList; }
			set
			{
				randomTaskInTaskList = value;
				OnPropertyChanged(nameof(RandomTaskInTaskList));
			}
		}


		private int _maxParallelTasks = 1;
		public int MaxParallelTasks
		{
			get { return _maxParallelTasks; }
			set
			{
				_maxParallelTasks = value;
				OnPropertyChanged(nameof(MaxParallelTasks));
			}
		}

		public string PauseButtonContent => IsRunning ? "Pause" : "Resume";

		protected bool _isRunning = false;
		public bool IsRunning
		{
			get => _isRunning;
			set
			{
				_isRunning = value;
				OnPropertyChanged(nameof(IsRunning));
				OnPropertyChanged(nameof(PauseButtonContent));
			}
		}


		public ICommand StopCommand { get; set; }
		public ICommand PauseCommand { get; set; }
		public ICommand CloseAllChromeCommand { get; set; }


		protected ManualResetEventSlim _pauseEvent;
		protected CancellationTokenSource _cancellationTokenSource;


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


	public class ThreadController
	{
		public delegate void ThreadAction();
		public Task _Task;
		CancellationTokenSource src;
		PauseTokenSource pauseSource;
		public bool StopTask()
		{
			if (_Task == null)
				return true;
			try
			{
				if (src == null)
					return true;
				src.Cancel();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void TaskWait(CancellationTokenSource TokenSrouce, PauseTokenSource PauseSource)
		{
			var ct = StartTask(async () =>
			{

				while (true)
				{
					try
					{
						TokenSrouce.Token.ThrowIfCancellationRequested();
					}
					catch
					{
						return;
					}
					await PauseSource.Token.PauseIfRequestedAsync();

					await Task.Delay(TimeSpan.FromSeconds(1));
				}
			}, TokenSrouce, PauseSource);
		}

		public bool StartTask(ThreadAction action, CancellationTokenSource TokenSrouce, PauseTokenSource PauseSource)
		{
			if (_Task != null)
			{
				StopTask();
				_Task = null;
			}
			try
			{
				src = TokenSrouce;
				pauseSource = PauseSource;


				if (TokenSrouce == null)
				{
					_Task = Task.Run(() => { action(); });
				}
				else
				{
					_Task = Task.Run(() => { action(); }, TokenSrouce.Token);
				}



				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> PauseTask()
		{
			if (_Task == null)
				return true;

			try
			{
				await pauseSource.PauseAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> ResumeTask()
		{
			if (_Task == null)
				return true;

			try
			{
				await pauseSource.ResumeAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
	public class PauseTokenSource
	{
		bool _paused = false;
		bool _pauseRequested = false;

		TaskCompletionSource<bool> _resumeRequestTcs;
		TaskCompletionSource<bool> _pauseConfirmationTcs;

		readonly SemaphoreSlim _stateAsyncLock = new SemaphoreSlim(1);
		readonly SemaphoreSlim _pauseRequestAsyncLock = new SemaphoreSlim(1);

		public PauseToken Token { get { return new PauseToken(this); } }

		public async Task<bool> IsPaused(CancellationToken token = default(CancellationToken))
		{
			await _stateAsyncLock.WaitAsync(token);
			try
			{
				return _paused;
			}
			finally
			{
				_stateAsyncLock.Release();
			}
		}

		public async Task ResumeAsync(CancellationToken token = default(CancellationToken))
		{
			await _stateAsyncLock.WaitAsync(token);
			try
			{
				if (!_paused)
				{
					return;
				}

				await _pauseRequestAsyncLock.WaitAsync(token);
				try
				{
					var resumeRequestTcs = _resumeRequestTcs;
					_paused = false;
					_pauseRequested = false;
					_resumeRequestTcs = null;
					_pauseConfirmationTcs = null;
					resumeRequestTcs.TrySetResult(true);
				}
				finally
				{
					_pauseRequestAsyncLock.Release();
				}
			}
			finally
			{
				_stateAsyncLock.Release();
			}
		}

		public async Task PauseAsync(CancellationToken token = default(CancellationToken))
		{
			await _stateAsyncLock.WaitAsync(token);
			try
			{
				if (_paused)
				{
					return;
				}

				Task pauseConfirmationTask = null;

				await _pauseRequestAsyncLock.WaitAsync(token);
				try
				{
					_pauseRequested = true;
					_resumeRequestTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
					_pauseConfirmationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
					pauseConfirmationTask = WaitForPauseConfirmationAsync(token);
				}
				finally
				{
					_pauseRequestAsyncLock.Release();
				}

				await pauseConfirmationTask;

				_paused = true;
			}
			finally
			{
				_stateAsyncLock.Release();
			}
		}

		private async Task WaitForResumeRequestAsync(CancellationToken token)
		{
			using (token.Register(() => _resumeRequestTcs.TrySetCanceled(), useSynchronizationContext: false))
			{
				await _resumeRequestTcs.Task;
			}
		}

		private async Task WaitForPauseConfirmationAsync(CancellationToken token)
		{
			using (token.Register(() => _pauseConfirmationTcs.TrySetCanceled(), useSynchronizationContext: false))
			{
				await _pauseConfirmationTcs.Task;
			}
		}

		internal async Task PauseIfRequestedAsync(CancellationToken token = default(CancellationToken))
		{
			Task resumeRequestTask = null;

			await _pauseRequestAsyncLock.WaitAsync(token);
			try
			{
				if (!_pauseRequested)
				{
					return;
				}
				resumeRequestTask = WaitForResumeRequestAsync(token);
				_pauseConfirmationTcs.TrySetResult(true);
			}
			finally
			{
				_pauseRequestAsyncLock.Release();
			}

			await resumeRequestTask;
		}
	}

	// PauseToken - consumer side
	public struct PauseToken
	{
		readonly PauseTokenSource _source;

		public PauseToken(PauseTokenSource source) { _source = source; }

		public Task<bool> IsPaused() { return _source.IsPaused(); }

		public Task PauseIfRequestedAsync(CancellationToken token = default(CancellationToken))
		{
			return _source.PauseIfRequestedAsync(token);
		}
	}
}
