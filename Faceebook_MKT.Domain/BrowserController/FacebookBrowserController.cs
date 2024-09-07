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
using Facebook_MKT.Data.Services;
using Facebook_MKT.Data.Entities;

namespace Faceebook_MKT.Domain.BrowserController
{
	public class FacebookBrowserController
	{
		Random random;
		AccountModel accountModel;
		private readonly IMapper _mapper;
		private readonly IDataService<Account> _dataService;
		private string url;
		//FacebookAPIController apiFB;
		public FacebookBrowserController(AccountModel accountModel,
			IDataService<Account> dataService,
			IMapper mapper)
		{
			this.accountModel = accountModel;
			_dataService = dataService;
			_mapper = mapper;
			random = new Random();
		}

		public ResultModel ExecuteTask(TaskModel task, AccountModel account)
		{
			switch (task.TaskName)
			{
				case "Đăng nhập":
					return LoginUIDPass();
				case "Nghỉ":
					return SleepThread(Convert.ToInt32(task.Fields[0].Value),
										Convert.ToInt32(task.Fields[1].Value));
				case "Lướt New Feed":
					return SurfNewFeed(Convert.ToInt32(task.Fields[0].Value),
						Convert.ToInt32(task.Fields[1].Value));
				case "Like":
					return LikePost(Convert.ToInt32(task.Fields[0].Value));
				case "Comment":
					return CommentPost(Convert.ToInt32(task.Fields[0].Value), task.Fields[2].Value.ToString());
				// Thêm các case khác cho từng task
				default:
					return ResultModel.Fail;
			}
		}
		public ResultModel InteractFacebookWWW(string keyWordSearch)
		{
			string url = "", pageSource = "", _user = "";
			//var body = null;
			DateTime currentTime = DateTime.Now;
			DateTime targetTime = currentTime.AddMinutes(15);
			//int randomLike = 0;
			//apiFB = new FacebookAPIController();
			try
			{
				accountModel.Driver.Url = "https://www.facebook.com/";
			}
			catch
			{
				return ResultModel.Fail;
			}
			url = accountModel.Driver.Url;
			//pageSource = accountModel.Driver.PageSource;
			//_user = RegexHelper.GetValueFromGroup("__user=(.*?)&", pageSource);

			accountModel.Status = "Đang check xem login chưa ...";
			if (SeleniumHelper.WaitElement(accountModel.Driver, By.Id("email")))
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
					return ResultModel.LoginFail;
				}

				if (accountModel.Driver.FindElement(By.Id("email")).Text == "")
				{
					if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("email"), accountModel.UID))
						return ResultModel.LoginFail;
				}

				Thread.Sleep(1000);
				try
				{
					var passElement = accountModel.Driver.FindElement(By.Id("pass"));
					passElement.Click();
					passElement.Clear();
				}
				catch
				{
					return ResultModel.LoginFail;
				}

				if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("pass"), accountModel.Password))
					return ResultModel.LoginFail;
				Thread.Sleep(2000);
				if (!SeleniumHelper.Click(accountModel.Driver, By.Name("login")))
					return ResultModel.LoginFail;
				if (SeleniumHelper.UrlChange(accountModel.Driver, url))
				{
					if (accountModel.Driver.Url.Contains("https://www.facebook.com/checkpoint/?next"))
					{
						if (accountModel.C_2FA != "")
						{
							var code2FA = FunctionHelper.ConvertTwoFA(accountModel.C_2FA);
							if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("approvals_code"), code2FA))
							{
								return ResultModel.LoginFail;
							}
							Thread.Sleep(1000);
							if (!SeleniumHelper.Click(accountModel.Driver, By.Id("checkpointSubmitButton")))
								return ResultModel.LoginFail;
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
					return ResultModel.LoginFail;
				}
			}

			if (url.Contains("956/"))
				return ResultModel.CheckPoint956;
			else if (url.Contains("282/"))
				return ResultModel.CheckPoint282;
			else if (url.Contains("checkpoint"))
				return ResultModel.CheckPoint;
			url = accountModel.Driver.Url;
			if (url != "https://www.facebook.com/")
			{
				return ResultModel.LoginFail;
			}

			//Chuyển về profile để tương tác
			pageSource = accountModel.Driver.PageSource;
			_user = RegexHelper.GetValueFromGroup("__user=(.*?)&", pageSource);
			if (_user != accountModel.UID)
			{
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[role=\"listitem\"]")))
				{
					SeleniumHelper.Click(accountModel.Driver, By.CssSelector("svg[role=\"img\"]"));

					Thread.Sleep(2000);
					SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[role=\"listitem\"]"));

				}
			}
			//Lấy Cookie
			Thread.Sleep(5000);
			if (String.IsNullOrEmpty(accountModel.Cookie))
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
				accountModel.Status = "Đang check xem login chưa ...";
			}


			while (DateTime.Now < targetTime)
			{
				accountModel.Status = "Đang tương tác luớt newfeed...";
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[aria-label=\"OK\"]")))
				{
					SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"OK\"]"));
				}
				Thread.Sleep(3000);
				//Đọc thông báo
				SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[href=\"/notifications/\"]"));
				Thread.Sleep(5000);
				SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[href=\"/notifications/\"]"));
				//Đọc tin nhắn
				SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Messenger\"]"));
				Thread.Sleep(5000);
				SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Messenger\"]"));
				Thread.Sleep(random.Next(10000, 20000));
				//Lướt newfeed
				//SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Thích\"]"), count: 0);
				for (int i = 0; i < 20; i++)
				{
					//SeleniumHelper.Scroll(accountModel.Driver, random.Next(400, 1000));
					Thread.Sleep(random.Next(10000, 20000));

					for (int j = 0; j < random.Next(20, 60); j++)
					{
						SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
					}

					if (i == 15)
					{
						accountModel.Driver.Navigate().Refresh();
					}

				}
				//Đi kết bạn và tham gia nhóm

				if (SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[type=\"search\"]"), keyWordSearch))
				{
					Thread.Sleep(5000);
					SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[type=\"search\"]"), OpenQA.Selenium.Keys.Enter);
				}
				Thread.Sleep(5000);
				//if (ManagePageForm._makeFriends)
				//{
				//	accountModel.Status = "Đang đi kết bạn...";
				//	try
				//	{
				//		var elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"listitem\"]"));
				//		foreach (var element in elements)
				//		{
				//			var r = element.Text;
				//			if (element.Text == "Mọi người")
				//			{
				//				element.Click();
				//				break;
				//			}
				//		}
				//		Thread.Sleep(5000);
				//		elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
				//		int countMakeFriends = 0;
				//		if (elements.Count > 0)
				//		{
				//			for (int i = 0; i < elements.Count; i++)
				//			{
				//				if (elements[i].Text == "Thêm bạn bè")
				//				{
				//					countMakeFriends++;
				//					elements[i].Click();
				//					Thread.Sleep(random.Next(4000, 12000));
				//					//if (countMakeFriends >= ManagePageForm._countMakeFriends)
				//					//{
				//					//	break;
				//					//}
				//					SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
				//				}
				//			}
				//		}
				//	}
				//	catch
				//	{

				//	}
				//	for (int j = 0; j < random.Next(20, 40); j++)
				//	{
				//		SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
				//	}
				//	for (int j = 0; j < random.Next(10, 20); j++)
				//	{
				//		try
				//		{
				//			accountModel.Driver.FindElement(By.TagName("body")).SendKeys(OpenQA.Selenium.Keys.ArrowUp);
				//		}
				//		catch
				//		{

				//		}
				//	}
				//}

				Thread.Sleep(3000);
				//Tham gia nhóm
				//if (ManagePageForm._joinGroup)
				//{
				//	accountModel.Status = "Đang đi tham gia nhóm...";

				//	try
				//	{
				//		var elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"listitem\"]"));
				//		foreach (var element in elements)
				//		{
				//			var r = element.Text;
				//			if (element.Text == "Nhóm")
				//			{
				//				element.Click();
				//				break;
				//			}
				//		}
				//		Thread.Sleep(5000);
				//		elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
				//		int countJoinGroup = 0;
				//		foreach (var element in elements)
				//		{
				//			var r = element.Text;
				//			if (element.Text == "Tham gia")
				//			{
				//				countJoinGroup++;
				//				if (countJoinGroup >= ManagePageForm._countJoinGroup)
				//				{
				//					break;
				//				}
				//				element.Click();
				//				Thread.Sleep(random.Next(4000, 12000));
				//				for (int j = 0; j < 10; j++)
				//				{
				//					SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
				//				}
				//				break;
				//			}
				//		}
				//		for (int j = 0; j < random.Next(20, 40); j++)
				//		{
				//			SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
				//		}

				//	}
				//	catch
				//	{

				//	}
				//}

				//like trang

				//Lướt watch
				accountModel.Status = "Đang lướt watch...";
				try
				{
					accountModel.Driver.Url = "https://www.facebook.com/watch";
				}
				catch
				{
					return ResultModel.Fail;
				}
				for (int i = 0; i < 10; i++)
				{
					for (int j = 0; j < random.Next(20, 50); j++)
					{
						SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
					}
					Thread.Sleep(random.Next(40000, 120000));
				}
				Thread.Sleep(random.Next(40000, 120000));
			}
			return ResultModel.Success;
		}

		private ResultModel SleepThread(int timeSleepFrom,int timeSleepTo)
		{
			Thread.Sleep(random.Next(timeSleepFrom,timeSleepTo));
			return ResultModel.Success;
		}
		public ResultModel SurfNewFeed(int timeFrom, int timeTo)
		{

			accountModel.Status = "Lướt newfeed ...";
			for (int i = 0; i < random.Next(timeFrom, timeTo); i++)
			{
				var lengthSurf = random.Next(10, 30);
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
		public ResultModel ReadNotification()
		{
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[href=\"/notifications/\"]"));
			Thread.Sleep(5000);
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[href=\"/notifications/\"]"));
			return ResultModel.Success;
		}
		public ResultModel MakeFriendsByKeyWord(string keyWord, int countFriends)
		{
			accountModel.Status = "Đang search ...";
			if (SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[type=\"search\"]"), keyWord))
			{
				Thread.Sleep(5000);
				SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[type=\"search\"]"), OpenQA.Selenium.Keys.Enter);
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
					accountModel.Driver.FindElement(By.TagName("body")).SendKeys(OpenQA.Selenium.Keys.ArrowUp);
				}
				catch
				{

				}
			}
			Thread.Sleep(3000);
			return ResultModel.Success;
		}
		public ResultModel JoinGroupByKeyWord(string keyWord, int countGroups)
		{
			accountModel.Status = $"Đang đi tham gia nhóm theo từ khóa {keyWord}...";
			try
			{
				var elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"listitem\"]"));
				foreach (var element in elements)
				{
					var r = element.Text;
					if (element.Text == "Nhóm")
					{
						element.Click();
						break;
					}
				}
				Thread.Sleep(5000);
				elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
				int countJoinGroup = 0;
				foreach (var element in elements)
				{
					var r = element.Text;
					if (element.Text == "Tham gia")
					{
						countJoinGroup++;
						if (countJoinGroup >= countGroups)
						{
							break;
						}
						element.Click();
						Thread.Sleep(random.Next(4000, 12000));
						for (int j = 0; j < 10; j++)
						{
							SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
						}
						break;
					}
				}
				for (int j = 0; j < random.Next(20, 40); j++)
				{
					SeleniumHelper.ScrollsArrowDown(accountModel.Driver);
				}
			}
			catch
			{

			}
			return ResultModel.Success;
		}
		public ResultModel ReadMessenger()
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

		private ResultModel CommentPost(int times, string comment)
		{
			// Logic comment post
			return ResultModel.Success;
		}
		public ResultModel LoginUIDPass()
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
			if (String.IsNullOrEmpty(accountModel.Cookie))
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
		public bool LoginByCookie()
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

		public ResultModel SwitchToProfile()
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
		public ResultModel CheckLogined()
		{
			accountModel.Status = "Đang check xem login chưa ...";
			if (SeleniumHelper.WaitElement(accountModel.Driver, By.Id("email")))
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
					return ResultModel.LoginFail;
				}

				if (accountModel.Driver.FindElement(By.Id("email")).Text == "")
				{
					if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("email"), accountModel.UID))
						return ResultModel.LoginFail;
				}

				Thread.Sleep(1000);
				try
				{
					var passElement = accountModel.Driver.FindElement(By.Id("pass"));
					passElement.Click();
					passElement.Clear();
				}
				catch
				{
					return ResultModel.LoginFail;
				}

				if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("pass"), accountModel.Password))
					return ResultModel.LoginFail;
				Thread.Sleep(2000);
				if (!SeleniumHelper.Click(accountModel.Driver, By.Name("login")))
					return ResultModel.LoginFail;
				if (SeleniumHelper.UrlChange(accountModel.Driver, url))
				{
					if (accountModel.Driver.Url.Contains("https://www.facebook.com/checkpoint/?next"))
					{
						if (accountModel.C_2FA != "")
						{
							var code2FA = FunctionHelper.ConvertTwoFA(accountModel.C_2FA);
							if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("approvals_code"), code2FA))
							{
								return ResultModel.LoginFail;
							}
							Thread.Sleep(1000);
							if (!SeleniumHelper.Click(accountModel.Driver, By.Id("checkpointSubmitButton")))
								return ResultModel.LoginFail;
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
					return ResultModel.LoginFail;
				}
			}
			return ResultModel.Success;
		}
	}
}
