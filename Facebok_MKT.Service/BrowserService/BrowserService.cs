using AutoMapper;
using Facebok_MKT.Service.DataService.Accounts;
using Faceebook_MKT.Domain.AnditectBrowserController;
using Faceebook_MKT.Domain.GPMLoginAPIController;
using Faceebook_MKT.Domain.Helpers;
using Faceebook_MKT.Domain.Models;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebok_MKT.Service.BrowserService
{
	public class BrowserService
    {
        private readonly IAccountDataService _accountdataService;
        ChromeDriver driver;
        AccountModel accountModel;
        //public string createdProfileId;
        private GPMLoginAPI api;
        private NstBrowserAPI NstBrowserAPI;
        public BrowserService(AccountModel accountModel,
			IAccountDataService accountdataService)
        {
			_accountdataService = accountdataService;
            this.accountModel = accountModel;
        }
        public async Task<ChromeDriver> OpenChromeGpm(string apiGpm,
                                        string createdProfileId, string name,
                                        string useragent = "", double scale = 0.7,
                                        string proxy = "", bool hideBrowser = false,
                                        string position = "0,0")
        {
            //this.createdProfileId = "";

            api = new GPMLoginAPI(apiGpm);

        CreateProfile:
            if (string.IsNullOrEmpty(createdProfileId))
            {
                var createdResult = api.Create(name, isNoiseCanvas: true);

                if (createdResult != null)
                {
                    var status = Convert.ToBoolean(createdResult["status"]);
                    if (status)
                    {
                        createdProfileId = Convert.ToString(createdResult["profile_id"]);
                        accountModel.GPMID = createdProfileId;
                        await _accountdataService.Update(accountModel.AccountIDKey, accountModel);
                    }

                }

                //this.createdProfileId = createdProfileId;
                //Console.WriteLine("Created profile ID: " + createdProfileId);
            }

            //if (string.IsNullOrEmpty(position))
            //{
            //	position = GetNewPosition(800, 800, scale);
            //}

            var arg = $"--window-position={position} --window-size=800,800 --force-device-scale-factor={scale} --mute-audio --hide-crash-restore-bubble --disable-notifications --disable-popup-blocking";

            if (!string.IsNullOrEmpty(useragent))
            {
                arg += $" --user-agent=\"{useragent}\"";
            }

            if (hideBrowser)
            {
                arg += $" --headless";
            }
            if (!string.IsNullOrEmpty(proxy))
                api.UpdateProxy(createdProfileId, proxy.Replace("http://", ""));

            var startedResult = api.Start(createdProfileId, null, arg);

            
            var browserLocation = Convert.ToString(startedResult["browser_location"]);
            var seleniumRemoteDebugAddress = Convert.ToString(startedResult["selenium_remote_debug_address"]);
            var gpmDriverPath = Convert.ToString(startedResult["selenium_driver_location"]);

            //if (gpmDriverPath == "")
            //{
            //    //createdProfileId = "";
            //    //goto CreateProfile;
            //    return null;
            //}

            var gpmDriverFileInfo = new FileInfo(gpmDriverPath);

            var service = ChromeDriverService.CreateDefaultService(gpmDriverFileInfo.DirectoryName, gpmDriverFileInfo.Name);
            service.HideCommandPromptWindow = true;
            var options = new ChromeOptions
            {
                BinaryLocation = browserLocation,
                DebuggerAddress = seleniumRemoteDebugAddress
            };

            driver = new ChromeDriver(service, options);

            return driver;
        }

        public async Task<ChromeDriver> OpenChromeNST(string apiGpm,
                                        string createdProfileId, string name,
                                        string useragent = "", double scale = 0.7,
                                        string proxy = "", bool hideBrowser = false,
                                        string position = "0,0")
        {
            //this.createdProfileId = "";

            NstBrowserAPI = new NstBrowserAPI();

        CreateProfile:
            if (string.IsNullOrEmpty(createdProfileId))
            {
                var createdResult = NstBrowserAPI.CreateProfile(name);

            }

            //if (string.IsNullOrEmpty(position))
            //{
            //	position = GetNewPosition(800, 800, scale);
            //}

            var arg = $"--window-position={position} --window-size=800,800 --force-device-scale-factor={scale} ";

            if (!string.IsNullOrEmpty(useragent))
            {
                arg += $" --user-agent=\"{useragent}\"";
            }

            if (hideBrowser)
            {
                arg += $" --headless";
            }
            if (!string.IsNullOrEmpty(proxy))
                await NstBrowserAPI.UpdateProxy(createdProfileId, proxy.Replace("http://", ""));

            var port = await NstBrowserAPI.StartProfile("aa2a5268-7292-4043-af02-f45990690e87");

            //var browserLocation = Convert.ToString(startedResult["browser_location"]);
            var seleniumRemoteDebugAddress = $"127.0.0.1:{port}";
            var gpmDriverPath = Path.GetFullPath("gpmdriver.exe");


            if (!File.Exists(gpmDriverPath))
            {
                Console.WriteLine($"ChromeDriver not found at {gpmDriverPath}");
                return null;
            }
            var gpmDriverFileInfo = new FileInfo(gpmDriverPath);

            var service = ChromeDriverService.CreateDefaultService(gpmDriverFileInfo.DirectoryName, gpmDriverFileInfo.Name);
            service.HideCommandPromptWindow = true;

            var options = new ChromeOptions
            {
                //BinaryLocation = browserLocation,
                DebuggerAddress = seleniumRemoteDebugAddress
            };
            try
            {
                driver = new ChromeDriver(service, options);
            }
            catch
            {

            }


            return driver;
        }

        public ChromeDriver OpenChrome(string userAgent = "", double scale = 0.5, string position = "0,0")
        {
            //if (string.IsNullOrEmpty(position))
            //{
            //	position = BrowserPositionHelper.GetNewPosition(600, 800, scale);
            //}

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            var chromeOption = new ChromeOptions();
            chromeOption.AddArgument("--user-data-dir=" + Path.GetFullPath("profile/" + accountModel.Email1));

            if (accountModel.UserAgent != "")
            {
                chromeOption.AddArgument("--user-agent=" + accountModel.UserAgent);
            }
            if (userAgent != "")
                chromeOption.AddArgument($"--user-agent={userAgent}");
            chromeOption.AddArgument("--disable-web-security");
            chromeOption.AddArgument("--disable-rtc-smoothness-algorithm");
            chromeOption.AddArgument("--disable-webrtc-hw-decoding");
            chromeOption.AddArgument("--disable-webrtc-hw-encoding");
            chromeOption.AddArgument("--disable-webrtc-multiple-routes");
            chromeOption.AddArgument("--disable-webrtc-hw-vp8-encoding");
            chromeOption.AddArgument("--enforce-webrtc-ip-permission-check");
            chromeOption.AddArgument("--force-webrtc-ip-handling-policy");
            chromeOption.AddArgument("ignore-certificate-errors");
            chromeOption.AddArgument("disable-infobars");
            chromeOption.AddArgument("mute-audio");
            chromeOption.AddArgument("--disable-popup-blocking");
            chromeOption.AddArgument("--disable-plugins");
            chromeOption.AddArgument($"--force-device-scale-factor={scale}");
            chromeOption.AddArgument("--no-sandbox");
            chromeOption.AddArgument("--log-level=3");
            chromeOption.AddArgument("test-type=browser");
            chromeOption.AddExcludedArgument("enable-automation");
            chromeOption.AddUserProfilePreference("useAutomationExtension", false);
            chromeOption.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
            chromeOption.AddArgument("disable-blink-features=AutomationControlled");
            chromeOption.AddArgument("--window-size=600,800");
            chromeOption.AddArgument($"--window-position={position}");

            if (!string.IsNullOrEmpty(accountModel.Proxy))
            {
                chromeOption.AddArgument("--proxy-server=" + accountModel.Proxy);
            }

            driver = new ChromeDriver(chromeDriverService, chromeOption);
            driver.Manage().Timeouts().PageLoad = new TimeSpan(0, 10, 0);

            return driver;
        }

        public void CloseChrome()
        {
            try
            {
                var windowHandles = driver.WindowHandles;
                foreach (var handle in windowHandles)
                {
                    driver.SwitchTo().Window(handle);
                    driver.Close();
                }

                driver.Quit();
            }
            catch { }
            try
            {
                api.Stop(accountModel.GPMID);
            }
            catch
            {

            }
        }
    }
}
