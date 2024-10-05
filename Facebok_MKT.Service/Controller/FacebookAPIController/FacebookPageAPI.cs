﻿using AutoMapper;
using Facebok_MKT.Service.DataService.Accounts;
using Facebok_MKT.Service.DataService.Pages;
using Faceebook_MKT.Domain.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.Controller.FacebookAPIController
{
	public class FacebookPageAPI : BaseFacebookAPI
	{
		private IPageDataService _pageDataService;
		private PageModel _pageModel;
		public FacebookPageAPI(AccountModel accountModel,
			IAccountDataService accountDataService,
			IPageDataService pagedataService,
			int folderAccountIDKey,
			int folderPageIDKey,
			IPageDataService pageDataService,
			PageModel pageModel) : base(accountModel,
										accountDataService,
										pagedataService,
										folderAccountIDKey,
										folderPageIDKey)
		{
			_pageDataService = pageDataService;
			_pageModel = pageModel;
		}

	}
}
