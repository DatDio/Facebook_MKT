using AutoMapper;
using Facebok_MKT.Service.DataService.Accounts;
using Facebok_MKT.Service.DataService.Pages;
using Facebook_MKT.Data.Entities;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using Faceebook_MKT.Domain.Systems;
using Leaf.xNet;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Facebok_MKT.Service.Controller.FacebookAPIController
{
	public class FacebookAccountAPI : BaseFacebookAPI
	{
		
		private object _lockFolder = new object();
		private FolderPageModel _folderPageModel;
		public FacebookAccountAPI(AccountModel accountModel,
			IAccountDataService accountDataService,
			IPageDataService pagedataService,
			FolderModel folderAccountModel,
			FolderPageModel folderPageModel = null) : base(accountModel, accountDataService, pagedataService, folderAccountModel
			)
		{
			_folderPageModel = folderPageModel;
		}

		public async Task<ObservableCollection<PageModel>> GetAllPageRestSharp()
		{
			var tokenStatus = GetToken();
			if (tokenStatus == false)
			{
				return null;
			}
			try
			{
				_accountModel.Status = "Get all page ...";
				var listPageModels = new ObservableCollection<PageModel>();
				RestClientOptions clientOptions = new RestClientOptions("https://graph.facebook.com/")
				{
					Proxy = FunctionHelper.ParseProxyRestSharp(_accountModel.Proxy)

				};

				using (var client = new RestClient(clientOptions))
				{
					var request = new RestRequest($"v17.0/me?fields=accounts.limit(100){{additional_profile_id,name}}&access_token={_accountModel.Token}", Method.Get);

					FunctionHelper.AddHeaderRestSharp(request, @"Connection: keep-alive
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
						//_accountModel.Cookie = "sb=M0wuZfJjr5WMqpJrlHPd8TH7; datr=M0wuZefjuyoRYlGUUC_df0Kd; ps_n=1; ps_l=1; c_user=100088871597130; wd=1422x704; dpr=1.309999942779541; ar_debug=1; i_user=61557646964362; presence=C%7B%22t3%22%3A%5B%5D%2C%22utc3%22%3A1727347633531%2C%22v%22%3A1%7D; fr=1t2TMXYuAMbJgobz3.AWWZBkGAgAXS9rikMW8kGOCXFUM.Bm9T1y..AAA.0.0.Bm9T1y.AWW4glZ6tmc; xs=14%3Ay81IceGWGpND7A%3A2%3A1722762441%3A-1%3A7946%3A%3AAcX768kV-6X3Wv10ETd9JC9MQHPMUrsEDxIzqNNR3VM";
						var cks = _accountModel.Cookie.Split(';');
						foreach (var ck in cks)
						{
							if (ck.Trim() == "")
								continue;

							var ckss = ck.Split("=".ToCharArray(), 2);

							request.AddCookie(ckss[0].Trim(), ckss[1].Trim(), "/", ".facebook.com");
						}
					}
					catch { }
					var body = "";
					for (var i = 0; i < 3; i++)
					{
						RestResponse response = client.Execute(request);
						body = response.Content == null ? "" : response.Content;

						if (body != "")
						{
							break;
						}
					}
					var matches = Regex.Matches(body, "\"additional_profile_id\": \"(.*?)\",");
					var mathchesName = Regex.Matches(body, "\"name\": \"(.*?)\",");
					if (matches.Count == 0)
					{
						return null;
					}
					for (int i = 0; i < matches.Count; i++)
					{
						var unicodeString = mathchesName[i].Groups[1].Value;
						string jsonString = $"\"{unicodeString}\"";
						string pageName = JToken.Parse(jsonString).ToString();
						var pageID = matches[i].Groups[1].Value;
						var pageModel = new PageModel
						{
							PageID = matches[i].Groups[1].Value,
							AccountIDKey = _accountModel.AccountIDKey,
							PageName = pageName,
							FolderIdKey = _folderPageModel.FolderIdKey,

						};
						lock (_lockFolder)
						{
							if (!Directory.Exists($"{SystemContants.FolderVideoPage}/{_folderPageModel.FolderName}/{pageID}"))
							{
								Directory.CreateDirectory($"{SystemContants.FolderVideoPage}/{_folderPageModel.FolderName}/{pageID}");

							}
							pageModel.PageFolderVideo = $"{SystemContants.FolderVideoPage}/{_folderPageModel.FolderName}/{pageID}";
						}

						listPageModels.Add(pageModel);
					}
					for (int i = listPageModels.Count - 1; i >= 0; i--)
					{
						var page = await _pagedataService.Create(listPageModels[i]);

						if (page == false)
						{
							listPageModels.RemoveAt(i); // Xóa phần tử tại chỉ số hiện tại
						}
					}

					return listPageModels;

				}
			}
			catch
			{
				//
			}
			return null;
		}

		public ResultModel LoginWWW(AccountModel account)
		{
			string body = "", refer = "", url = "", jazoest = "",
				   lsd = "", login_source = "", submit = "",
				   fb_dtsg = "", nh = "", _js_datr = "";
			RequestParams pr = null;
			Account accountEnity = null;
			using (var rq = new HttpRequest())
			{
				rq.Cookies = new CookieStorage();
				rq.AllowAutoRedirect = true;
				account.Proxy = "";
				rq.Proxy = FunctionHelper.ConvertToProxyClient(account.Proxy);
				rq.UserAgent = account.UserAgent;
				//load trang chủ
				try
				{
					FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                        sec-ch-ua-mobile: ?0
                                        sec-ch-ua-platform: ""Windows""
                                        DNT: 1
                                        Upgrade-Insecure-Requests: 1
                                        Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                        Sec-Fetch-Site: none
                                        Sec-Fetch-Mode: navigate
                                        Sec-Fetch-User: ?1
                                        Sec-Fetch-Dest: document
                                        Accept-Language: en-US,en;q=0.9");

					body = rq.Get("https://www.facebook.com/").ToString();
					refer = rq.Address.AbsoluteUri;
				}
				catch { }

				_js_datr = RegexHelper.GetValueFromGroup("_js_datr\",\"(.*?)\"", body);
				if (_js_datr != "")
				{
					rq.Cookies.Add(new System.Net.Cookie("_js_datr", _js_datr, "/", ".facebook.com"));
					rq.Cookies.Add(new System.Net.Cookie("wd", "2550x1274", "/", ".facebook.com"));
				}

				//bấm accept all cookie
				if (body.Contains("accept_only_essential"))
				{
					jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
					lsd = RegexHelper.GetValueFromGroup("name=\"lsd\" value=\"(.*?)\"", body);

					try
					{
						pr = new RequestParams
						{
							["accept_only_essential"] = "false",
							["dpr"] = "1",
							["lsd"] = lsd,
							["jazoest"] = jazoest
						};

						FunctionHelper.AddHeaderxNet(rq, $@"accept: */*
                                            accept-language: en-US,en;q=0.9
                                            content-type: application/x-www-form-urlencoded
                                            referer: {refer}
                                            sec-ch-ua-mobile: ?0
                                            sec-ch-ua-platform: ""Windows""
                                            sec-fetch-dest: empty
                                            sec-fetch-mode: cors
                                            sec-fetch-site: same-origin
                                            x-asbd-id: 129477
                                            x-fb-lsd: {lsd}");

						rq.Post("https://www.facebook.com/cookie/consent/", pr);
					}
					catch { }
				}

				//bấm login
				url = RegexHelper.GetValueFromGroup("data-testid=\"royal_login_form\" action=\"(.*?)\"", body);
				if (url != "")
				{
					jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
					lsd = RegexHelper.GetValueFromGroup("name=\"lsd\" value=\"(.*?)\"", body);
					login_source = RegexHelper.GetValueFromGroup("name=\"login_source\" value=\"(.*?)\"", body);

					try
					{
						pr = new RequestParams
						{
							["jazoest"] = jazoest,
							["lsd"] = lsd,
							["email"] = account.UID,
							["login_source"] = login_source,
							["next"] = "",
							["encpass"] = account.Password,
						};

						FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                        Cache-Control: max-age=0
                                        sec-ch-ua-mobile: ?0
                                        sec-ch-ua-platform: ""Windows""
                                        Origin: https://www.facebook.com
                                        DNT: 1
                                        Upgrade-Insecure-Requests: 1
                                        Content-Type: application/x-www-form-urlencoded
                                        Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                        Sec-Fetch-Site: same-origin
                                        Sec-Fetch-Mode: navigate
                                        Sec-Fetch-User: ?1
                                        Sec-Fetch-Dest: document
                                        Referer: {refer}
                                        Accept-Language: en-US,en;q=0.9");

						body = rq.Post("https://www.facebook.com" + url, pr).ToString();
						refer = rq.Address.AbsoluteUri;
					}
					catch { }
				}

				rq.Cookies.Remove("https://www.facebook.com/", "_js_datr");

				if (refer.Contains("https://www.facebook.com/checkpoint/?next"))
				{
					for (int i = 0; i < 3; i++)
					{
						//check 2fa
						if (body.Contains("approvals_code"))
						{
							submit = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("button value=\"(.*?)\" class=\"(.*?)\" id=\"checkpointSubmitButton\" name=\"submit\\[Continue\\]\"", body));
							if (submit != "")
							{
								jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
								fb_dtsg = RegexHelper.GetValueFromGroup("name=\"fb_dtsg\" value=\"(.*?)\"", body);
								nh = RegexHelper.GetValueFromGroup("name=\"nh\" value=\"(.*?)\"", body);

								try
								{
									pr = new RequestParams
									{
										["jazoest"] = jazoest,
										["fb_dtsg"] = fb_dtsg,
										["nh"] = nh,
										["approvals_code"] = FunctionHelper.ConvertTwoFA(account.C_2FA),
										["submit[Continue]"] = submit
									};

									FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                            Cache-Control: max-age=0
                                            sec-ch-ua-mobile: ?0
                                            sec-ch-ua-platform: ""Windows""
                                            Origin: https://www.facebook.com
                                            DNT: 1
                                            Upgrade-Insecure-Requests: 1
                                            Content-Type: application/x-www-form-urlencoded
                                            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: navigate
                                            Sec-Fetch-User: ?1
                                            Sec-Fetch-Dest: document
                                            Referer: {refer}
                                            Accept-Language: en-US,en;q=0.9");

									body = rq.Post("https://www.facebook.com/checkpoint/?next", pr).ToString();
									refer = rq.Address.AbsoluteUri;
								}
								catch { }
							}
						}

						//check continue
						submit = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("button value=\"(.*?)\" class=\"(.*?)\" id=\"checkpointSubmitButton\" name=\"submit\\[Continue\\]\"", body));
						if (submit != "")
						{
							jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
							fb_dtsg = RegexHelper.GetValueFromGroup("name=\"fb_dtsg\" value=\"(.*?)\"", body);
							nh = RegexHelper.GetValueFromGroup("name=\"nh\" value=\"(.*?)\"", body);

							try
							{
								pr = new RequestParams
								{
									["jazoest"] = jazoest,
									["fb_dtsg"] = fb_dtsg,
									["nh"] = nh,
									["submit[Continue]"] = submit
								};

								FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                            Cache-Control: max-age=0
                                            sec-ch-ua-mobile: ?0
                                            sec-ch-ua-platform: ""Windows""
                                            Origin: https://www.facebook.com
                                            DNT: 1
                                            Upgrade-Insecure-Requests: 1
                                            Content-Type: application/x-www-form-urlencoded
                                            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: navigate
                                            Sec-Fetch-User: ?1
                                            Sec-Fetch-Dest: document
                                            Referer: {refer}
                                            Accept-Language: en-US,en;q=0.9");

								body = rq.Post("https://www.facebook.com/checkpoint/?next", pr).ToString();
								refer = rq.Address.AbsoluteUri;
							}
							catch { }
						}

						//check This was me
						submit = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("button value=\"(.*?)\" class=\"(.*?)\" id=\"checkpointSubmitButton\" name=\"submit\\[This was me\\]\"", body));
						if (submit != "")
						{
							jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
							fb_dtsg = RegexHelper.GetValueFromGroup("name=\"fb_dtsg\" value=\"(.*?)\"", body);
							nh = RegexHelper.GetValueFromGroup("name=\"nh\" value=\"(.*?)\"", body);

							try
							{
								pr = new RequestParams
								{
									["jazoest"] = jazoest,
									["fb_dtsg"] = fb_dtsg,
									["nh"] = nh,
									["submit[This was me]"] = submit
								};

								FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                            Cache-Control: max-age=0
                                            sec-ch-ua-mobile: ?0
                                            sec-ch-ua-platform: ""Windows""
                                            Origin: https://www.facebook.com
                                            DNT: 1
                                            Upgrade-Insecure-Requests: 1
                                            Content-Type: application/x-www-form-urlencoded
                                            Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: navigate
                                            Sec-Fetch-User: ?1
                                            Sec-Fetch-Dest: document
                                            Referer: {refer}
                                            Accept-Language: en-US,en;q=0.9");

								body = rq.Post("https://www.facebook.com/checkpoint/?next", pr).ToString();
								refer = rq.Address.AbsoluteUri;
							}
							catch { }
						}

						if (refer == "https://www.facebook.com/")
						{
							rq.Cookies.Remove("https://www.facebook.com/", "checkpoint");
							break;
						}
					}
				}

				if (refer.Contains("956/"))
				{
					_accountModel.Status = "Login bị checkpoint 956";
					_accountDataService.Update(_accountModel.AccountIDKey, _accountModel);
					return ResultModel.CheckPoint;
				}

				if (refer.Contains("282/"))
				{
					_accountModel.Status = "Login bị checkpoint 282";
					_accountDataService.Update(_accountModel.AccountIDKey, _accountModel);
					return ResultModel.CheckPoint;
				}

				if (refer.Contains("checkpoint"))
				{
					_accountModel.Status = "Login bị checkpoint ";
					_accountDataService.Update(_accountModel.AccountIDKey, _accountModel);
					return ResultModel.CheckPoint;
				}

				account.Cookie = rq.Cookies.GetCookieHeader("https://www.facebook.com/");
				if (!account.Cookie.Contains("c_user"))
				{
					_accountModel.Status = "Login thất bại";
					_accountDataService.Update(_accountModel.AccountIDKey, _accountModel);
					return ResultModel.Fail;
				}
				_accountModel.Cookie =
				_accountModel.Status = "Login thành công!";
				_accountDataService.Update(_accountModel.AccountIDKey, _accountModel);
				return ResultModel.Fail;
			}
			return ResultModel.Success;
		}

		public ResultModel RegPage(string name, string link)
		{
			string body = "", refer = "", jazoest = "", lsd = "", fb_dtsg = "", id_page = "", fbid = "";
			RequestParams pr = null;

			using (var rq = new HttpRequest())
			{
				rq.Cookies = new CookieStorage();
				rq.AllowAutoRedirect = true;
				rq.Proxy = FunctionHelper.ConvertToProxyClient(_accountModel.Proxy);
				FunctionHelper.SetCookieToRequestXnet(rq, _accountModel.Cookie);
				rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				//load trang chủ
				try
				{
					FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                        sec-ch-ua-mobile: ?0
                                        sec-ch-ua-platform: ""Windows""
                                        DNT: 1
                                        Upgrade-Insecure-Requests: 1
                                        Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                        Sec-Fetch-Site: none
                                        Sec-Fetch-Mode: navigate
                                        Sec-Fetch-User: ?1
                                        Sec-Fetch-Dest: document
                                        Accept-Language: en-US,en;q=0.9");

					body = rq.Get("https://www.facebook.com/pages/creation/?ref_type=launch_point").ToString();
					refer = rq.Address.AbsoluteUri;
				}
				catch
				{
					return ResultModel.Fail;
				}

				if (refer.Contains("checkpoint"))
					return ResultModel.CheckPoint;

				fb_dtsg = RegexHelper.GetValueFromGroup("DTSGInitialData\",\\[\\],{\"token\":\"(.*?)\"", body);
				lsd = RegexHelper.GetValueFromGroup("LSD\",\\[\\],{\"token\":\"(.*?)\"", body);
				jazoest = RegexHelper.GetValueFromGroup("jazoest=(.*?)\"", body);

				//lấy categories
				pr = new RequestParams
				{
					["av"] = _accountModel.UID,
					["dpr"] = "1",
					["fb_dtsg"] = fb_dtsg,
					["lsd"] = lsd,
					["jazoest"] = jazoest,
					["fb_api_caller_class"] = "RelayModern",
					["fb_api_req_friendly_name"] = "usePagesCometAdminEditingCategoryDataSourceQuery",
					["variables"] = $"{{\"params\":{{\"search_string\":\"{FunctionHelper.GenerateRandomStringOnly(1)}\"}}}}",
					["server_timestamps"] = "true",
					["doc_id"] = "6219626854831519"
				};

				try
				{
					FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                        DNT: 1
                                        sec-ch-ua-mobile: ?0
                                        X-FB-Friendly-Name: usePagesCometAdminEditingCategoryDataSourceQuery
                                        X-FB-LSD: {lsd}
                                        Content-Type: application/x-www-form-urlencoded
                                        X-ASBD-ID: 129477
                                        dpr: 1
                                        sec-ch-prefers-color-scheme: dark
                                        sec-ch-ua-platform: ""Windows""
                                        Accept: */*
                                        Origin: https://www.facebook.com
                                        Sec-Fetch-Site: same-origin
                                        Sec-Fetch-Mode: cors
                                        Sec-Fetch-Dest: empty
                                        Referer: https://www.facebook.com/pages/creation/?ref_type=launch_point
                                        Accept-Language: en-US,en;q=0.9");

					body = rq.Post("https://www.facebook.com/api/graphql/", pr).ToString();
				}
				catch { }

				var matches = Regex.Matches(body, "\"category_id\":\"(.*?)\"");
				if (matches.Count == 0)
					return ResultModel.Fail;
				//var variables = $"{{\"input\":{{\"bio\":\"\",\"categories\":[\"{matches[new Random().Next(matches.Count)].Groups[1].Value}\"],\"creation_source\":\"comet\",\"name\":\"{name}\",\"page_referrer\":\"launch_point\",\"actor_id\":\"{account.C_UID}\",\"client_mutation_id\":\"2\"}}}}";
				//tạo page
				pr = new RequestParams
				{
					["av"] = _accountModel.UID,
					["dpr"] = "1",
					["fb_dtsg"] = fb_dtsg,
					["lsd"] = lsd,
					["jazoest"] = jazoest,
					["fb_api_caller_class"] = "RelayModern",
					["fb_api_req_friendly_name"] = "AdditionalProfilePlusCreationMutation",
					["variables"] = $"{{\"input\":{{\"bio\":\"\",\"categories\":[\"2347428775505624\"],\"creation_source\":\"comet\",\"name\":\"{name}\",\"page_referrer\":\"launch_point\",\"actor_id\":\"{_accountModel.UID}\",\"client_mutation_id\":\"2\"}}}}",
					//["variables"] = $"{{\"input\":{{\"bio\":\"\",\"categories\":[\"{matches[new Random().Next(matches.Count)].Groups[1].Value}\"],\"creation_source\":\"comet\",\"name\":\"{name}\",\"page_referrer\":\"launch_point\",\"actor_id\":\"{account.C_UID}\",\"client_mutation_id\":\"2\"}}}}",
					["server_timestamps"] = "true",
					["doc_id"] = "5296879960418435",
					["__user"] = _accountModel.UID,
					["__a"] = "1",
					["__req"] = "1",
					["__hs"] = "19650.HYP:comet_pkg.2.1..2.1",
					["__ccg"] = "EXCELLENT",
					["__rev"] = "1009401453",
					["__s"] = "h6yd5k:jdfbm7:p7dga8",
					["__hsi"] = "7292578806040008403",
					["__dyn"] = "7AzHK4HzE4e5Q1ryaxG4VuC2-m1xDwAxu13wFwhUngS3q5UObwNwnof8boG0x8bo6u3y4o2vyE3Qwb-q7oc81xoswIK1Rwwwg8a8465o-cw5Mx62G5Usw9m1YwBgK7o884y0Mo4G1hx-3m1mzXw8W58jwGzE8FU5e7oqBwJK2W5olwUwOzEjUlDw-wUwxwjFovUy2a0SEuBwFKq2-azqwqo4i223908O321LwTwNxe6Uak1xwJwxyo6J0qo4e16wWw-w",
					["__csr"] = "gtmwyD6jYQmDnbaLOlEG95Ek_9nONaIIPGCB8IlmKz94_f-GEhhdDhKrlpdvKJXFkiWBBhmh93eEN7GeAXdQU-tfy5G9AKqZ7BhbyprBAAG9zSiaGnzolABz9GDykmaDGmjy9ESHAghx28Dxem2Py-diCGfDwwzoWmu4EK5V89UnG9hedy8f8jxyifyVU46ey8mxe58gwTx-228GAcyaxa3e1iK5898vwLyUO2O9DwioiwhUbGxS48ee2am367U5ycwSwLwmE5C1qyoaU8oC6o2OU6W0lS0q21vw2181K209u0g60KU9cw0JHw0lWo08mE02qXw2TXx24IMtzU0PO0sS0iO3a0brw3x87K17w0Gvw2UU0iBx20jq2Cbw1Dq1Rw0Tqw2Go3vw2uE1ez7wKw8K0k6qm0ZA1RU882Hw72x3g",
					["__comet_req"] = "15",
					["__aaid"] = "0",
					["__spin_r"] = "1009401453",
					["__spin_b"] = "trunk",
					["__spin_t"] = "1697935817"
				};

				try
				{
					FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                        Viewport-Width: 1497
                                        DNT: 1
                                        sec-ch-ua-mobile: ?0
                                        X-FB-Friendly-Name: AdditionalProfilePlusCreationMutation
                                        X-FB-LSD: {lsd}
                                        Content-Type: application/x-www-form-urlencoded
                                        X-ASBD-ID: 129477
                                        dpr: 1
                                        sec-ch-ua-platform: ""Windows""
                                        Accept: */*
                                        Origin: https://www.facebook.com
                                        Sec-Fetch-Site: same-origin
                                        Sec-Fetch-Mode: cors
                                        Sec-Fetch-Dest: empty
                                        Referer: https://www.facebook.com/pages/creation/?ref_type=launch_point
                                        Accept-Language: en-US,en;q=0.9");

					body = rq.Post("https://www.facebook.com/api/graphql/", pr).ToString();
				}
				catch { }

				if (body.Contains("\"name_error\":\""))
				{
					return ResultModel.NameError;
				}

				id_page = RegexHelper.GetValueFromGroup("additional_profile\":{\"id\":\"(.*?)\"}", body);
				if (id_page == "")
					return ResultModel.Fail;
				//up avt
				if (link != "")
				{
				UpAvatar:
					try
					{
						Leaf.xNet.MultipartContent data = new Leaf.xNet.MultipartContent()
				{
					{new FileContent(Path.GetFullPath(link)), "file", new FileInfo(link).Name}
				};

						FunctionHelper.AddHeaderxNet(rq, $@"DNT: 1
                                        sec-ch-ua-mobile: ?0
                                        X-FB-LSD: {lsd}
                                        X-ASBD-ID: 129477
                                        dpr: 1
                                        sec-ch-prefers-color-scheme: dark
                                        sec-ch-ua-platform: ""Windows""
                                        Accept: */*
                                        Origin: https://www.facebook.com
                                        Sec-Fetch-Site: same-origin
                                        Sec-Fetch-Mode: cors
                                        Sec-Fetch-Dest: empty
                                        Referer: https://www.facebook.com/pages/creation/?ref_type=launch_point
                                        Accept-Language: en-US,en;q=0.9");

						body = rq.Post($"https://www.facebook.com/profile/picture/upload/?photo_source=57&profile_id={_accountModel.UID}&__user={_accountModel.UID}&__a=1&__req=q&__hs=19637.HYP:comet_pkg.2.1..2.1&dpr=1&__ccg=EXCELLENT&__rev=1009108056&__s=gl8rtx:7c2567:6xvqo3&__hsi=7287108301766858208&__dyn=7AzHK4HzE4e5Q1ryaxG4VuC2-m1xDwAxu13wFwhUngS3q5UObwNwnof8boG0x8bo6u3y4o2vyE3Qwb-q7oc81xoswIK1Rwwwg8a8465o-cw5Mx62G5Usw9m1YwBgK7o884y0Mo4G1hx-3m1mzXw8W58jwGzE8FU5e7oqBwJK2W5olwUwOzEjUlDw-wUwxwjFovUy2a0SEuBwFKq2-azqwqo4i223908O321LwTwNxe6Uak1xwJwxyo6J0qo4e16wWw&__csr=gJdON4h5tjNAhsrvsBqsBsCx6D94TWbbEBlb6ldPhfj9nYJJf4_j9Q8OSBejFbbDijQrWgGWTAiCWgGVLijLgCteaBnuimG-4etaqcyV9pkcFyrUnG8y-dyp4uFKqay2eKt2kA22exG8ykjGii4AWGUrU8Apeu4U-mEjyoiK2Cm9Bx-222PmEW4oKiUKm5o5e5HxmqbBwkEiG58yfBwOwko-aHGu5aAxe4Qi1xDwlUK2Om4VU4meyo4O6onw862uu2O2KEhwIzElAxO8wLwj8nwkES3a1awuEaU17UcpXg1sEfU2iwdC2m0c-wUwvk4o0LW05mo08CU6x00dm61RCCg0EF01hi0x81540pq0pS0axwbWcw0Epw9C02da58C1-w1xe0qu02W-04DA04aE1bk1Zw4kU1ao19x5pU8841w&__comet_req=15&fb_dtsg={fb_dtsg}&jazoest={jazoest}&lsd={lsd}&__aaid=0&__spin_r=1009108056&__spin_b=trunk&__spin_t=1696662115", data).ToString();
					}
					catch { }

					fbid = RegexHelper.GetValueFromGroup("\"fbid\":\"(.*?)\"", body);
					if (fbid == "")
						//return ResultModel.Fail;

						pr = new RequestParams
						{
							["av"] = id_page,
							["dpr"] = "1",
							["fb_dtsg"] = fb_dtsg,
							["lsd"] = lsd,
							["jazoest"] = jazoest,
							["fb_api_caller_class"] = "RelayModern",
							["fb_api_req_friendly_name"] = "AdditionalProfilePlusEditMutation",
							["variables"] = $"{{\"input\":{{\"additional_profile_plus_id\":\"{id_page}\",\"creation_source\":\"comet\",\"profile_photo\":{{\"existing_photo_id\":\"{fbid}\"}},\"cover_photo\":null,\"actor_id\":\"{id_page}\",\"client_mutation_id\":\"2\"}}}}",
							["server_timestamps"] = "true",
							["doc_id"] = "6470849629597825",
						};

					try
					{
						FunctionHelper.AddHeaderxNet(rq, $@"DNT: 1
                                        sec-ch-ua-mobile: ?0
                                        X-FB-Friendly-Name: AdditionalProfilePlusEditMutation
                                        X-FB-LSD: {lsd}
                                        Content-Type: application/x-www-form-urlencoded
                                        X-ASBD-ID: 129477
                                        dpr: 1
                                        sec-ch-prefers-color-scheme: dark
                                        sec-ch-ua-platform: ""Windows""
                                        Accept: */*
                                        Origin: https://www.facebook.com
                                        Sec-Fetch-Site: same-origin
                                        Sec-Fetch-Mode: cors
                                        Sec-Fetch-Dest: empty
                                        Referer: https://www.facebook.com/pages/creation/?ref_type=launch_point
                                        Accept-Language: en-US,en;q=0.9");

						body = rq.Post("https://www.facebook.com/api/graphql/", pr).ToString();
					}
					catch { }

					//Thread.Sleep(5000);
					//if (!FunctionHelper.CheckAvatarUploaded(id_page))
					//    goto UpAvatar;
				}
			}
			if (id_page == "")
				return ResultModel.Fail;
			return ResultModel.Success;
		}
		public async Task<ResultModel> CheckLiveUid()
		{
			_accountModel.Status = "Đang check live uid ...";

			try
			{
				var options = new RestClientOptions("https://graph.facebook.com")
				{
					UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36",
					MaxTimeout = -1,
					Proxy = FunctionHelper.ParseProxyRestSharp(_accountModel.Proxy),
				};
				var client = new RestClient(options);
				var request = new RestRequest($"/{_accountModel.UID}/picture?width=500&access_token=6628568379|c1e620fa708a1d5696fb991c1bde5662&redirect=false", Method.Get);
				RestResponse response =  client.ExecuteAsync(request).Result;
				if (response.Content == null)
				{
					_accountModel.Status = "Check Uid Body Null";
					return ResultModel.Fail;
				}

				if (response.Content.Contains("368"))
				{
					_accountModel.Status = "Check Uid bị Spam Ip";
					return ResultModel.Fail;
				}

				if (!response.Content.Contains("error") && !response.Content.Contains(".gif"))
				{
					_accountModel.Status = "Uid Live";
					return ResultModel.Success;
				}
				else
				{
					_accountModel.Status = "Uid Die";
					return ResultModel.Fail;
				}
			}
			catch
			{
				//
			}
			_accountModel.Status = "Check Uid Error";
			return ResultModel.Fail;
		}


		public async Task<ResultModel> CheckLiveCookie()
		{
			_accountModel.Status = "Đang check live cookie ...";
			var refer = "";
			using (var rq = new HttpRequest())
			{
				rq.UserAgent = _accountModel.UserAgent;
				rq.KeepAlive = true;
				rq.AllowAutoRedirect = true;
				rq.Cookies = new CookieStorage();
				FunctionHelper.SetCookieToRequestXnet(rq, _accountModel.Cookie);
				rq.Proxy = FunctionHelper.ConvertToProxyClient(_accountModel.Proxy);

				FunctionHelper.AddHeaderxNet(rq, @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
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
					refer = rq.Get("https://www.facebook.com/settings").Address.AbsoluteUri;
				}
				catch
				{
					try
					{
						refer = rq.Response.Address.AbsoluteUri;
					}
					catch
					{
						//
					}
				}

				if (refer.Contains("facebook.com/settings"))
				{
					refer = null;
					_accountModel.Status = "Cookie Live";
					return ResultModel.Success;
				}

				if (refer.Contains("facebook.com/login") || refer.Contains("facebook.com/index"))
				{
					refer = null;
					_accountModel.Status = "Cookie Out";
					return ResultModel.Fail;
				}

				if (refer.Contains("956/") || refer.Contains("facebook.com/nt/screen"))
				{
					refer = null;
					_accountModel.Status = "Cookie CP 956";
					return ResultModel.Fail;
				}

				if (refer.Contains("282/"))
				{
					refer = null;
					_accountModel.Status = "Cookie CP 282";
					return ResultModel.Fail;
				}

				if (refer.Contains("checkpoint"))
				{
					var body = "";
					FunctionHelper.AddHeaderxNet(rq, @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
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
						body = rq.Get("https://mbasic.facebook.com/settings").ToString();
					}
					catch
					{
						try
						{
							body = rq.Response.ToString();
						}
						catch
						{
							//
						}
					}

					var stt = "";
					var content = RegexHelper.GetValueFromGroup("<div class=\"k\">(.*?)<\\/div><\\/section>", body);
					var matches = Regex.Matches(content, "<div>");
					if (matches.Count > 1)
					{
						stt += $"Cookie checkpoint {matches.Count} dòng";
					}
					else
					{
						stt += "Cookie checkpoint";
					}

					refer = null;
					body = null;
					_accountModel.Status = stt;
					return ResultModel.Fail;
				}
			}

			refer = null;
			_accountModel.Status = "Cookie Die";
			return ResultModel.Fail;
		}

		
	}
}
