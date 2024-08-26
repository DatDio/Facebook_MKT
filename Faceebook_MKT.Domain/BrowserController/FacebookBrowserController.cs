using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.BrowserController
{
	public class FacebookBrowserController
	{
		Random random;
		AccountModel account;
		//FacebookAPIController apiFB;
		public FacebookBrowserController(AccountModel account)
		{
			this.account = account;
			random = new Random();
		}
		
		

		public bool LoginUIDPass()
		{
			//FunctionHelper.EditValueColumn(account, "C_Status", "Đang login!", true);
			var t = account.Driver.FindElement(By.Id("email")).Text;
			if (account.Driver.FindElement(By.Id("email")).Text == "")
			{
				if (!SeleniumHelper.SendKeys(account.Driver, By.Id("email"), account.UID))
					return false;
			}

			Thread.Sleep(1000);
			if (!SeleniumHelper.SendKeys(account.Driver, By.Id("pass"), account.Password))
				return false;
			Thread.Sleep(2000);
			if (!SeleniumHelper.Click(account.Driver, By.Name("login")))
				return false;
			var url = account.Driver.Url;
			if (SeleniumHelper.UrlChange(account.Driver, url))
			{
				if (account.Driver.Url == "https://www.facebook.com/checkpoint/?next")
				{
					if (account.C_2FA != "")
					{
						var code2FA = FunctionHelper.ConvertTwoFA(account.C_2FA);
						if (!SeleniumHelper.SendKeys(account.Driver, By.Id("approvals_code"), code2FA))
						{
							return false;
						}
						Thread.Sleep(1000);
						if (!SeleniumHelper.Click(account.Driver, By.Id("checkpointSubmitButton")))
							return false;
					}
				}
			}
			//Lưu trình duyệt
			if (SeleniumHelper.WaitElement(account.Driver, By.CssSelector("button[name=\"submit[Continue]\"]")))
			{
				SeleniumHelper.Click(account.Driver, By.CssSelector("button[name=\"submit[Continue]\"]"));
			}

			if (SeleniumHelper.WaitElement(account.Driver, By.Id("approvals_code")))
			{
				return false;
			}
			return false;
		}
		public bool LoginByCookie()
		{
			//FunctionHelper.EditValueColumn(account, "C_Status", "Đến trang login ...");

			var cookies = account.Cookie.Split(';');
			foreach (var cookie in cookies)
			{
				try
				{
					var arr = cookie.Split("=".ToCharArray(), 2);
					var ck = new Cookie(arr[0].Trim(), arr[1].Trim());
					account.Driver.Manage().Cookies.AddCookie(ck);
				}
				catch
				{
				}
			}
			try
			{
				account.Driver.Url = "https://www.facebook.com/";
			}
			catch
			{
				return false;
			}
			if (SeleniumHelper.WaitElement(account.Driver, By.Id("m_login_email")))
			{
				return false;
			}
			return true;
		}
	}
}
