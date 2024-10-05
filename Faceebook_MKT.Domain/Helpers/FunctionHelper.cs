using Leaf.xNet;
using OtpNet;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Helpers
{
	public class FunctionHelper
	{
		public static string ConvertTwoFA(string token)
		{
			for (var i = 0; i < 5; i++)
			{
				try
				{
					var totp = new Totp(Base32Encoding.ToBytes(token));
					var code = totp.ComputeTotp();
					if (code != "")
					{
						return code;
					}
				}
				catch
				{
					//
				}
			}

			return "";
		}
		public static string RandomPasswordString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var random = new Random();
			return (char)random.Next(65, 90) + new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
		public static string GenerateRandomString(int length)
		{
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			char[] randomString = new char[length];

			for (int i = 0; i < length; i++)
			{
				randomString[i] = chars[random.Next(chars.Length)];
			}

			return new string(randomString);
		}
		public static string GenerateRandomStringOnly(int length)
		{
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			char[] randomString = new char[length];

			for (int i = 0; i < length; i++)
			{
				randomString[i] = chars[random.Next(chars.Length)];
			}

			return new string(randomString);
		}
		public static void AddHeaderxNet(HttpRequest rq, string Header)
		{
			var header = Header.Replace("\r", "").Split('\n');
			foreach (var line in header)
			{
				try
				{
					var arr = line.Split(":".ToCharArray(), 2);
					rq.AddHeader(arr[0].Trim(), arr[1].Trim());
				}
				catch { }
			}
		}
		public static void AddHeaderRestSharp(RestRequest rq, string Header)
		{
			var header = Header.Replace("\r", "").Split('\n');
			foreach (var line in header)
			{
				try
				{
					var arr = line.Split(":".ToCharArray(), 2);
					rq.AddHeader(arr[0].Trim(), arr[1].Trim());
				}
				catch { }
			}
		}
		public static void SetCookieToRequestXnet(HttpRequest rq, string cookie, string domain = ".facebook.com")
		{
			rq.Cookies = new CookieStorage();
			var cookies = cookie.Split(';');
			foreach (var ck in cookies)
			{
				try
				{
					var arr = ck.Split("=".ToCharArray(), 2);
					rq.Cookies.Add(new System.Net.Cookie(arr[0].Trim(), arr[1].Trim(), "/", domain));
				}
				catch { }
			}
		}
		public static ProxyClient ConvertToProxyClient(string proxy)
		{
			if (string.IsNullOrEmpty(proxy))
				return null;

			if (proxy.Contains("socks5://"))
			{
				proxy = proxy.Replace("socks5://", "");
				var proxies = proxy.Split(':');
				if (proxies.Length > 2)
				{
					return new Socks5ProxyClient(proxies[0], int.Parse(proxies[1]), proxies[2], proxies[3]);
				}
				else
				{
					return new Socks5ProxyClient(proxies[0], int.Parse(proxies[1]));
				}
			}
			else
			{
				proxy = proxy.Replace("http://", "");
				var proxies = proxy.Split(':');
				if (proxies.Length > 2)
				{
					return new HttpProxyClient(proxies[0], int.Parse(proxies[1]), proxies[2], proxies[3]);
				}
				else
				{
					return new HttpProxyClient(proxies[0], int.Parse(proxies[1]));
				}
			}
		}

		public static WebProxy ParseProxyRestSharp(string proxy)
		{
			if (string.IsNullOrEmpty(proxy))
				return null;

			var proxies = proxy.Split(':');
			var p = new WebProxy(proxies[0], int.Parse(proxies[1]));
			if (proxies.Length > 2)
			{
				p.Credentials = new NetworkCredential(proxies[2], proxies[3]);
			}

			return p;
		}
	}
}
