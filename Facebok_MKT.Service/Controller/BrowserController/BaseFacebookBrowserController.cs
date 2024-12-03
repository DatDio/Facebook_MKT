using AutoMapper;
using Facebok_MKT.Service.DataService.Accounts;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using OpenQA.Selenium;
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
			for (int i = 1; i <= timeSleep; i++)
			{
				_pauseEvent.Wait();
				_cancellationTokenSource.Token.ThrowIfCancellationRequested();
				accountModel.Status = $"Nghỉ {timeSleep - i}s ...";
				await Task.Delay(1000);
			}

			return ResultModel.Success;
		}

		// Placeholder methods to be implemented in concrete subclasses
		protected async virtual Task<ResultModel> SwitchToProfile()
		{
			var pageSource = accountModel.Driver.PageSource;
			var _user = RegexHelper.GetValueFromGroup("__user=(.*?)&", pageSource);
			if (_user != accountModel.UID)
			{
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[aria-label=\"Trang cá nhân của bạn\"]")))
				{
					if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Trang cá nhân của bạn\"]")))
					{
						return ResultModel.Fail;
					}

					await Task.Delay(random.Next(3000, 5000));
					if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[role=\"listitem\"]")))
						return ResultModel.Fail;
					await Task.Delay(random.Next(5000, 10000));
					pageSource = accountModel.Driver.PageSource;
					_user = RegexHelper.GetValueFromGroup("__user=(.*?)&", pageSource);
					if (_user != accountModel.UID)
					{
						return ResultModel.Fail;
					}
				}
			}
			return ResultModel.Success;
		}

		public async virtual Task<ResultModel> CheckLogined()
		{
			url = accountModel.Driver.Url;
			if (!url.Contains("facebook"))
			{
				try
				{
					accountModel.Driver.Url = "https://www.facebook.com/";
				}
				catch
				{
					return ResultModel.Fail;
				}
			}

			url = accountModel.Driver.Url;
			if (url.Contains("956/"))
			{
				accountModel.Status = "CheckPoint 956!";
				await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
				return ResultModel.CheckPoint;
			}
			else if (url.Contains("282/"))
			{
				accountModel.Status = "CheckPoint 282!";
				await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
				return ResultModel.CheckPoint;
			}
			else if (url.Contains("checkpoint"))
			{
				accountModel.Status = "CheckPoint!";
				await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
				return ResultModel.CheckPoint;
			}
			accountModel.Status = "Đang check xem login chưa ...";
			if (SeleniumHelper.WaitElement(accountModel.Driver, By.Id("email"), 5))
			{
				//var status = LoginByCookie();
				accountModel.Status = "Đang login...";
				try
				{
					var emailElement = accountModel.Driver.FindElement(By.Id("email"));
					emailElement.Click();
					emailElement.Clear();
				}
				catch
				{
					return ResultModel.Fail;
				}

				if (accountModel.Driver.FindElement(By.Id("email")).Text == "")
				{
					if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("email"), accountModel.UID.ToString()))
						return ResultModel.Fail;
				}
				await Task.Delay(random.Next(2000, 5000));

				try
				{
					var passElement = accountModel.Driver.FindElement(By.Id("pass"));
					passElement.Click();
					Thread.Sleep(1000);
					passElement.Clear();
				}
				catch
				{
					return ResultModel.Fail;
				}

				if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("pass"), accountModel.Password))
					return ResultModel.Fail;
				await Task.Delay(random.Next(2000, 5000));
				if (!SeleniumHelper.Click(accountModel.Driver, By.Name("login")))
					return ResultModel.Fail;
				if (SeleniumHelper.UrlChange(accountModel.Driver, url))
				{
					if (accountModel.Driver.Url.Contains("https://www.facebook.com/checkpoint/?next")
						|| accountModel.Driver.Url.Contains("https://www.facebook.com/two_step_verification/two_factor"))
					{
						if (accountModel.C_2FA != "")
						{
							var code2FA = FunctionHelper.ConvertTwoFA(accountModel.C_2FA);
							if (SeleniumHelper.WaitElement(accountModel.Driver, By.Id("approvals_code"), 8))
							{
								if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("approvals_code"), code2FA))
								{
									return ResultModel.Fail;
								}
							}
							if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("input[type=\"text\"]")))
							{
								if (!SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[type=\"text\"]"), code2FA))
								{
									return ResultModel.Fail;
								}
							}

							await Task.Delay(random.Next(2000, 5000));
							if (!SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[type=\"text\"]"), Keys.Enter))
							{
								return ResultModel.Fail;
							}
							//if (!SeleniumHelper.Click(accountModel.Driver, By.Id("checkpointSubmitButton")))
							//	return ResultModel.Fail;
						}
					}
				}
				url = accountModel.Driver.Url;
				//Lưu trình duyệt
				accountModel.Status = "Đang đợi nút lưu trình duyệt ...";
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("button[name=\"submit[Continue]\"]"), 8))
				{
					SeleniumHelper.Click(accountModel.Driver, By.CssSelector("button[name=\"submit[Continue]\"]"));
				}
				await Task.Delay(random.Next(2000, 3000));
				//Luôn xác nhận đó là tôi
				accountModel.Status = "Đợi nút luôn xác nhận đó là tôi ...";
				try
				{
					var elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"none\"]"));
					foreach (var element in elements)
					{
						if (element.Text.Contains("Luôn xác nhận đó là tôi"))
						{
							element.Click();
							break;
						}
					}
				}
				catch
				{
					return ResultModel.Fail;
				}
				if (!SeleniumHelper.UrlChange(accountModel.Driver, url))
				{
					return ResultModel.Fail;
				}
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.Id("approvals_code")))
				{
					return ResultModel.Fail;
				}
				try
				{
					url = accountModel.Driver.Url;
				}
				catch
				{
					return ResultModel.Fail;
				}
				if (url != "https://www.facebook.com/")
				{
					return ResultModel.Fail;
				}
				var allCookies = accountModel.Driver.Manage().Cookies.AllCookies;
				var cookies = "";
				foreach (var cookie in allCookies)
				{
					string name = cookie.Name;
					string value = cookie.Value;
					cookies += name + "=" + value + ";";
				}
				accountModel.Cookie = cookies;
				await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
			}

			return ResultModel.Success;
		}

		protected async virtual Task<ResultModel> ReadNotification()
		{
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[href=\"/notifications/\"]"));
			Thread.Sleep(5000);
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[href=\"/notifications/\"]"));
			return ResultModel.Success;
		}
		protected async virtual Task<ResultModel> ReadMessenger()
		{
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Messenger\"]"));
			Thread.Sleep(5000);
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Messenger\"]"));
			Thread.Sleep(random.Next(10000, 20000));
			return ResultModel.Success;
		}
	}

}
