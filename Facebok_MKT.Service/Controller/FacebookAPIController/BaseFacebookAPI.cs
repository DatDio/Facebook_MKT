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
		protected FolderModel _folderAccountModel;
		protected FolderPageModel _folderPageModel;
		protected Random _random;
		protected AccountModel _accountModel;
		protected readonly IAccountDataService _accountDataService;
		protected readonly IPageDataService _pagedataService;
		public BaseFacebookAPI(AccountModel accountModel,
			IAccountDataService accountDataService,
			IPageDataService pagedataService,
			FolderModel folderAccountModel)
		{
			_accountModel = accountModel;
			_accountDataService = accountDataService;
			_pagedataService = pagedataService;
			_folderAccountModel = folderAccountModel;
			_random = new Random();

		}
	
		public bool GetToken()
		{
			_accountModel.Status = "Đang get token ...";
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
			_accountModel.Status = "Get token fail ...";
			return false;

		}
	}
}
