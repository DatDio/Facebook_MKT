using AutoMapper;
using Facebook_MKT.Data.Entities;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebok_MKT.Service.DataService.Pages;
using Facebok_MKT.Service.DataService.Accounts;

namespace Facebok_MKT.Service.Controller.BrowserController
{
	public class FacebookBrowserPageController : BaseFacebookBrowserController
	{
		private PageModel pageModel;
		private readonly IPageDataService _pageDataService;
		private string url;
		public FacebookBrowserPageController(AccountModel accountModel,
			IAccountDataService accountDataService,
			IPageDataService pageDataService,
			ManualResetEventSlim pauseEvent,
			CancellationTokenSource cancellationTokenSource,
			PageModel pageModel) : base(accountModel, accountDataService,
				pauseEvent, cancellationTokenSource)
		{
			_pageDataService = pageDataService;
			this.pageModel = pageModel;
		}
		public override async Task<ResultModel> ExecuteTask(TaskModel task)
		{
			switch (task.TaskName)
			{
				case "Nghỉ":
					return await SleepThread(Convert.ToInt32(task.Fields[0].Value),
										Convert.ToInt32(task.Fields[1].Value));
				case "Lướt New Feed":
					return await SurfNewFeed(Convert.ToInt32(task.Fields[0].Value),
						Convert.ToInt32(task.Fields[1].Value));
				case "Like":
					return await LikePost(Convert.ToInt32(task.Fields[0].Value));
				case "Comment":
					return await CommentPost(Convert.ToInt32(task.Fields[0].Value), task.Fields[2].Value.ToString());


				case "Đọc tin nhắn":
					return await ReadMessenger();
				case "Đọc thông báo":
					return await ReadNotification();
				case "Đăng bài":
					var f = task.Fields[1].Label.ToString();
					var c = task.Fields[2].Value.ToString();
					return await UpPost(task.Fields[1].Label.ToString(), task.Fields[2].Value.ToString());
				case "Tham gia nhóm theo từ khóa:":
					return await JoinGroupByKeyWord(task.Fields[0].Value.ToString(), Convert.ToInt32(task.Fields[1].Value));
				default:
					return ResultModel.Fail;
			}
		}
		public override async Task<ResultModel> Initialization()
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
					accountModel.Status = "Chuyển về page profile thất bại!";
					_accountDataService.Update(accountModel.AccountIDKey, accountModel);
					return ResultModel.Fail;
				}
			}
			return ResultModel.Success;
		}
		private async Task<ResultModel> SleepThread(int timeSleepFrom, int timeSleepTo)
		{
			var timeSleep = random.Next(timeSleepFrom, timeSleepTo);
			Thread.Sleep(timeSleep);
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
			url = accountModel.Driver.Url;
			if(url != "https://www.facebook.com/")
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
			var randomTime = random.Next(timeFrom, timeTo);
			for (int i = 0; i < randomTime; i++)
			{
				var lengthSurf = random.Next(100, 400);
				SeleniumHelper.Scroll(accountModel.Driver, lengthSurf);
				Thread.Sleep(1000);
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
		private async Task<ResultModel> JoinGroupByKeyWord(string keyWord, int countGroups)
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
		private async Task<ResultModel> ReadMessenger()
		{
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Messenger\"]"));
			Thread.Sleep(5000);
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Messenger\"]"));
			Thread.Sleep(random.Next(10000, 20000));
			return ResultModel.Success;
		}
		private async Task<ResultModel> LikePost(int times)
		{
			// Logic like post
			return ResultModel.Success;
		}
		private async Task<ResultModel> CommentPost(int times, string comment)
		{
			// Logic comment post
			return ResultModel.Success;
		}
		private ResultModel LoginUIDPass()
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
			if (_user != pageModel.PageID)
			{
				try
				{
					accountModel.Driver.Url = $"https://www.facebook.com/profile.php?id={pageModel.PageID}";
				}
				catch
				{
					return ResultModel.Fail;

				}
				//Bấm chuyển trang
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[aria-label=\"Chuyển\"]")))
				{
					if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Chuyển\"]")))
					{
						return ResultModel.Fail;
					}
				}
				else
				{
					return ResultModel.Fail;
				}
				//
				Thread.Sleep(2000);
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[aria-label=\"Chuyển\"]")))
				{
					if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Chuyển\"]"), count: 1))
					{
						return ResultModel.Fail;
					}
				}
				else
				{
					return ResultModel.Fail;
				}
				Thread.Sleep(8000);
				pageSource = accountModel.Driver.PageSource;
				_user = RegexHelper.GetValueFromGroup("__user=(.*?)&", pageSource);
				if (_user != pageModel.PageID)
				{
					return ResultModel.Fail;
				}

				return ResultModel.Success;
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
