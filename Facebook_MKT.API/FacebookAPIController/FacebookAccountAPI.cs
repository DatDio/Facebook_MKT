using AutoMapper;
using Facebook_MKT.API.Helpers;
using Facebook_MKT.Data.Entities;
using Facebook_MKT.Data.Services;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using Leaf.xNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Facebook_MKT.API.FacebookAPI
{
	public class FacebookAccountAPI : BaseFacebookAPI
	{

		public FacebookAccountAPI(AccountModel accountModel, 
			IDataService<Account> accountDataService,
			IMapper mapper) : base(accountModel, accountDataService, mapper)
		{
		}
	

		public bool GetToken()
		{
			string body = "";
			using (var rq = new Leaf.xNet.HttpRequest())
			{
				rq.AllowAutoRedirect = true;
				rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				//account.Cookie = "sb=zYiCYXmTV6Lo2j0SSdD4z-EX; datr=BFBXZS3zHKgU7vEYxxtr9uq0; wl_cbv=v2%3Bclient_version%3A2376%3Btimestamp%3A1702437833; ps_n=0; ps_l=0; dpr=1.309999942779541; wd=798x701; c_user=100088692310375; xs=7%3AlEmBBZtACWloxw%3A2%3A1708170569%3A-1%3A-1%3A%3AAcXEyuGMYqa1OCp2vb6LB_7IcqgD-Cs7vNN9J_m6nQ; fr=1WURFPTaHjsjWEuRB.AWVPrppDlyLBgBB1B1HnK4GEtRE.Bl0N1_..AAA.0.0.Bl0N1_.AWUUwD9vsCE";
				HttpsHelper.SetCookieToRequestXnet(rq, accountModel.Cookie);
				rq.Proxy = HttpsHelper.ConvertToProxyClient(accountModel.Proxy);

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
					account.C_Token = "EAA" + token;
					return true;
				}
			}
			return false;

		}

		
		public async ResultModel LoginWWW(AccountModel account)
		{
			string body = "", refer = "", url = "", jazoest = "", lsd = "", login_source = "", submit = "", fb_dtsg = "", nh = "", _js_datr = "";
			RequestParams pr = null;

			using (var rq = new Leaf.xNet.HttpRequest())
			{
				rq.Cookies = new CookieStorage();
				rq.AllowAutoRedirect = true;
				account.Proxy = "";
				rq.Proxy = HttpsHelper.ConvertToProxyClient(account.Proxy);
				rq.UserAgent = account.UserAgent;
				//load trang chủ
				try
				{
					HttpsHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
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

						HttpsHelper.AddHeaderxNet(rq, $@"accept: */*
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

						HttpsHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
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

									HttpsHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
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

								HttpsHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
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

								HttpsHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
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
					var accountEnity = _mapper.Map<Account>(_accountModel);
					 _accountDataService.Update(_accountModel.AccountIDKey, accountEnity);
					return ResultModel.CheckPoint956;
				}

				if (refer.Contains("282/"))
				{
					_accountModel.Status = "Login bị checkpoint 282";
					var accountEnity = _mapper.Map<Account>(_accountModel);
					_accountDataService.Update(_accountModel.AccountIDKey, accountEnity);
					return ResultModel.CheckPoint956;
				}

				if (refer.Contains("checkpoint"))
				{
					_accountModel.Status = "Login bị checkpoint ";
					var accountEnity = _mapper.Map<Account>(_accountModel);
					_accountDataService.Update(_accountModel.AccountIDKey, accountEnity);
					return ResultModel.CheckPoint956;
				}

				account.Cookie = rq.Cookies.GetCookieHeader("https://www.facebook.com/");
				if (!account.Cookie.Contains("c_user"))
				{
					_accountModel.Status = "Login thất bại";
					var accountEnity = _mapper.Map<Account>(_accountModel);
					_accountDataService.Update(_accountModel.AccountIDKey, accountEnity);
					return ResultModel.Fail;
				}
				_accountModel.Cookie=
				_accountModel.Status = "Login thành công!";
				var accountEnity = _mapper.Map<Account>(_accountModel);
				_accountDataService.Update(_accountModel.AccountIDKey, accountEnity);
				return ResultModel.Fail;
			}
			return ResultModel.Success;
		}
		public bool LoginUidPassMBasic(AccountModel account)
		{
			string body = "", refer = "", login = "", lsd = "", jazoest = "", m_ts = "", li = "", fb_dtsg = "", nh = "", submit = "";

			HttpsHelper.EditValueColumn(account, "C_Status", "Đang login uid pass ...", true);

			using (var rq = new Leaf.xNet.HttpRequest())
			{
				rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				rq.KeepAlive = true;
				rq.AllowAutoRedirect = true;
				rq.Cookies = new CookieStorage();
				rq.Proxy = HttpsHelper.ConvertToProxyClient(account.Proxy);

				//Load trang chủ mbasic
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
					body = rq.Get("https://mbasic.facebook.com/").ToString();
					refer = rq.Address.AbsoluteUri;
				}
				catch
				{
					HttpsHelper.EditValueColumn(account, "C_Status", "Load mbasic catch", true);
					return false;
				}

				// Điền uid pass và bấm login
				login = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("<input value=\"(.*?)\" type=\"submit\" name=\"login\"", body));
				if (login == "")
				{
					body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";
					HttpsHelper.EditValueColumn(account, "C_Status", "Không tìm thấy nút login", true);
					return false;
				}

				lsd = RegexHelper.GetValueFromGroup("name=\"lsd\" value=\"(.*?)\"", body);
				jazoest = RegexHelper.GetValueFromGroup("name=\"jazoest\" value=\"(.*?)\"", body);
				m_ts = RegexHelper.GetValueFromGroup("name=\"m_ts\" value=\"(.*?)\"", body);
				li = RegexHelper.GetValueFromGroup("name=\"li\" value=\"(.*?)\"", body);

				RequestParams pr = new RequestParams
				{
					["lsd"] = lsd,
					["jazoest"] = jazoest,
					["m_ts"] = m_ts,
					["li"] = li,
					["try_number"] = "0",
					["unrecognized_tries"] = "0",
					["email"] = account.C_UID,
					["pass"] = account.C_Password,
					["login"] = login,
					["bi_xrwh"] = "0"
				};

				try
				{
					HttpsHelper.AddHeaderxNet(rq, @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                    Accept-Language:en-US,en;q=0.9
                                                    sec-ch-ua-mobile:?0
                                                    sec-ch-ua-platform:\""Windows\""
                                                    Sec-Fetch-Dest:document
                                                    Sec-Fetch-Mode:navigate
                                                    Sec-Fetch-Site:none
                                                    Sec-Fetch-User:?1
                                                    Upgrade-Insecure-Requests:1");
					rq.Referer = refer;
					body = rq.Post("https://mbasic.facebook.com/login/device-based/regular/login/?refsrc=https%3A%2F%2Fmbasic.facebook.com%2F&lwv=100&refid=8", pr).ToString();
					refer = rq.Address.AbsoluteUri;
				}
				catch
				{
					body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

					HttpsHelper.EditValueColumn(account, "C_Status", "Bấm login mbasic catch", true);
					return false;
				}

				if (body.Contains("name=\"pass\""))
				{
					body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

					HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic sai pass", true);
					return false;
				}

				//Kiểm tra 2FA
				fb_dtsg = RegexHelper.GetValueFromGroup("name=\"fb_dtsg\" value=\"(.*?)\"", body);
				nh = RegexHelper.GetValueFromGroup("name=\"nh\" value=\"(.*?)\"", body);
				login = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("<input value=\"(.*?)\" type=\"submit\" name=\"submit\\[Submit Code\\]\"", body));

				if (login != "" && account.C_2FA != "")
				{
					pr = new RequestParams
					{
						["fb_dtsg"] = fb_dtsg,
						["jazoest"] = jazoest,
						["checkpoint_data"] = "",
						["approvals_code"] = HttpsHelper.ConvertTwoFA(account.C_2FA),
						["codes_submitted"] = "0",
						["submit[Submit Code]"] = login,
						["nh"] = nh
					};

					try
					{
						HttpsHelper.AddHeaderxNet(rq, @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                    Accept-Language:en-US,en;q=0.9
                                                    sec-ch-ua-mobile:?0
                                                    sec-ch-ua-platform:\""Windows\""
                                                    Sec-Fetch-Dest:document
                                                    Sec-Fetch-Mode:navigate
                                                    Sec-Fetch-Site:none
                                                    Sec-Fetch-User:?1
                                                    Upgrade-Insecure-Requests:1");
						rq.Referer = refer;
						body = rq.Post("https://mbasic.facebook.com/login/checkpoint/", pr).ToString();
						refer = rq.Address.AbsoluteUri;

						login = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("<input value=\"(.*?)\" type=\"submit\" name=\"submit\\[Continue\\]\"", body));
					}
					catch
					{
						//
					}

					//save device
					pr = new RequestParams
					{
						["fb_dtsg"] = fb_dtsg,
						["jazoest"] = jazoest,
						["checkpoint_data"] = "",
						["name_action_selected"] = "save_device",
						["submit[Continue]"] = login,
						["nh"] = nh
					};

					try
					{
						HttpsHelper.AddHeaderxNet(rq, @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                    Accept-Language:en-US,en;q=0.9
                                                    sec-ch-ua-mobile:?0
                                                    sec-ch-ua-platform:\""Windows\""
                                                    Sec-Fetch-Dest:document
                                                    Sec-Fetch-Mode:navigate
                                                    Sec-Fetch-Site:none
                                                    Sec-Fetch-User:?1
                                                    Upgrade-Insecure-Requests:1");
						rq.Referer = refer;
						body = rq.Post("https://mbasic.facebook.com/login/checkpoint/", pr).ToString();
						refer = rq.Address.AbsoluteUri;

						login = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("<input value=\"(.*?)\" type=\"submit\" name=\"submit\\[Continue\\]\"", body));
					}
					catch
					{
						//
					}

					if (login != "")
					{
						pr = new RequestParams
						{
							["fb_dtsg"] = fb_dtsg,
							["jazoest"] = jazoest,
							["checkpoint_data"] = "",
							["submit[Continue]"] = login,
							["nh"] = nh
						};

						try
						{
							HttpsHelper.AddHeaderxNet(rq, @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                    Accept-Language:en-US,en;q=0.9
                                                    sec-ch-ua-mobile:?0
                                                    sec-ch-ua-platform:\""Windows\""
                                                    Sec-Fetch-Dest:document
                                                    Sec-Fetch-Mode:navigate
                                                    Sec-Fetch-Site:none
                                                    Sec-Fetch-User:?1
                                                    Upgrade-Insecure-Requests:1");
							rq.Referer = refer;
							body = rq.Post("https://mbasic.facebook.com/login/checkpoint/", pr).ToString();
							refer = rq.Address.AbsoluteUri;

							login = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("id=\"checkpointSubmitButton\" class=\"(.*?)\"><input value=\"(.*?)\" type=\"submit\" name=\"submit\\[This was me\\]\"", body, 2));
						}
						catch
						{
							//
						}

						if (login != "")
						{
							pr = new RequestParams
							{
								["fb_dtsg"] = fb_dtsg,
								["jazoest"] = jazoest,
								["checkpoint_data"] = "",
								["submit[This was me]"] = login,
								["nh"] = nh
							};

							try
							{
								HttpsHelper.AddHeaderxNet(rq, @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                    Accept-Language:en-US,en;q=0.9
                                                    sec-ch-ua-mobile:?0
                                                    sec-ch-ua-platform:\""Windows\""
                                                    Sec-Fetch-Dest:document
                                                    Sec-Fetch-Mode:navigate
                                                    Sec-Fetch-Site:none
                                                    Sec-Fetch-User:?1
                                                    Upgrade-Insecure-Requests:1");
								rq.Referer = refer;
								body = rq.Post("https://mbasic.facebook.com/login/checkpoint/", pr).ToString();
								refer = rq.Address.AbsoluteUri;

								login = HttpUtility.HtmlDecode(RegexHelper.GetValueFromGroup("<input value=\"(.*?)\" type=\"submit\" name=\"submit\\[Continue\\]\"", body));
							}
							catch
							{
								//
							}

							if (login != "")
							{
								pr = new RequestParams
								{
									["fb_dtsg"] = fb_dtsg,
									["jazoest"] = jazoest,
									["checkpoint_data"] = "",
									["name_action_selected"] = "save_device",
									["submit[Continue]"] = login,
									["nh"] = nh
								};

								try
								{
									HttpsHelper.AddHeaderxNet(rq, @"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
                                                    Accept-Language:en-US,en;q=0.9
                                                    sec-ch-ua-mobile:?0
                                                    sec-ch-ua-platform:\""Windows\""
                                                    Sec-Fetch-Dest:document
                                                    Sec-Fetch-Mode:navigate
                                                    Sec-Fetch-Site:none
                                                    Sec-Fetch-User:?1
                                                    Upgrade-Insecure-Requests:1");
									rq.Referer = refer;
									body = rq.Post("https://mbasic.facebook.com/login/checkpoint/", pr).ToString();
									refer = rq.Address.AbsoluteUri;
								}
								catch
								{
									//
								}
							}
						}
					}
				}
				else if (login != "" && account.C_2FA == "")
				{
					body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

					HttpsHelper.EditValueColumn(account, "C_Status", "Login bị 2FA", true);
					return false;
				}

				// Lấy Cookie
				account.Cookie = rq.Cookies.GetCookieHeader("https://m.facebook.com/");
				HttpsHelper.EditValueColumn(account, "Cookie", account.Cookie, true);
				if (!account.Cookie.Contains("c_user"))
				{
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
						body = rq.Get("https://m.facebook.com/checkpoint/").ToString();
						refer = rq.Address.AbsoluteUri;
					}
					catch
					{
						body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

						HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic lỗi chưa xác định", true);
						return false;
					}
					submit = RegexHelper.GetValueFromGroup("button type=\"submit\" value=\"(.*?)\" class=\"(.*?)\" name=\"submit\\[Continue\\]\"", body);
					if (submit == "")
					{
						body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

						HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic checkpoint", true);
						return false;
					}
					fb_dtsg = RegexHelper.GetValueFromGroup("input type=\"hidden\" name=\"fb_dtsg\" value=\"(.*?)\"", body);
					jazoest = RegexHelper.GetValueFromGroup("input type=\"hidden\" name=\"jazoest\" value=\"(.*?)\"", body);
					nh = RegexHelper.GetValueFromGroup("input type=\"hidden\" name=\"nh\" value=\"(.*?)\"", body);
					pr = new RequestParams
					{
						["fb_dtsg"] = fb_dtsg,
						["jazoest"] = jazoest,
						["checkpoint_data"] = "",
						["submit[Continue]"] = submit,
						["nh"] = nh
					};
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
						rq.Referer = refer;
						body = rq.Post("https://m.facebook.com/checkpoint/", pr).ToString();
						refer = rq.Address.AbsoluteUri;
					}
					catch
					{
						body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

						HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic lỗi chưa xác định", true);
						return false;
					}
					submit = RegexHelper.GetValueFromGroup("button type=\"submit\" value=\"(.*?)\" class=\"(.*?)\" name=\"submit\\[Continue\\]\"", body);
					if (submit != "")
					{
						pr = new RequestParams
						{
							["fb_dtsg"] = fb_dtsg,
							["jazoest"] = jazoest,
							["checkpoint_data"] = "",
							["submit[Continue]"] = submit,
							["nh"] = nh
						};
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
							rq.Referer = refer;
							body = rq.Post("https://m.facebook.com/checkpoint/", pr).ToString();
							refer = rq.Address.AbsoluteUri;
						}
						catch
						{
							body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";
							HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic lỗi chưa xác định", true);
							return false;
						}
					}
					submit = RegexHelper.GetValueFromGroup("button type=\"submit\" value=\"(.*?)\" class=\"(.*?)\" name=\"submit\\[This Is Okay\\]\"", body);
					if (submit != "")
					{
						pr = new RequestParams
						{
							["fb_dtsg"] = fb_dtsg,
							["jazoest"] = jazoest,
							["checkpoint_data"] = "",
							["submit[This Is Okay]"] = submit,
							["nh"] = nh
						};
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
							rq.Referer = refer;
							body = rq.Post("https://m.facebook.com/checkpoint/", pr).ToString();
							refer = rq.Address.AbsoluteUri;
						}
						catch
						{
							body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";
							HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic lỗi chưa xác định", true);
							return false;
						}
						submit = RegexHelper.GetValueFromGroup("button type=\"submit\" value=\"(.*?)\" class=\"(.*?)\" name=\"submit\\[Continue\\]\"", body);
						pr = new RequestParams
						{
							["fb_dtsg"] = fb_dtsg,
							["jazoest"] = jazoest,
							["checkpoint_data"] = "",
							["name_action_selected"] = "save_device",
							["submit[Continue]"] = submit,
							["nh"] = nh
						};
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
							rq.Referer = refer;
							body = rq.Post("https://m.facebook.com/checkpoint/", pr).ToString();
							refer = rq.Address.AbsoluteUri;
						}
						catch
						{
							body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";
							HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic lỗi chưa xác định", true);
							return false;
						}
						account.Cookie = rq.Cookies.GetCookieHeader("https://m.facebook.com/");
						if (account.Cookie.Contains("c_user"))
						{
							body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";
							HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic thành công", true);
							return true;
						}
					}
					if (!body.Contains("password_new"))
					{
						body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

						HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic checkpoint", true);
						return false;
					}
					submit = RegexHelper.GetValueFromGroup("button type=\"submit\" value=\"(.*?)\" class=\"(.*?)\" name=\"submit\\[Continue\\]\"", body);
					string newpass = HttpsHelper.GenerateRandomString(12);
					pr = new RequestParams
					{
						["fb_dtsg"] = fb_dtsg,
						["jazoest"] = jazoest,
						["checkpoint_data"] = "",
						["password_new"] = newpass,
						["submit[Continue]"] = submit,
						["nh"] = nh
					};
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
						rq.Referer = refer;
						body = rq.Post("https://m.facebook.com/checkpoint/", pr).ToString();
						refer = rq.Address.AbsoluteUri;
					}
					catch
					{
						body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

						HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic lỗi chưa xác định", true);
						return false;
					}
					account.C_Password = newpass;
					HttpsHelper.EditValueColumn(account, "C_Pass", account.C_Password, true);
					submit = RegexHelper.GetValueFromGroup("id=\"checkpointSecondaryButton\"><button type=\"submit\" value=\"(.*?)\" class=\"(.*?)\" name=\"submit\\[Go to News Feed\\]\"", body);
					if (submit == "")
					{
						submit = RegexHelper.GetValueFromGroup("<button type=\"submit\" value=\"(.*?)\" class=\"(.*?)\" name=\"submit\\[Go to News Feed\\]\"", body);
						if (submit == "")
						{
							body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

							HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic checkpoint", true);
							return false;
						}
					}
					pr = new RequestParams
					{
						["fb_dtsg"] = fb_dtsg,
						["jazoest"] = jazoest,
						["checkpoint_data"] = "",
						["submit[Go to News Feed]"] = submit,
						["nh"] = nh
					};
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
						rq.Referer = refer;
						body = rq.Post("https://m.facebook.com/checkpoint/", pr).ToString();
						refer = rq.Address.AbsoluteUri;
					}
					catch
					{
						body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";
						HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic lỗi chưa xác định", true);
						return false;
					}

					account.Cookie = rq.Cookies.GetCookieHeader("https://m.facebook.com/");
					if (account.Cookie.Contains("c_user"))
					{
						body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

						HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic + đổi pass thành công", true);
						return true;
					}

					body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

					HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic checkpoint", true);
					return false;
				}

				HttpsHelper.EditValueColumn(account, "Cookie", account.Cookie, true);
				if (account.Cookie.Contains("c_user"))
				{
					body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

					HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic thành công", true);
					return true;
				}

				if (body.Contains("You’re Temporarily Blocked") || body.Contains("we temporarily locked it"))
				{
					body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

					HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic spam ip", true);
					return false;
				}

				body = ""; refer = ""; login = ""; lsd = ""; jazoest = ""; m_ts = ""; li = ""; fb_dtsg = ""; nh = ""; submit = "";

				HttpsHelper.EditValueColumn(account, "C_Status", "Login mbasic thất bại", true);
				return false;
			}
		}
		public ResultModel RegPage(AccountModel account, string name, string link)
		{
			string body = "", refer = "", jazoest = "", lsd = "", fb_dtsg = "", id_page = "", fbid = "";
			RequestParams pr = null;

			using (var rq = new Leaf.xNet.HttpRequest())
			{
				rq.Cookies = new CookieStorage();
				rq.AllowAutoRedirect = true;
				rq.Proxy = HttpsHelper.ConvertToProxyClient(account.Proxy);
				HttpsHelper.SetCookieToRequestXnet(rq, account.Cookie);
				rq.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
				//load trang chủ
				try
				{
					HttpsHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
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
					["av"] = account.C_UID,
					["dpr"] = "1",
					["fb_dtsg"] = fb_dtsg,
					["lsd"] = lsd,
					["jazoest"] = jazoest,
					["fb_api_caller_class"] = "RelayModern",
					["fb_api_req_friendly_name"] = "usePagesCometAdminEditingCategoryDataSourceQuery",
					["variables"] = $"{{\"params\":{{\"search_string\":\"{HttpsHelper.GenerateRandomStringOnly(1)}\"}}}}",
					["server_timestamps"] = "true",
					["doc_id"] = "6219626854831519"
				};

				try
				{
					HttpsHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
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
					["av"] = account.C_UID,
					["dpr"] = "1",
					["fb_dtsg"] = fb_dtsg,
					["lsd"] = lsd,
					["jazoest"] = jazoest,
					["fb_api_caller_class"] = "RelayModern",
					["fb_api_req_friendly_name"] = "AdditionalProfilePlusCreationMutation",
					["variables"] = $"{{\"input\":{{\"bio\":\"\",\"categories\":[\"2347428775505624\"],\"creation_source\":\"comet\",\"name\":\"{name}\",\"page_referrer\":\"launch_point\",\"actor_id\":\"{account.C_UID}\",\"client_mutation_id\":\"2\"}}}}",
					//["variables"] = $"{{\"input\":{{\"bio\":\"\",\"categories\":[\"{matches[new Random().Next(matches.Count)].Groups[1].Value}\"],\"creation_source\":\"comet\",\"name\":\"{name}\",\"page_referrer\":\"launch_point\",\"actor_id\":\"{account.C_UID}\",\"client_mutation_id\":\"2\"}}}}",
					["server_timestamps"] = "true",
					["doc_id"] = "5296879960418435",
					["__user"] = account.C_UID,
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
					HttpsHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
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

						HttpsHelper.AddHeaderxNet(rq, $@"DNT: 1
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

						body = rq.Post($"https://www.facebook.com/profile/picture/upload/?photo_source=57&profile_id={account.C_UID}&__user={account.C_UID}&__a=1&__req=q&__hs=19637.HYP:comet_pkg.2.1..2.1&dpr=1&__ccg=EXCELLENT&__rev=1009108056&__s=gl8rtx:7c2567:6xvqo3&__hsi=7287108301766858208&__dyn=7AzHK4HzE4e5Q1ryaxG4VuC2-m1xDwAxu13wFwhUngS3q5UObwNwnof8boG0x8bo6u3y4o2vyE3Qwb-q7oc81xoswIK1Rwwwg8a8465o-cw5Mx62G5Usw9m1YwBgK7o884y0Mo4G1hx-3m1mzXw8W58jwGzE8FU5e7oqBwJK2W5olwUwOzEjUlDw-wUwxwjFovUy2a0SEuBwFKq2-azqwqo4i223908O321LwTwNxe6Uak1xwJwxyo6J0qo4e16wWw&__csr=gJdON4h5tjNAhsrvsBqsBsCx6D94TWbbEBlb6ldPhfj9nYJJf4_j9Q8OSBejFbbDijQrWgGWTAiCWgGVLijLgCteaBnuimG-4etaqcyV9pkcFyrUnG8y-dyp4uFKqay2eKt2kA22exG8ykjGii4AWGUrU8Apeu4U-mEjyoiK2Cm9Bx-222PmEW4oKiUKm5o5e5HxmqbBwkEiG58yfBwOwko-aHGu5aAxe4Qi1xDwlUK2Om4VU4meyo4O6onw862uu2O2KEhwIzElAxO8wLwj8nwkES3a1awuEaU17UcpXg1sEfU2iwdC2m0c-wUwvk4o0LW05mo08CU6x00dm61RCCg0EF01hi0x81540pq0pS0axwbWcw0Epw9C02da58C1-w1xe0qu02W-04DA04aE1bk1Zw4kU1ao19x5pU8841w&__comet_req=15&fb_dtsg={fb_dtsg}&jazoest={jazoest}&lsd={lsd}&__aaid=0&__spin_r=1009108056&__spin_b=trunk&__spin_t=1696662115", data).ToString();
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
						HttpsHelper.AddHeaderxNet(rq, $@"DNT: 1
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
					//if (!HttpsHelper.CheckAvatarUploaded(id_page))
					//    goto UpAvatar;
				}
			}
			if (id_page == "")
				return ResultModel.Fail;
			return ResultModel.Success;
		}
	}
}
