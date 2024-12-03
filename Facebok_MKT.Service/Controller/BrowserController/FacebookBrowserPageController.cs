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
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Windows;
using Faceebook_MKT.Domain.Systems;
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
				case "Đăng reel":
					var content = task.Fields[0].Value.ToString();
					var comment = task.Fields[1].Value.ToString();
					return await UpReel(task.Fields[0].Value.ToString(),
						task.Fields[1].Value.ToString());
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
				else if (logined != ResultModel.CheckPoint)
				{
					return ResultModel.CheckPoint;
				}
				if (i == 2)
				{
					return ResultModel.Fail;
				}
			}
			for (int i = 0; i <= 2; i++)
			{
				ResultModel switchProfile;
				// Chọn ngẫu nhiên giữa SwitchToProfileByClickAvatar và SwitchToProfile
				if (random.Next(0, 2) == 0)
				{
					switchProfile = await SwitchToProfileByClickAvatar();
				}
				else
				{
					switchProfile = await SwitchToProfile();
				}

				// Kiểm tra kết quả của phương thức đã chọn
				if (switchProfile != ResultModel.Fail)
				{
					return ResultModel.Success;
				}
				else
				{
					try
					{
						accountModel.Driver.Navigate().Refresh();
					}
					catch
					{
						return ResultModel.Fail;
					}

				}

				if (i >= 2)
				{
					accountModel.Status = "Chuyển về page profile thất bại!";
					await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
					return ResultModel.Fail;
				}
			}
			return ResultModel.Success;
		}
		private async Task<ResultModel> SleepThread(int timeSleepFrom, int timeSleepTo)
		{
			var timeSleep = random.Next(timeSleepFrom, timeSleepTo);
			for (int i = 0; i < timeSleep; i++)
			{
				_pauseEvent.Wait();
				_cancellationTokenSource.Token.ThrowIfCancellationRequested();

				accountModel.Status = $"Nghỉ {timeSleep - i}s ...";
				await Task.Delay(1000);
			}

			return ResultModel.Success;
		}

		private async Task<ResultModel> UpPost(string content, string filePaths)
		{
			url = accountModel.Driver.Url;
			if (!url.Contains("https://www.facebook.com/profile")
				|| !url.Contains(pageModel.PageID))
			{
				pageModel.PageStatus = "Đến trang page profile";
				try
				{
					accountModel.Driver.Url = "https://www.facebook.com/profile";
				}
				catch
				{
					return ResultModel.Fail;
				}
			}

			pageModel.PageStatus = "scroll ...";
			for (int i = 0; i < 2; i++)
			{
				SeleniumHelper.Scroll(accountModel.Driver, 200);
			}
			pageModel.PageStatus = "Bấm ảnh/video ...";
			await Task.Delay(random.Next(3000, 5000));
			try
			{
				var elementButton = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
				foreach (var ele in elementButton)
				{
					var text = ele.Text;
					if (text == "Ảnh/video")
					{
						// Sử dụng Actions để di chuyển đến phần tử trước khi click
						Actions actions = new Actions(accountModel.Driver);
						actions.MoveToElement(ele).Click().Perform();
						break;
					}
				}
			}
			catch
			{
				return ResultModel.Fail;
			}

			pageModel.PageStatus = "Nhập content ...";
			await Task.Delay(random.Next(3000, 5000));
			#region nhập content 
			if (!SeleniumHelper.SendKeysWithEmoji(accountModel.Driver,
				By.CssSelector("div[aria-placeholder=\"Bạn đang nghĩ gì?\"]"), content))
			{
				return ResultModel.Fail;
			}

			#endregion
			await Task.Delay(random.Next(3000, 5000));
			try
			{
				var element = accountModel.Driver.FindElement(By.CssSelector("input[accept=\"image/*,image/heif,image/heic,video/*,video/mp4,video/x-m4v,video/x-matroska,.mkv\"]"));
				element.SendKeys(filePaths);
			}
			catch
			{
				return ResultModel.Fail;
			}
			pageModel.PageStatus = "Bấm đăng ...";
			await Task.Delay(random.Next(3000, 5000));
			if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Đăng\"]")))
			{
				return ResultModel.Fail;
			}
			pageModel.PageStatus = "Check xem có nút thêm nút không ...";
			await Task.Delay(random.Next(3000, 5000));
			if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[aria-label=\"Lúc khác\"]")))
			{
				if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Lúc khác\"]")))
				{
					return ResultModel.Fail;
				}
			}
			pageModel.PageStatus = "Đang đăng ...";
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
			pageModel.PageStatus = "Đăng bài thành công!";
			await FolderHelper.DeleteFileVideo(filePaths);
			return ResultModel.Success;
		}

		private async Task<ResultModel> UpReel(string content, string commentReel)
		{
			pageModel.PageStatus = "Đến trang up reel...";
			string pageSource = "", video_id = "", filePaths = "", folderPath = "";
			folderPath = await FolderHelper.GetFolderVideoPathUpReel(pageModel.PageFolderVideo);
			filePaths = await FolderHelper.GetVideoPathUpReel(folderPath);
			if (String.IsNullOrEmpty(folderPath))
			{
				pageModel.PageStatus = "Hết video!";
				await _pageDataService.Update(pageModel.PageID, pageModel);
				return ResultModel.Fail;
			}
			if (String.IsNullOrEmpty(content))
			{
				content = FolderHelper.GetTittleFileTxTUpReel(folderPath, SystemContants.FileTittleTxt);
			}
			var duration = FolderHelper.GetDuration(filePaths);
			if (duration == 0)
			{
				pageModel.PageStatus = "Video bị lỗi!";
				return ResultModel.Fail;
			}

			if (duration >= 90)
			{
				return await UpPost(content, filePaths);
			}
			try
			{
				accountModel.Driver.Url = "https://www.facebook.com/reels/create/?surface=ADDL_PROFILE_PLUS";
			}
			catch
			{
				return ResultModel.Fail;
			}
			try
			{
				IWebElement button = accountModel.Driver.FindElement(By.CssSelector("div[aria-label=\"Đóng\"]"));
				var t = button.Text;
				Actions actions = new Actions(accountModel.Driver);
				actions.MoveToElement(button).Click().Perform();
			}
			catch
			{

			}

			//SeleniumHelper.Click(page.driver, By.CssSelector("input[type=\"file\"]"));
			pageModel.PageStatus = "Up video reel...";
			await Task.Delay(random.Next(3000, 5000));
			if (!SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("input[type=\"file\"]"), filePaths))
			{
				return ResultModel.Fail;
			}

			//Bấm tiếp
			pageModel.PageStatus = "Bấm tiếp ...";
			await Task.Delay(random.Next(3000, 5000));
			if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Tiếp\"]"), 10))
			{
				return ResultModel.Fail;
			}
			//Bấm tiếp lần 2
			pageModel.PageStatus = "Bấm tiếp ...";
			await Task.Delay(random.Next(3000, 5000));
			for (int i = 0; i < 20; i++)
			{
				try
				{
					var elements = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
					foreach (var element in elements)
					{
						var r = element.Text;
						if (element.Text == "Tiếp")
						{
							element.Click();
							goto DienTittle;
						}
					}
				}
				catch
				{
					await Task.Delay(1000);
				}
			}
			return ResultModel.Fail;
		DienTittle:
			//Điền titile
			pageModel.PageStatus = "Điền titile ...";
			await Task.Delay(random.Next(3000, 5000));
			if (!SeleniumHelper.SendKeysWithEmoji(accountModel.Driver,
				By.CssSelector("div[aria-label=\"Mô tả thước phim của bạn...\"]"), content))
			{
				return ResultModel.Fail;
			}
			pageModel.PageStatus = "Check xem bị lỗi không ...";
			//if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[aria-disabled=\"false\"]")))
			//{
			//	if(SeleniumHelper.GetTextElement(accountModel.Driver, By.CssSelector("div[aria-disabled=\"false\"]")).Contains("Không thể tải file của bạn lên:"))
			//	{
			//		try
			//		{
			//			// Thực hiện thao tác khiến trang hiển thị popup
			//			accountModel.Driver.Navigate().Refresh();
			//			var alert = accountModel.Driver.SwitchTo().Alert();
			//			Console.WriteLine($"Popup xuất hiện: {alert.Text}");
			//			alert.Accept();
			//		}
			//		catch (UnhandledAlertException)
			//		{
			//			//// Nếu popup xuất hiện, xử lý alert
			//			//var alert = accountModel.Driver.SwitchTo().Alert();
			//			//Console.WriteLine($"Popup xuất hiện: {alert.Text}");
			//			//alert.Accept(); 
			//		}
			//		return ResultModel.Fail;
			//	}
			//}
			//aria-disabled="false"
			//Bấm đăng
			pageModel.PageStatus = "Bấm đăng ...";
			await Task.Delay(random.Next(3000, 5000));
			for (int i = 0; i < 20; i++)
			{
				try
				{
					//IWebElement button = accountModel.Driver.
					//					FindElements(By.CssSelector("div[aria-label=\"Đăng\"]"))[1];
					//var t = button.Text;
					//Actions actions = new Actions(accountModel.Driver);
					//actions.MoveToElement(button).Click().Perform();
					//break;

					var buttons = accountModel.Driver.FindElements(By.CssSelector("div[aria-label=\"Đăng\"]"));
					foreach (var button in buttons)
					{
						var t = button.Text;
						if (t == "Đăng")
						{
							Actions actions = new Actions(accountModel.Driver);
							actions.MoveToElement(button).Click().Perform();
							goto waitUpReel;
						}
					}

				}
				catch
				{
					await Task.Delay(1000);
				}
			}

		waitUpReel:
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Đăng\"]"), 10, count: 1);
			
			await Task.Delay(random.Next(20000, 30000));


			if (commentReel != "")
			{
				pageModel.PageStatus = "Đang comment ...";
				for (int i = 0; i < 10; i++)
				{
					try
					{
						pageSource = accountModel.Driver.PageSource;
						break;
					}
					catch
					{
						Thread.Sleep(1000);
					}
				}

				video_id = RegexHelper.GetValueFromGroup("\"video_id\":(.*?),\"", pageSource);
				if (video_id == "null")
				{
					accountModel.Driver.Navigate().Refresh();
					for (int i = 0; i < 10; i++)
					{
						Thread.Sleep(10000);
						for (int j = 0; j < 10; j++)
						{
							try
							{
								pageSource = accountModel.Driver.PageSource;
								break;
							}
							catch
							{
								Thread.Sleep(1000);
							}
						}
						video_id = RegexHelper.GetValueFromGroup("\"video_id\":(.*?),\"", pageSource);
						if (video_id == "null")
						{
							accountModel.Driver.Navigate().Refresh();
						}
						else if (video_id != "null")
						{
							video_id = video_id.Replace("\"", "");
							break;
						}
					}
				}
				else
				{
					video_id = video_id.Replace("\"", "");
				}
				//Comment
				Thread.Sleep(2000);
				if (!SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("div[role=\"textbox\"]"), commentReel))
				{
					return ResultModel.Fail;
				}
				Thread.Sleep(2000);
				if (!SeleniumHelper.SendKeys(accountModel.Driver, By.CssSelector("div[role=\"textbox\"]"), OpenQA.Selenium.Keys.Enter))
				{
					return ResultModel.Fail;
				}
			}
			pageModel.PageStatus = "Up reel thành công ...";
			await _pageDataService.Update(pageModel.PageID, pageModel);
			await FolderHelper.DeleteFolder(folderPath);
			return ResultModel.Success;
		}

		private async Task<ResultModel> SurfNewFeed(int timeFrom, int timeTo)
		{
			pageModel.PageStatus = "Lướt newfeed ...";
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
					pageModel.PageStatus = "CheckPoint 956!";
					await _pageDataService.Update(pageModel.PageID, pageModel);
					return ResultModel.CheckPoint;
				}
				else if (url.Contains("282/"))
				{
					accountModel.Status = "CheckPoint 282!";
					await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
					pageModel.PageStatus = "CheckPoint 282!";
					await _pageDataService.Update(pageModel.PageID, pageModel);
					return ResultModel.CheckPoint;
				}
				else if (url.Contains("checkpoint"))
				{
					accountModel.Status = "CheckPoint!";
					await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
					pageModel.PageStatus = "CheckPoint !";
					await _pageDataService.Update(pageModel.PageID, pageModel);
					return ResultModel.CheckPoint;
				}
				if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[aria-label=\"Xem gợi ý\"]"), 5))
				{
					for (int i = 0; i < 3; i++)
					{
						await Task.Delay(random.Next(1000, 3000));
						SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Theo dõi\"]"), count: i);
					}
					try
					{
						accountModel.Driver.Navigate().Refresh();
					}
					catch
					{
						return ResultModel.Fail;
					}

				}
			}
			return ResultModel.Success;
		}

		private async Task<ResultModel> ReadNotification()
		{
			pageModel.PageStatus = "Đọc thông báo ...";
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[href=\"/notifications/\"]"));
			_pauseEvent.Wait();
			_cancellationTokenSource.Token.ThrowIfCancellationRequested();
			await Task.Delay(random.Next(4000, 10000));
			pageModel.PageStatus = "Tắt thông báo ...";
			_pauseEvent.Wait();
			_cancellationTokenSource.Token.ThrowIfCancellationRequested();
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
			SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[aria-label=\"Messenger\"]"));
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
			pageModel.PageStatus = "Đang chuyển page profile ...";
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
				pageModel.PageStatus = "Bấm chuyển trang ...";
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

		//Chuyển page bằng cách click avatar
		protected async Task<ResultModel> SwitchToProfileByClickAvatar()
		{
			pageModel.PageStatus = "Đang chuyển page profile ...";
			var pageSource = accountModel.Driver.PageSource;
			var _user = RegexHelper.GetValueFromGroup("__user=(.*?)&", pageSource);
			if (_user != pageModel.PageID)
			{
				if (SeleniumHelper.WaitElement(accountModel.Driver,
					By.CssSelector("div[aria-label=\"Trang cá nhân của bạn\"]")))
				{
					if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Trang cá nhân của bạn\"]")))
					{
						return ResultModel.Fail;
					}
					pageModel.PageStatus = "Check xem có nút xem tất cả trang cá nhân ko ...";
					if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("div[aria-label=\"Xem tất cả trang cá nhân\"]"), 10))
					{
						if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Xem tất cả trang cá nhân\"]")))
						{
							return ResultModel.Fail;
						}
						//if (SeleniumHelper.WaitElement(accountModel.Driver, By.CssSelector("a[aria-label=\"Xem tất cả các Trang\"]"), 10))
						//{
						//	await Task.Delay(random.Next(1000, 2000));
						//	if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("a[aria-label=\"Xem tất cả các Trang\"]")))
						//	{
						//		return ResultModel.Fail;
						//	}
						//}
					}
					await Task.Delay(random.Next(3000, 5000));
					try
					{
						var allElementPage = accountModel.Driver.FindElements(By.CssSelector("div[role=\"listitem\"]"));
						foreach (var element in allElementPage)
						{
							var t = element.Text.Trim();
							if (t.Trim().Contains(pageModel.PageName.Trim()))
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
					await Task.Delay(random.Next(5000, 10000));
				}
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

		public async override Task<ResultModel> CheckLogined()
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
				pageModel.PageStatus = "CheckPoint 956!";
				await _pageDataService.Update(pageModel.PageID, pageModel);
				return ResultModel.CheckPoint;
			}
			else if (url.Contains("282/"))
			{
				accountModel.Status = "CheckPoint 282!";
				await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
				pageModel.PageStatus = "CheckPoint 282!";
				await _pageDataService.Update(pageModel.PageID, pageModel);
				return ResultModel.CheckPoint;
			}
			else if (url.Contains("checkpoint"))
			{
				accountModel.Status = "CheckPoint!";
				await _accountDataService.Update(accountModel.AccountIDKey, accountModel);
				pageModel.PageStatus = "CheckPoint !";
				await _pageDataService.Update(pageModel.PageID, pageModel);
				return ResultModel.CheckPoint;
			}
			pageModel.PageStatus = "Đang check xem login chưa ...";
			if (SeleniumHelper.WaitElement(accountModel.Driver, By.Id("email"), 5))
			{
				//var status = LoginByCookie();
				pageModel.PageStatus = "Đang login...";
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
				pageModel.PageStatus = "Nhập email ...";
				if (accountModel.Driver.FindElement(By.Id("email")).Text == "")
				{
					if (!SeleniumHelper.SendKeys(accountModel.Driver, By.Id("email"), accountModel.UID.ToString()))
						return ResultModel.Fail;
				}

				Thread.Sleep(1000);
				pageModel.PageStatus = "Nhập pass ...";
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
				pageModel.PageStatus = "Check url change ...";
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
				pageModel.PageStatus = "Đang đợi nút lưu trình duyệt ...";
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
