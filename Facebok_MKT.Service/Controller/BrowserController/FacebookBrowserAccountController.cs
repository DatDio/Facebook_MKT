using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using Facebok_MKT.Service.DataService.Accounts;
using System.Diagnostics;

namespace Facebok_MKT.Service.Controller.BrowserController
{
	public class FacebookBrowserAccountController : BaseFacebookBrowserController
	{

		private string url;
		//FacebookAPIController apiFB;
		public FacebookBrowserAccountController(AccountModel accountModel,
								IAccountDataService _accountDataService,
			ManualResetEventSlim pauseEvent,
			CancellationTokenSource cancellationTokenSource) : base(accountModel, _accountDataService, pauseEvent, cancellationTokenSource)
		{

		}

		public override async Task<ResultModel> ExecuteTask(TaskModel task)
		{
			switch (task.TaskName)
			{
				case "Đăng nhập":
					return await LoginUIDPass();
				case "Nghỉ":
					return await SleepThread(Convert.ToInt32(task.Fields[0].Value),
										Convert.ToInt32(task.Fields[1].Value));
				case "Lướt New Feed":
					return await SurfNewFeed(Convert.ToInt32(task.Fields[0].Value),
						Convert.ToInt32(task.Fields[1].Value));
				//case "Like":
				//	return LikePost(Convert.ToInt32(task.Fields[0].Value));
				//case "Comment":
				//	return CommentPost(Convert.ToInt32(task.Fields[0].Value), task.Fields[2].Value.ToString());
				//case "Kết bạn theo từ khóa":
				//	return MakeFriendsByKeyWord((task.Fields[0].Value).ToString(), Convert.ToInt32(task.Fields[1].Value));
				//case "Kết bạn theo gợi ý":
				//	return MakeFriendsBySuggestion(Convert.ToInt32(task.Fields[0].Value));
				//case "Đọc tin nhắn":
				//	return ReadMessenger();
				//case "Đọc thông báo":
				//	return ReadNotification();
				//case "Đăng bài":
				//	var f = task.Fields[1].Label.ToString();
				//	var c = task.Fields[2].Value.ToString();
				//	return UpPost(task.Fields[1].Label.ToString(), task.Fields[2].Value.ToString());
				//case "Tham gia nhóm theo từ khóa:":
				//	return JoinGroupByKeyWord((task.Fields[0].Value).ToString(), Convert.ToInt32(task.Fields[1].Value));
				default:
					return ResultModel.Fail;
			}
		}

		public override async Task<ResultModel> Initialization()
		{
			for (int i = 0; i <= 2; i++)
			{
				var logined = await CheckLogined();
				if (logined !=  ResultModel.Fail)
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
					if (string.IsNullOrEmpty(accountModel.Cookie))
					{
						var allCookies = accountModel.Driver.Manage().Cookies.AllCookies;
						var cookies = "";
						foreach (var cookie in allCookies)
						{
							string name = cookie.Name;
							string value = cookie.Value;
							cookies += name + "=" + value + ";";
						}
						accountModel.Cookie = cookies;
						_accountDataService.Update(accountModel.AccountIDKey, accountModel);
					}
					return ResultModel.Success;
				}
				if (i == 2)
				{
					accountModel.Status = "Chuyển về page profile thất bại!";
					_accountDataService.Update(accountModel.AccountIDKey, accountModel);
					return ResultModel.Fail;
				}
			}
			return ResultModel.Success;
		}
		private async Task<ResultModel> UpPost(string content, string filePaths)
		{
			try
			{
				accountModel.Driver.Url = "https://www.facebook.com/profile";
			}
			catch
			{
				return ResultModel.Fail;
			}
			for (int i = 0; i < 2; i++)
			{
				SeleniumHelper.Scroll(accountModel.Driver, 500);
			}
			Thread.Sleep(1000);
			try
			{
				var elementButton = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
				foreach (var ele in elementButton)
				{
					var text = ele.Text;
					if (text == "Ảnh/video")
					{
						ele.Click();
						break;
					}
				}
			}
			catch
			{
				return ResultModel.Fail;
			}
			Thread.Sleep(2000);
			#region nhập content 
			try
			{
				var elvsadf = accountModel.Driver.FindElement(By.CssSelector("div[aria-placeholder=\"Bạn đang nghĩ gì?\"]"));
				elvsadf.SendKeys(content);
			}
			catch
			{
				return ResultModel.Fail;
			}
			#endregion

			try
			{
				//var file = Path.GetFullPath(filePaths);
				//var filevideo = "C:\\Users\\ADMIN\\Videos\\Vũ trụ bao la 2\\Yummy77.mp4";

				//var filePathss = $"{file}\n{filevideo}";

				var element = accountModel.Driver.FindElement(By.CssSelector("input[accept=\"image/*,image/heif,image/heic,video/*,video/mp4,video/x-m4v,video/x-matroska,.mkv\"]"));
				element.SendKeys(filePaths);
			}
			catch
			{
				return ResultModel.Fail;
			}
			Thread.Sleep(3000);
			if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Đăng\"]")))
			{
				return ResultModel.Fail;
			}
			Thread.Sleep(5000);
			try
			{
				var eledang = accountModel.Driver.FindElement(By.CssSelector("div[aria-label=\"Đăng\"]"));
			}
			catch
			{

			}

			if (!SeleniumHelper.WaitElementHidden(accountModel.Driver, By.CssSelector("div[aria-label=\"Đăng\"]"), 60))
			{
				return ResultModel.Fail;
			}
			return ResultModel.Success;
		}
		private async Task<ResultModel> SurfNewFeed(int timeFrom, int timeTo)
		{
			accountModel.Status = "Lướt newfeed ...";
			var randomTime = random.Next(timeFrom, timeTo);
			var endTime = DateTime.Now.AddSeconds(randomTime);
			while (DateTime.Now < endTime)
			{
				_pauseEvent.Wait();
				_cancellationTokenSource.Token.ThrowIfCancellationRequested();
				var lengthSurf = random.Next(100, 1000);
				SeleniumHelper.Scroll(accountModel.Driver, lengthSurf);
				Thread.Sleep(random.Next(5000, 20000));

				url = accountModel.Driver.Url;
				if (url.Contains("956/"))
					return ResultModel.CheckPoint956;
				else if (url.Contains("282/"))
					return ResultModel.CheckPoint282;
				else if (url.Contains("checkpoint"))
					return ResultModel.CheckPoint;
			}

			return ResultModel.Success;
		}

		private async Task<ResultModel> ReadNotification()
		{
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[href=\"/notifications/\"]"));
			Thread.Sleep(5000);
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[href=\"/notifications/\"]"));
			return ResultModel.Success;
		}
		private async Task<ResultModel> MakeFriendsByKeyWord(string keyWord, int countFriends)
		{
			accountModel.Status = "Đang search ...";
			if (SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[type=\"search\"]"), keyWord))
			{
				Thread.Sleep(5000);
				SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[type=\"search\"]"), Keys.Enter);
			}
			Thread.Sleep(5000);

			accountModel.Status = "Đang đi kết bạn...";
			try
			{
				var elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"listitem\"]"));
				foreach (var element in elements)
				{
					var r = element.Text;
					if (element.Text == "Mọi người")
					{
						element.Click();
						break;
					}
				}
				Thread.Sleep(5000);
				elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
				int countMakeFriends = 0;
				if (elements.Count > 0)
				{
					for (int i = 0; i < elements.Count; i++)
					{
						if (elements[i].Text == "Thêm bạn bè")
						{
							countMakeFriends++;
							elements[i].Click();
							Thread.Sleep(random.Next(4000, 12000));
							if (countMakeFriends >= countFriends)
							{
								break;
							}
							SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
						}
					}
				}
			}
			catch
			{

			}
			for (int j = 0; j < random.Next(20, 40); j++)
			{
				SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
			}
			for (int j = 0; j < random.Next(10, 20); j++)
			{
				try
				{
					accountModel.Driver.FindElement(By.TagName("body")).SendKeys(Keys.ArrowUp);
				}
				catch
				{

				}
			}
			Thread.Sleep(3000);
			return ResultModel.Success;
		}

		private async Task<ResultModel> MakeFriendsBySuggestion(int countFriends)
		{
			accountModel.Status = "Đang search ...";

			Thread.Sleep(5000);

			accountModel.Status = "Đang đi kết bạn...";
			try
			{
				var elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"listitem\"]"));
				foreach (var element in elements)
				{
					var r = element.Text;
					if (element.Text == "Mọi người")
					{
						element.Click();
						break;
					}
				}
				Thread.Sleep(5000);
				elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
				int countMakeFriends = 0;
				if (elements.Count > 0)
				{
					for (int i = 0; i < elements.Count; i++)
					{
						if (elements[i].Text == "Thêm bạn bè")
						{
							countMakeFriends++;
							elements[i].Click();
							Thread.Sleep(random.Next(4000, 12000));
							if (countMakeFriends >= countFriends)
							{
								break;
							}
							SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
						}
					}
				}
			}
			catch
			{

			}
			for (int j = 0; j < random.Next(20, 40); j++)
			{
				SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
			}
			for (int j = 0; j < random.Next(10, 20); j++)
			{
				try
				{
					accountModel.Driver.FindElement(By.TagName("body")).SendKeys(Keys.ArrowUp);
				}
				catch
				{

				}
			}
			Thread.Sleep(3000);
			return ResultModel.Success;
		}
		private ResultModel JoinGroupByKeyWord(string keyWord, int countGroups)
		{
			accountModel.Status = $"Đang đi tham gia nhóm theo từ khóa {keyWord}...";
			try
			{
				accountModel.Driver.Url = "https://www.facebook.com/groups/feed/";
				Thread.Sleep(5000);
				if (SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[placeholder=\"Tìm kiếm nhóm\"]"), keyWord))
				{
					Thread.Sleep(5000);
					SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[placeholder=\"Tìm kiếm nhóm\"]"), Keys.Enter);
				}
				Thread.Sleep(3000);
			}
			catch
			{
				return ResultModel.Fail;
			}
			try
			{
				int countJoinGroup = 0;
			reJoinGroup:
				var Groups = accountModel.Driver.FindElements(By.CssSelector("div[role=\"article\"]"));
				var linkGroups = accountModel.Driver.FindElements(By.CssSelector("a[role=\"presentation\"]"));

				for (int i = 0; i < Groups.Count; i++)
				{
					var sgg = Groups[i].Text;
					if (Groups[i].Text.Contains("Truy cập"))
					{
						SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
						continue;
					}
					countJoinGroup++;
					if (countJoinGroup <= countGroups)
					{
						break;
					}
					linkGroups[i].Click();
					Thread.Sleep(3000);
					for (int j = 0; j < 5; j++)
					{
						SeleniumHelper.Scroll(accountModel.Driver, random.Next(500, 1000));
						Thread.Sleep(random.Next(2000, 6000));
					}
					SeleniumHelper.Scroll(accountModel.Driver, -10000);
					var joinButtons = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
					foreach (var button in joinButtons)
					{
						var r = button.Text;
						if (button.Text == "Tham gia nhóm")
						{
							try
							{
								Actions actions = new Actions(accountModel.Driver);
								actions.MoveToElement(button).Click().Perform();
								Thread.Sleep(3000);
								break;
							}
							catch
							{

							}


						}
					}
					Thread.Sleep(random.Next(4000, 10000));
					accountModel.Driver.Navigate().Back();
					// Chờ cho đến khi trang tải xong
					WebDriverWait wait = new WebDriverWait(accountModel.Driver, TimeSpan.FromSeconds(10));
					wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
					goto reJoinGroup;
				}

			}
			catch
			{
				return ResultModel.Fail;
			}
			return ResultModel.Success;
		}
		private ResultModel ReadMessenger()
		{
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Messenger\"]"));
			Thread.Sleep(5000);
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Messenger\"]"));
			Thread.Sleep(random.Next(10000, 20000));
			return ResultModel.Success;
		}
		private ResultModel LikePost(int times)
		{
			// Logic like post
			return ResultModel.Success;
		}

		private async Task<ResultModel> CommentPost(int times, string comment)
		{
			// Logic comment post
			return ResultModel.Success;
		}
		private async Task<ResultModel> LoginUIDPass()
		{
			//FunctionHelper.EditValueColumn(accountModel, "C_Status", "Đang login!", true);
			accountModel.Status = "Đang login ...";
			var t = accountModel.Driver.FindElement(By.Id("email")).Text;
			if (accountModel.Driver.FindElement(By.Id("email")).Text == "")
			{
				if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("email"), accountModel.UID))
					return ResultModel.Fail;
			}

			Thread.Sleep(1000);
			if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("pass"), accountModel.Password))
				return ResultModel.Fail;
			Thread.Sleep(2000);
			if (!SeleniumHelper.Click(accountModel.Driver, By.Name("login")))
				return ResultModel.Fail;
			var url = accountModel.Driver.Url;
			if (SeleniumHelper.UrlChange(accountModel.Driver, url))
			{
				if (accountModel.Driver.Url == "https://www.facebook.com/checkpoint/?next")
				{
					if (accountModel.C_2FA != "")
					{
						var code2FA = FunctionHelper.ConvertTwoFA(accountModel.C_2FA);
						if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("approvals_code"), code2FA))
						{
							return ResultModel.Fail;
						}
						Thread.Sleep(1000);
						if (!SeleniumHelper.Click(accountModel.Driver, By.Id("checkpointSubmitButton")))
							return ResultModel.Fail;
					}
				}
			}
			//Lưu trình duyệt
			if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("button[name=\"submit[Continue]\"]")))
			{
				SeleniumHelper.Click(accountModel.Driver, By.CssSelector("button[name=\"submit[Continue]\"]"));
			}

			if (SeleniumHelper.WaitElement(accountModel.Driver, By.Id("approvals_code")))
			{
				return ResultModel.Fail;
			}
			if (string.IsNullOrEmpty(accountModel.Cookie))
			{
				var allCookies = accountModel.Driver.Manage().Cookies.AllCookies;
				var cookies = "";
				foreach (var cookie in allCookies)
				{
					string name = cookie.Name;
					string value = cookie.Value;
					cookies += name + "=" + value + ";";
				}
				accountModel.Cookie = cookies;

			}
			return ResultModel.Fail;
		}
		private bool LoginByCookie()
		{
			//FunctionHelper.EditValueColumn(accountModel, "C_Status", "Đến trang login ...");

			var cookies = accountModel.Cookie.Split(';');
			foreach (var cookie in cookies)
			{
				try
				{
					var arr = cookie.Split("=".ToCharArray(), 2);
					var ck = new Cookie(arr[0].Trim(), arr[1].Trim());
					accountModel.Driver.Manage().Cookies.AddCookie(ck);
				}
				catch
				{
				}
			}
			try
			{
				accountModel.Driver.Url = "https://www.facebook.com/";
			}
			catch
			{
				return false;
			}
			if (SeleniumHelper.WaitElement(accountModel.Driver, By.Id("m_login_email")))
			{
				return false;
			}
			return true;
		}

		protected async override Task<ResultModel> SwitchToProfile()
		{
			var pageSource = accountModel.Driver.PageSource;
			var _user = RegexHelper.GetValueFromGroup("__user=(.*?)&", pageSource);
			if (_user != accountModel.UID)
			{
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[role=\"listitem\"]")))
				{
					SeleniumHelper.Click(accountModel.Driver, By.CssSelector("svg[role=\"img\"]"));

					Thread.Sleep(2000);
					if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[role=\"listitem\"]")))
						return ResultModel.Fail;
				}
			}
			return ResultModel.Success;
		}
		protected async override Task<ResultModel> CheckLogined()
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

				Thread.Sleep(1000);
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
				Thread.Sleep(2000);
				if (!SeleniumHelper.Click(accountModel.Driver, By.Name("login")))
					return ResultModel.Fail;
				if (SeleniumHelper.UrlChange(accountModel.Driver, url))
				{
					if (accountModel.Driver.Url.Contains("https://www.facebook.com/checkpoint/?next"))
					{
						if (accountModel.C_2FA != "")
						{
							var code2FA = FunctionHelper.ConvertTwoFA(accountModel.C_2FA);
							if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("approvals_code"), code2FA))
							{
								return ResultModel.Fail;
							}
							Thread.Sleep(1000);
							if (!SeleniumHelper.Click(accountModel.Driver, By.Id("checkpointSubmitButton")))
								return ResultModel.Fail;
						}
					}
				}
				//Lưu trình duyệt
				accountModel.Status = "Đang đợi nút lưu trình duyệt ...";
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("button[name=\"submit[Continue]\"]")))
				{
					SeleniumHelper.Click(accountModel.Driver, By.CssSelector("button[name=\"submit[Continue]\"]"));
				}

				if (SeleniumHelper.WaitElement(accountModel.Driver, By.Id("approvals_code")))
				{
					return ResultModel.Fail;
				}
			}
			return ResultModel.Success;
		}
	}
}
