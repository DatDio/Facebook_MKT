using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Playwright;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.AnditectBrowserController
{
	public class NstBrowserAPI
	{
		private string _url = "http://localhost:8848";
		private string _apiKey = "381045f3-acd4-4735-8a8a-7ec3e3ebd22a";

		public NstBrowserAPI() { }


	//	public async Task<string> Lanch_And_Connect_Browser()
	//	{
	//		var playwright = await Playwright.CreateAsync();
	//		// Kết nối tới phiên bản trình duyệt hiện tại thông qua WebSocketDebuggerUrl
	//		var browser = await playwright.Chromium.ConnectOverCDPAsync("ws://127.0.0.1:22974/devtools/browser/dda4abe0-6912-4044-b91c-08bd53bb754b");
	//		var p = browser.Contexts.Count();
	//		var context = browser.Contexts.FirstOrDefault();
	//		//context.BackgroundPage
	//		if (context == null)
	//		{

	//			return;
	//		}

	//		// Lấy trang (tab) đầu tiên
	//		var page = context.Pages.FirstOrDefault();
	//		if (page == null)
	//		{
	//			return;
	//		}

	//		// Điều khiển tab hiện tại
	//		await page.GotoAsync("https://www.youtube.com");




	//		var NstBrowser = new NstBrowserAPI();

	//		await NstBrowser.CreateProfile("profilecreated");
	//		var accountModeldfgdfg = AccountsSeleted[0];
	//		var browserService = new BrowserService(accountModeldfgdfg, _accountDataService);
	//		// = await NstBrowser.UpdateProxy("24be1457-30f2-4144-a5ea-d40cd1e51bdf", "171.236.163.25:33253:1mdTqyOKA:PTBtrr");
	//		await browserService.OpenChromeNST(_apiGPMUrl,
	//													   accountModeldfgdfg.GPMID, accountModeldfgdfg.UID,
	//		accountModeldfgdfg.UserAgent, scale: _scale,
	//													   accountModeldfgdfg.Proxy, position: "0,0");
	//		var ws = await NstBrowser.StartProfile("aa2a5268-7292-4043-af02-f45990690e87");


	//		string _url = "http://localhost:8848";
	//		string _apiKey = "381045f3-acd4-4735-8a8a-7ec3e3ebd22a";
	//		var config = new
	//		{
	//			headless = true, // true hoặc false
	//			autoClose = true,
	//			args = new Dictionary<string, string>
	//{
	//	{ "--window-position", "100,200" } // Cài đặt tọa độ cửa sổ
 //   }
	//		};

	//		// Serialize config to JSON
	//		string configJson = JsonSerializer.Serialize(config);

	//		// Tạo truy vấn URL
	//		var query = new FormUrlEncodedContent(new[]
	//		{
	//		new KeyValuePair<string, string>("x-api-key", _apiKey),
	//		new KeyValuePair<string, string>("config", Uri.EscapeDataString(JsonSerializer.Serialize(configJson)))
	//	});

	//		string queryString = await query.ReadAsStringAsync();
	//		string browserWSEndpoint = $"ws://localhost:8848/devtool/launch/27cc8185-7ac0-4f00-ba70-f8a5de00864b?{queryString}";
	//		var playwrightgs = await Playwright.CreateAsync();

	//		// Địa chỉ WebSocketDebugger URL



	//		// Kết nối tới phiên bản trình duyệt hiện tại thông qua WebSocketDebuggerUrl
	//		var browserdfg = await playwrightgs.Chromium.ConnectOverCDPAsync(browserWSEndpoint);

	//		// Build the request URL
	//		List<string> listProfile = new List<string>()
	//			{
	//				"27cc8185-7ac0-4f00-ba70-f8a5de00864b",
	//				"f2884872-f8bf-4669-af56-e89df7eeba4c"
	//			};

	//		await Parallel.ForEachAsync(listProfile, new ParallelOptions
	//		{
	//			MaxDegreeOfParallelism = 1 // Số luồng tối đa chạy song song
	//		}, async (profile, cancellationToken) =>
	//		{
	//			string profileid = "";
	//			lock (lockFolder)
	//			{
	//				profileid = profile;
	//			}

	//			// Build the WebSocketDebugger URL với profileid
	//			string url = $"ws://localhost:8848/devtool/launch/{profileid}?{query}";

	//			// Khởi tạo Playwright
	//			var playwright = await Playwright.CreateAsync();

	//			// Địa chỉ WebSocketDebugger URL
	//			string webSocketDebuggerUrl = url;

	//			try
	//			{
	//				// Kết nối tới phiên bản trình duyệt hiện tại thông qua WebSocketDebuggerUrl
	//				var browser = await playwright.Chromium.ConnectOverCDPAsync(webSocketDebuggerUrl);

	//				// Lấy context đầu tiên
	//				var context = browser.Contexts.FirstOrDefault();
	//				if (context == null)
	//				{
	//					Console.WriteLine($"No context found for profile: {profileid}");
	//					return;
	//				}

	//				// Lấy trang (tab) đầu tiên
	//				var page = context.Pages.FirstOrDefault();
	//				if (page == null)
	//				{
	//					Console.WriteLine($"No page found for profile: {profileid}");
	//					return;
	//				}

	//				// Điều khiển tab hiện tại
	//				await page.GotoAsync("https://www.youtube.com");
	//				await browser.CloseAsync();
	//				Console.WriteLine($"Navigated to YouTube for profile: {profileid}");
	//			}
	//			catch (Exception ex)
	//			{
	//				Console.WriteLine($"Error for profile {profileid}: {ex.Message}");
	//			}
	//		});



	//		if (ws == "")
	//		{



	//		}
	//	}



		public async Task<string> StartProfile(string profileId)
		{
			var options = new RestClientOptions(_url)
			{
				MaxTimeout = -1 // No timeout
			};

			using (var restClient = new RestClient(options))
			{
				try
				{
					// Define the config object
					var config = new
					{
						headless = false, // You can customize this
						autoClose = true
					};

					// Serialize the config object to JSON
					string configJson = JsonSerializer.Serialize(config);
					string query = $"x-api-key={Uri.EscapeDataString(_apiKey)}&config={Uri.EscapeDataString(configJson)}";

					// Build the request URL
					string url = $"{_url}/devtool/launch/{profileId}?{query}";

					// Create the request
					var request = new RestRequest(url, Method.Get);
					request.AddHeader("Content-Type", "application/json");
					request.AddHeader("x-api-key", _apiKey);

					// Send the request and get the response
					RestResponse response = await restClient.ExecuteAsync(request);

					// Check if the response is not null or empty
					string responseContent = response.Content ?? "";

					if (!string.IsNullOrEmpty(responseContent))
					{
						var ws = RegexHelper.GetValueFromGroup("\"webSocketDebuggerUrl\":\"(.*?)\"},", responseContent);
						// You could add additional checks here to validate the content
						return ws;
					}
					else
					{
						Console.WriteLine("Response is empty.");
						return "";
					}
				}
				catch
				{


					return "";
				}
			}
		}
		public async Task<bool> CreateProfile(string ProfileName)
		{
			var options = new RestClientOptions(_url)
			{
				MaxTimeout = -1,
			};
			using (var restClient = new RestClient(options))
			{
				string respone = "";
				var request = new RestRequest($"api/agent/profile", Method.Post);
				request.AddHeader("Content-Type", "application/json");

				var body = @"{
				" + "\n" +
							@"    ""args"": {
				" + "\n" +
							@"       ""--window-size"":""400,400""
				" + "\n" +
							@"    },
				" + "\n" +
							@"    ""fingerprint"": {
				" + "\n" +
							@"        ""deviceMemory"": 8,
				" + "\n" +
							@"        ""disableImageLoading"": false,
				" + "\n" +
							@"        ""doNotTrack"": true,
				" + "\n" +
							@"        ""flags"": {
				" + "\n" +
							@"            ""audio"": ""Real"",
				" + "\n" +
							@"            ""canvas"": ""Noise"",
				" + "\n" +
							@"            ""clientRect"": ""Noise"",
				" + "\n" +
							@"            ""gpu"": ""Disabled"",
				" + "\n" +
							@"            ""localization"": ""Custom"",
				" + "\n" +
							@"            ""screen"": ""Custom"",
				" + "\n" +
							@"            ""speech"": ""Masked"",
				" + "\n" +
							@"            ""webgl"": ""Noise""
				" + "\n" +
							@"        },
				" + "\n" +
							@"        ""hardwareConcurrency"": 14,
				" + "\n" +
							@"        ""localization"": {
				" + "\n" +
							@"            ""basedOnProxy"": true,
				" + "\n" +
							@"            ""languages"": [
				" + "\n" +
							@"                ""en-US"",""en""
				" + "\n" +
							@"            ]
				" + "\n" +
							@"        },
				" + "\n" +
							@"        ""restoreLastSession"": false,
				" + "\n" +
							@"        ""webrtc"": {}
				" + "\n" +
							@"    },
				" + "\n" +
							@"    ""kernel"": ""chromium"",
				" + "\n" +
							@"    ""name"": ""4"",
				" + "\n" +
							@"    ""platform"": ""windows"",
				" + "\n" +
							@"    ""kernelMilestone"":""128""
				" + "\n" +
							@"}";
				request.AddHeader("x-api-key", _apiKey);
				request.AddParameter("application/json", body, ParameterType.RequestBody);
				RestResponse response = await restClient.ExecuteAsync(request);
				respone = response.Content == null ? "" : response.Content;
				if (respone != "")
				{
					return true;
				}
			}

			return true;
		}
		public async Task<bool> UpdateProxy(string profileID, string proxy)
		{
			var options = new RestClientOptions(_url)
			{
				MaxTimeout = -1,
			};
			using (var restClient = new RestClient(options))
			{
				string respone = "";
				var request = new RestRequest($"api/agent/profile/proxy/batch", Method.Put);
				request.AddHeader("Content-Type", "application/json");
				var body = @"{" + "\n" +
				@"    ""profileIds"": [" + "\n" +
				$@"        ""{profileID}""" + "\n" +
				@"    ]," + "\n" +
				@"    ""proxyConfig"": {" + "\n" +
				$@"        ""checker"": ""Nstbrowser""," + "\n" +
				@"        ""host"": ""string""," + "\n" +
				@"        ""password"": ""string""," + "\n" +
				@"        ""port"": ""string""," + "\n" +
				@"        ""protocol"": ""string""," + "\n" +
				@"        ""proxySetting"": ""string""," + "\n" +
				@"        ""proxyType"": ""string""," + "\n" +
				$@"        ""url"": ""{proxy}""," + "\n" +
				@"        ""username"": ""string""" + "\n" +
				@"    }" + "\n" +
				@"}";
				request.AddHeader("x-api-key", _apiKey);
				request.AddParameter("application/json", body, ParameterType.RequestBody);
				RestResponse response = await restClient.ExecuteAsync(request);
				respone = response.Content == null ? "" : response.Content;
				if (respone != "")
				{
					return true;
				}
			}

			return true;
		}

		public async Task<bool> StopAllProfile()
		{
			var options = new RestClientOptions(_url)
			{
				MaxTimeout = -1,
			};
			using (var restClient = new RestClient(options))
			{
				string respone = "";
				var request = new RestRequest($"api/agent/browser/stopAll", Method.Get);
				request.AddHeader("Content-Type", "application/json");

				request.AddHeader("x-api-key", _apiKey);
				//request.AddParameter("application/json", ParameterType.RequestBody);
				RestResponse response = await restClient.ExecuteAsync(request);
				respone = response.Content == null ? "" : response.Content;
				if (respone != "")
				{
					return true;
				}
			}

			return true;
		}
	}
}
