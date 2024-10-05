using AutoMapper;
using Facebok_MKT.Service.DataService.Accounts;
using Facebok_MKT.Service.DataService.Pages;
using Facebook_MKT.Data.Entities;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using Leaf.xNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.Controller.FacebookAPIController
{
	public abstract class BaseFacebookAPI
	{
		protected int _folderAccountIDKey;
		protected int _folderPageIDKey;
		protected Random _random;
		protected AccountModel _accountModel;
		protected readonly IAccountDataService _accountDataService;
		protected readonly IPageDataService _pagedataService;
		public BaseFacebookAPI(AccountModel accountModel,
			IAccountDataService accountDataService,
			IPageDataService pagedataService,
			int folderAccountIDKey,
			int folderPageIDKey)
		{
			_accountModel = accountModel;
			_accountDataService = accountDataService;
			_pagedataService = pagedataService;
			_folderAccountIDKey = folderAccountIDKey;
			_folderPageIDKey = folderPageIDKey;
			_random = new Random();

		}
		public ResultModel CheckLiveUid()
		{
			_accountModel.Status = "Đang check live uid...";
			using (var rq = new HttpRequest())
			{
				rq.AllowAutoRedirect = true;
				rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36 Edg/113.0.1774.57";
				//rq.Proxy = FunctionHelper.ConvertToProxyClient(accountModel.Account.Proxy);

				var body = "";
				try
				{
					rq.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
					rq.AddHeader("Accept-Language", "en-US,en;q=0.9");
					rq.AddHeader("sec-ch-ua-mobile", "?0");
					rq.AddHeader("sec-ch-ua-platform", "\"Windows\"");
					rq.AddHeader("Sec-Fetch-Dest", "document");
					rq.AddHeader("Sec-Fetch-Mode", "navigate");
					rq.AddHeader("Sec-Fetch-Site", "none");
					rq.AddHeader("Sec-Fetch-User", "?1");
					rq.AddHeader("Upgrade-Insecure-Requests", "1");
					body = rq.Get($"https://graph.facebook.com/{_accountModel.UID}/picture?type=normal&redirect=false").ToString();
					if (body.Contains("height") && body.Contains("width"))
					{

						return ResultModel.Success;
					}
				}
				catch
				{
					//
				}

			}
			return ResultModel.Fail;
		}


		public bool GetToken()
		{
			string body = "";
			using (var rq = new HttpRequest())
			{
				rq.AllowAutoRedirect = true;
				//rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				//_accountModel.Cookie = "sb=1REHZqy4tKJ-gwGNzqOqPUNg; datr=1REHZvhOaBgJ5LpJvF9mO1Hu; c_user=100082319049751; ps_n=1; ps_l=1; xs=47%3ANYM4jy5s60oHHg%3A2%3A1712051538%3A-1%3A6278%3A%3AAcXUQJ9mlf0GGCYCQ48fTIeXx3Fh-skZhswaownktgE; fr=1I4OIBpw4mwTh4mBA.AWU9lRJYdnr5CW40Ud_tzVSlliw.Bm9SeM..AAA.0.0.Bm9SyC.AWU5J8K0qN4; dpr=1.309999942779541; presence=C%7B%22t3%22%3A%5B%5D%2C%22utc3%22%3A1727343752084%2C%22v%22%3A1%7D; wd=788x698";
				FunctionHelper.SetCookieToRequestXnet(rq, _accountModel.Cookie);
				rq.Proxy = FunctionHelper.ConvertToProxyClient(_accountModel.Proxy);

				FunctionHelper.AddHeaderxNet(rq, @"Connection: keep-alive
										Cache-Control: max-age=0
										sec-ch-ua: ""Not)A;Brand"";v=""99"", ""Google Chrome"";v=""127"", ""Chromium"";v=""127""
										sec-ch-ua-mobile: ?0
										sec-ch-ua-platform: ""Windows""
										Upgrade-Insecure-Requests: 1
										User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36
										Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
										Sec-Fetch-Site: none
										Sec-Fetch-Mode: navigate
										Sec-Fetch-User: ?1
										Sec-Fetch-Dest: document
										Accept-Language: vi,fr-FR;q=0.9,fr;q=0.8,en-US;q=0.7,en;q=0.6");

				try
				{
					body = rq.Get("https://www.facebook.com/ajax/bootloader-endpoint/?modules=AdsCanvasComposerDialog.react&__a=1").ToString();
				}
				catch
				{
					//
				}

				var token = RegexHelper.GetValueFromGroup("\"access_token\":\"EAA(.*?)\"", body);
				if (token == "")
				{

				}
				if (token != "")
				{
					_accountModel.Token = "EAA" + token;
					return true;
				}
			}
			return false;

		}
		public bool GetAllPage()
		{
			string body = "", refer = "";
			List<PageModel> listPageModels = new List<PageModel>();
			using (var rq = new HttpRequest())
			{
				rq.AllowAutoRedirect = true;
				//rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				_accountModel.Cookie = "sb=M0wuZfJjr5WMqpJrlHPd8TH7; datr=M0wuZefjuyoRYlGUUC_df0Kd; ps_n=1; ps_l=1; c_user=100088871597130; wd=1422x704; dpr=1.309999942779541; fr=1bF0ytxK3rhlwctW2.AWWGqVI0XoPuwbShPurPV23dHAQ.Bm9S8O..AAA.0.0.Bm9S8O.AWXdnfnKbAY; xs=14%3Ay81IceGWGpND7A%3A2%3A1722762441%3A-1%3A7946%3A%3AAcXmCA0nG7rZs-3wyxx33Xy4b-SMtL8NDI5BtNTna-E; presence=C%7B%22t3%22%3A%5B%5D%2C%22utc3%22%3A1727344403828%2C%22v%22%3A1%7D; ar_debug=1";
				FunctionHelper.SetCookieToRequestXnet(rq, _accountModel.Cookie);
				rq.Proxy = FunctionHelper.ConvertToProxyClient(_accountModel.Proxy);
				FunctionHelper.AddHeaderxNet(rq, @"Connection: keep-alive
										Cache-Control: max-age=0
										sec-ch-ua: ""Not)A;Brand"";v=""99"", ""Google Chrome"";v=""127"", ""Chromium"";v=""127""
										sec-ch-ua-mobile: ?0
										sec-ch-ua-platform: ""Windows""
										Upgrade-Insecure-Requests: 1
										User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36
										Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
										Sec-Fetch-Site: none
										Sec-Fetch-Mode: navigate
										Sec-Fetch-User: ?1
										Sec-Fetch-Dest: document
										Accept-Language: vi,fr-FR;q=0.9,fr;q=0.8,en-US;q=0.7,en;q=0.6");

				try
				{
					var url = "https://graph.facebook.com/v17.0/me?fields=accounts.limit(100){additional_profile_id,name}&access_token=EAAI4BG12pyIBOZBEywMh8ffKDlLNlCthatyaR3W7YML6VPeKPvmfTpY7AdlDZBSz42RoFaa0cnlUYDG0vZC6ocbYPk4u5E8fwXgZC43ZCfKZACJZB85LYdlaZB6XN2PomqNKe7m3pRyR7TsOvEYWGNcCol54yanfmRhLJDZCOZAZBhvcNqi37m16XMl8933RPBdjhS9mOC1t35AIgZDZD";
					var response = rq.Get(url);

					// Kiểm tra xem phản hồi có lỗi hay không
					if (response.IsOK)
					{
						body = response.ToString(); // Hoặc sử dụng response.ToJson()
						Console.WriteLine(body);
					}
					try
					{
						//body = bodyd.ToString();
					}
					catch
					{

					}

				}
				catch
				{
					//return ResultModel.Fail;
				}


				var matches = Regex.Matches(body, "\"additional_profile_id\": \"(.*?)\",");
				var mathchesName = Regex.Matches(body, "\"name\": \"(.*?)\",");
				if (matches.Count == 0)
				{
					return true;
				}
				for (int i = 0; i < matches.Count; i++)
				{
					var unicodeString = mathchesName[i].Groups[1].Value;
					string jsonString = $"\"{unicodeString}\"";
					string decodedString = JToken.Parse(jsonString).ToString();
					//listPageModels.Add(new PageModel
					//{
					//	C_UIDVia = account.C_UID,
					//	CookieVia = account.Cookie,
					//	C_IDPage = matches[i].Groups[1].Value,
					//	C_NamePage = decodedString,
					//	C_FolderPage = account.C_Folder,
					//	ProxyPage = account.Proxy,
					//	C_GPMIDPage = account.C_GPMID
					//});
					//lock (ManagePageForm.lockfolder)
					//{
					//	if (!Directory.Exists($"VideoReel/_cbbFolderManageAcc/{matches[i].Groups[1].Value}"))
					//	{
					//		Directory.CreateDirectory($"VideoReel/_cbbFolderManageAcc/{matches[i].Groups[1].Value}");
					//	}
					//}
				}
				//Sql.BulkInsert(listPageModels);
				//Sql.ReloadDataFolderManagePage(account.C_Folder);
				//FunctionHelper.EditValueColumn(account, "C_IDPage", account.C_IDPage, true);
				return true;
			}
		}


	}
}
