using Facebok_MKT.Service.DataService.Accounts;
using Facebok_MKT.Service.DataService.Groups;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.Controller.BrowserController
{
	public class FacebookBrowserGroupController : BaseFacebookBrowserController
	{
		private GroupModel _groupModel;
		private IGroupDataService _groupDataService;
		public FacebookBrowserGroupController(AccountModel accountModel,
			IAccountDataService accountDataService,
			IGroupDataService groupDataService,
			ManualResetEventSlim pauseEvent,
			CancellationTokenSource cancellationTokenSource,
			GroupModel groupModel)
			: base(accountModel, accountDataService, pauseEvent, cancellationTokenSource)
		{
			_groupModel = groupModel;
			_groupDataService = groupDataService;
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
				//case "Like":
				//	return await LikePost(Convert.ToInt32(task.Fields[0].Value));
				//case "Comment":
				//	return await CommentPost(Convert.ToInt32(task.Fields[0].Value), task.Fields[2].Value.ToString());

				case "Đọc tin nhắn":
					return await ReadMessenger();
				case "Đọc thông báo":
					return await ReadNotification();
				case "Đăng bài":
					var f = task.Fields[1].Label.ToString();
					var c = task.Fields[2].Value.ToString();
					return await UpPost(task.Fields[1].Label.ToString(), task.Fields[2].Value.ToString());

				default:
					return ResultModel.Fail;
			}
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
		public async Task<ResultModel> UpPost(string content, string filePaths)
		{
			_groupModel.GroupStatus = "Lướt newfeed ...";
			for (int i = 0; i < 2; i++)
			{
				SeleniumHelper.Scroll(accountModel.Driver, 500);
				await Task.Delay(random.Next(1000, 3000));
			}
			SeleniumHelper.ScrollsArrowUp(accountModel.Driver);
			SeleniumHelper.Scroll(accountModel.Driver, -1000);
			Thread.Sleep(1000);
			//try
			//{
			//	var elementButton = accountModel.Driver.FindElements(By.CssSelector("div[role=\"button\"]"));
			//	foreach (var ele in elementButton)
			//	{
			//		var text = ele.Text;
			//		if (text == "Bạn viết gì đi...")
			//		{
			//			Actions actions = new Actions(accountModel.Driver);
			//			actions.MoveToElement(ele).Click().Perform();
			//			break;
			//		}
			//	}
			//}
			//catch
			//{
			//	return ResultModel.Fail;
			//}

			_groupModel.GroupStatus = "Nhập ảnh/Video ...";
			await Task.Delay(random.Next(1000, 3000));
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
				_groupModel.GroupStatus = "Đăng bài thất bại!";
				return ResultModel.Fail;
			}
			_groupModel.GroupStatus = "Điền content ...";
			await Task.Delay(random.Next(2000, 5000));
			#region nhập content 
			if (!SeleniumHelper.SendKeysWithEmoji(accountModel.Driver, By.CssSelector("div[aria-label=\"Tạo bài viết công khai...\"]"), content))
			{
				_groupModel.GroupStatus = "Đăng bài thất bại!";
				return ResultModel.Fail;
			}

			#endregion


			await Task.Delay(random.Next(2000, 5000));
			if (!SeleniumHelper.Click(accountModel.Driver, By.CssSelector("div[aria-label=\"Đăng\"]")))
			{
				_groupModel.GroupStatus = "Đăng bài thất bại!";
				return ResultModel.Fail;
			}
			await Task.Delay(random.Next(2000, 5000));
			try
			{
				var eledang = accountModel.Driver.FindElement(By.CssSelector("div[aria-label=\"Đăng\"]"));
			}
			catch
			{

			}

			if (!SeleniumHelper.WaitElementHidden(accountModel.Driver, By.CssSelector("div[aria-label=\"Đăng\"]"), 60))
			{
				_groupModel.GroupStatus = "Đăng bài thất bại!";
				return ResultModel.Fail;
			}
			_groupModel.GroupStatus = "Đăng bài thành công!";
			return ResultModel.Success;
		}



		public async Task<ResultModel> CheckLoginedAndGotoGroup()
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
			try
			{
				accountModel.Driver.Url = $"https://www.facebook.com/{_groupModel.GroupID}";
			}
			catch
			{
				return ResultModel.Fail;

			}
			return ResultModel.Success;
		}

	}
}
