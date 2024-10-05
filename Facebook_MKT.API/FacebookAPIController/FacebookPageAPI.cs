using AutoMapper;
using Facebook_MKT.API.Helpers;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Faceebook_MKT.Domain.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Facebook_MKT.API.FacebookAPI
{
	public class FacebookPageAPI : BaseFacebookAPI
	{
		private IDataService<Page> _pageDataService;
		private PageModel _pageModel;
		public FacebookPageAPI(AccountModel accountModel,
			IDataService<Account> accountDataService,
			IMapper mapper,
			IDataService<Page> pageDataService,
			PageModel pageModel) : base(accountModel, accountDataService, mapper)
		{
			_pageDataService = pageDataService;
			_pageModel = pageModel;
		}
		public async ResultModel GetAllPage()
		{
			string body = "", refer = "";
			List<PageModel> listPageModels = new List<PageModel>();
			using (var rq = new Leaf.xNet.HttpRequest())
			{
				rq.AllowAutoRedirect = true;
				rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				//account.Cookie = "sb=zYiCYXmTV6Lo2j0SSdD4z-EX; datr=BFBXZS3zHKgU7vEYxxtr9uq0; wl_cbv=v2%3Bclient_version%3A2376%3Btimestamp%3A1702437833; ps_n=0; ps_l=0; dpr=1.309999942779541; wd=798x701; c_user=100088692310375; xs=7%3AlEmBBZtACWloxw%3A2%3A1708170569%3A-1%3A-1%3A%3AAcXEyuGMYqa1OCp2vb6LB_7IcqgD-Cs7vNN9J_m6nQ; fr=1WURFPTaHjsjWEuRB.AWVPrppDlyLBgBB1B1HnK4GEtRE.Bl0N1_..AAA.0.0.Bl0N1_.AWUUwD9vsCE";
				HttpsHelper.SetCookieToRequestXnet(rq, _accountModel.Cookie);
				//account.Proxy = "";
				rq.Proxy = HttpsHelper.ConvertToProxyClient(_accountModel.Proxy);
				HttpsHelper.AddHeaderxNet(rq, @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                Accept-Language:en-US,en;q=0.9
                                sec-ch-ua-mobile:?0
                                sec-ch-ua-platform:\""Windows\""
                                Sec-Fetch-Dest:document
                                Sec-Fetch-Mode:navigate
                                Sec-Fetch-Site:none
                                Sec-Fetch-User:?1
                                Upgrade-Insecure-Requests:1");

				try
				{
					body = rq.Get($"https://graph.facebook.com/v17.0/me?fields=accounts.limit(100){{additional_profile_id,name}}&access_token={account.C_Token}").ToString();
				}
				catch
				{
					return ResultModel.Fail;
				}

				var matches = Regex.Matches(body, "\"additional_profile_id\": \"(.*?)\",");
				var mathchesName = Regex.Matches(body, "\"name\": \"(.*?)\",");
				if (matches.Count == 0)
				{
					return ResultModel.HasNoPage;
				}
				for (int i = 0; i < matches.Count; i++)
				{
					var unicodeString = mathchesName[i].Groups[1].Value;
					string jsonString = $"\"{unicodeString}\"";
					string decodedString = JToken.Parse(jsonString).ToString();
					listPageModels.Add(new PageModel
					{
						C_UIDVia = account.C_UID,
						CookieVia = account.Cookie,
						C_IDPage = matches[i].Groups[1].Value,
						C_NamePage = decodedString,
						C_FolderPage = account.C_Folder,
						ProxyPage = account.Proxy,
						C_GPMIDPage = account.C_GPMID
					});
					lock (ManagePageForm.lockfolder)
					{
						if (!Directory.Exists($"VideoReel/_cbbFolderManageAcc/{matches[i].Groups[1].Value}"))
						{
							Directory.CreateDirectory($"VideoReel/_cbbFolderManageAcc/{matches[i].Groups[1].Value}");
						}
					}
				}
				//Sql.BulkInsert(listPageModels);
				//Sql.ReloadDataFolderManagePage(account.C_Folder);
				//HttpsHelper.EditValueColumn(account, "C_IDPage", account.C_IDPage, true);
				return ResultModel.Success;
			}
		}
	}
}
