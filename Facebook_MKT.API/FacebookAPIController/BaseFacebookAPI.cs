using AutoMapper;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.API.FacebookAPI
{
	public abstract class BaseFacebookAPI
	{
		protected Random _random;
		protected AccountModel _accountModel;
		protected readonly IMapper _mapper;
		protected readonly IDataService<Account> _accountDataService;

		public BaseFacebookAPI(AccountModel accountModel,
			IDataService<Account> accountDataService,
			IMapper mapper)
		{
			_accountModel = accountModel;
			_accountDataService = accountDataService;
			_mapper = mapper;
			_random = new Random();

		}
		public ResultModel CheckLiveUid()
		{
			_accountModel.Status = "Đang check live uid...";
			using (var rq = new Leaf.xNet.HttpRequest())
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



	}
}
