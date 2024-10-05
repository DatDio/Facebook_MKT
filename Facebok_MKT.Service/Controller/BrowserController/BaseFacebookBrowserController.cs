using AutoMapper;
using Facebok_MKT.Service.DataService.Accounts;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.Controller.BrowserController
{
	public abstract class BaseFacebookBrowserController
	{
		protected ManualResetEventSlim _pauseEvent;
		protected CancellationTokenSource _cancellationTokenSource;

		protected Random random;
		protected AccountModel accountModel;
		protected readonly IAccountDataService _accountDataService;
		protected string url;

		public BaseFacebookBrowserController(AccountModel accountModel,
			IAccountDataService accountDataService,
			ManualResetEventSlim pauseEvent,
			CancellationTokenSource cancellationTokenSource)
		{
			this.accountModel = accountModel;
			_accountDataService = accountDataService;
			random = new Random();
			_pauseEvent = pauseEvent;
			_cancellationTokenSource = cancellationTokenSource;
		}

		// Abstract method for executing a task, to be implemented by subclasses
		public abstract Task<ResultModel> ExecuteTask(TaskModel task);

		// Method to initialize browser and profile switching
		public async virtual Task<ResultModel> Initialization()
		{
			for (int i = 0; i <= 2; i++)
			{
				var logined = await CheckLogined();
				if (logined != ResultModel.Fail)
				{
					break;
				}
				if (i == 2)
				{
					return ResultModel.Fail;
				}
			}
			for (int i = 0; i <= 2; i++)
			{
				var switchProfile = await SwitchToProfile();
				if (switchProfile != ResultModel.Fail)
				{
					return ResultModel.Success;
				}
				if (i == 2)
				{
					accountModel.Status = "switch profile thất bại!";
					_accountDataService.Update(accountModel.AccountIDKey, accountModel);
					return ResultModel.Fail;
				}
			}
			return ResultModel.Success;
		}

		// Shared sleep method
		protected async Task<ResultModel> SleepThread(int timeSleepFrom, int timeSleepTo)
		{
			var timeSleep = random.Next(timeSleepFrom, timeSleepTo);
			await Task.Delay(timeSleep * 1000);
			return ResultModel.Success;
		}

		// Placeholder methods to be implemented in concrete subclasses
		protected abstract Task<ResultModel> CheckLogined();
		protected abstract Task<ResultModel> SwitchToProfile();
	}

}
