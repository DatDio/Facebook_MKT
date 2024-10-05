using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook_MKT.API.Helpers
{
	public static class HttpsHelper
	{
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
	}
}
