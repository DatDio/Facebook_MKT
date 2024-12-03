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
				case "Like":
					return await LikePost(Convert.ToInt32(task.Fields[0].Value));
				case "Comment":
					return await CommentPost(Convert.ToInt32(task.Fields[0].Value), task.Fields[2].Value.ToString());
				case "Kết bạn theo từ khóa":
					return await MakeFriendsByKeyWord((task.Fields[0].Value).ToString(), Convert.ToInt32(task.Fields[1].Value));
				case "Kết bạn theo gợi ý":
					return await MakeFriendsBySuggestion(Convert.ToInt32(task.Fields[0].Value));
				case "Đọc tin nhắn":
					return await ReadMessenger();
				case "Đọc thông báo":
					return await ReadNotification();
				case "Đăng bài":
					var f = task.Fields[1].Label.ToString();
					var c = task.Fields[2].Value.ToString();
					return await UpPost(task.Fields[1].Label.ToString(), task.Fields[2].Value.ToString());
				case "Tham gia nhóm theo từ khóa":
					return await JoinGroupByKeyWord((task.Fields[0].Value).ToString(), Convert.ToInt32(task.Fields[1].Value));
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
					accountModel.Status = "Login fail!";
					return ResultModel.Fail;
				}
			}
			for (int i = 0; i <= 2; i++)
			{
				var switchProfile = await SwitchToProfile();
				if (switchProfile != ResultModel.Fail)
				{
					//if (string.IsNullOrEmpty(accountModel.Cookie))
					//{
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
					//}
					return ResultModel.Success;
				}
				if (i == 2)
				{
					accountModel.Status = "Chuyển về page profile thất bại!";
					await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
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
			url = accountModel.Driver.Url;
			if (url != "https://www.facebook.com/")
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
			var endTime = DateTime.Now.AddSeconds(randomTime);
			while (DateTime.Now < endTime)
			{
				_pauseEvent.Wait();
				_cancellationTokenSource.Token.ThrowIfCancellationRequested();
				var lengthSurf = random.Next(100, 1000);
				SeleniumHelper.Scroll(accountModel.Driver, lengthSurf);
				await Task.Delay(random.Next(5000, 20000));

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
			}

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
			accountModel.Status = "Đang đến trang kết bạn  ...";
			try
			{
				accountModel.Driver.Url = "https://www.facebook.com/friends/suggestions";
			}
			catch
			{
				return ResultModel.Fail;
			}
			Thread.Sleep(5000);
			try
			{
				int countMakeFriends = 0;
				while (countMakeFriends < countFriends)
				{
					// Tìm lại các phần tử mỗi lần lặp
					var elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"none\"]"));

					if (elements.Count == 0) break;

					for (int i = 0; i < elements.Count; i++)
					{
						try
						{
							// Kiểm tra xem phần tử có còn hợp lệ không
							if (elements[i].Text == "Thêm bạn bè")
							{
								countMakeFriends++;
								elements[i].Click();
								Thread.Sleep(random.Next(4000, 6000));

								if (countMakeFriends >= countFriends)
								{
									break;
								}

								SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
								break;
							}
						}
						catch (StaleElementReferenceException)
						{

							break;
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

		private async Task<ResultModel> JoinGroupByKeyWord(string keyWord, int countGroups)
		{
			accountModel.Status = $"Đang đi tham gia nhóm theo từ khóa {keyWord}...";
			try
			{
				accountModel.Driver.Url = "https://www.facebook.com/groups/feed/";
				await Task.Delay(random.Next(3000, 5000));
				if (SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[placeholder=\"Tìm kiếm nhóm\"]"), keyWord))
				{
					await Task.Delay(random.Next(3000, 5000));
					SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[placeholder=\"Tìm kiếm nhóm\"]"), Keys.Enter);
				}
				await Task.Delay(random.Next(3000, 5000));
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
					if (countJoinGroup >= countGroups)
					{
						break;
					}
					Actions actions = new Actions(accountModel.Driver);
					actions.MoveToElement(linkGroups[i]).Click().Perform();
					await Task.Delay(random.Next(3000, 5000));
					for (int j = 0; j < 5; j++)
					{
						SeleniumHelper.Scroll(accountModel.Driver, random.Next(500, 1000));
						await Task.Delay(random.Next(3000, 6000));
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
								Actions actionClick = new Actions(accountModel.Driver);
								actions.MoveToElement(button).Click().Perform();
								await Task.Delay(random.Next(3000, 5000));
								break;
							}
							catch
							{

							}
						}
					}
					await Task.Delay(random.Next(3000, 10000));
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

	}
}
