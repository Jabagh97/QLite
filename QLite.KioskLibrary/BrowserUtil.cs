using Microsoft.Extensions.Configuration;
using Quavis.QorchLite.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace Quavis.QorchLite.Hwlib
{
    public class BrowserUtil
    {
        IConfiguration _Cfg;
        TimerUtil _tu;
        public BrowserUtil(IConfiguration config, TimerUtil tu)
        {
            _Cfg = config;
            _tu = tu;
        }

        public string GetMACAddress()
        {
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                var nic = nics.Where(x => x.OperationalStatus == OperationalStatus.Up && x.NetworkInterfaceType != NetworkInterfaceType.Loopback).OrderByDescending(x => x.GetIPStatistics().BytesReceived).First();
                return nic?.GetPhysicalAddress().ToString();
            }
            catch (Exception ex)
            {
                LoggerAdapter.Error(ex, "error extracting active nic macAddress");
                throw;
            }
        }
        public Process StartChrome(string filePath = null, string userp = "")
        {
            if (filePath == null)
            {
                var qlhost = _Cfg.GetValue<string>("qlhost", "http://localhost:5000");
                if (!qlhost.EndsWith("/"))
                    qlhost += "/";

                filePath = $"{qlhost}KioskMvc?KioskId=" + CommonCtx.KioskHwId;
            }
            var kioskMode = _Cfg.GetValue<bool>("browseinkioskmode", true);

            string kioskCommand = kioskMode ? " --kiosk" : string.Empty;
            string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Chrome-UserData-" + userp);
            string disablePopups = "  --disable-features=InfiniteSessionRestore --disable-session-crashed-bubble --no-first-run --allow-insecure-localhost";

            //filePath = filePath.Replace("&", "^&");//"\"" + filePath + "\""
            filePath += kioskCommand + disablePopups + " --new-window --user-data-dir=" + dataDirectory;/*+ " --no-first-run(dont ask default browser) --suppress-message-center-popups --disable-features=Translate --fast --fast-start --unsafely-treat-insecure-origin-as-secure=" + filePath + " --allow-running-insecure-content --check-for-update-interval=604800 --extensions-update-frequency=604800 --new-window --window-position=0,0 --disable-notifications --enable-media-stream --no-default-browser-check --aggressive-cache-discard --disk-cache-size=1 --disable-cache --disk-cache-dir="+dataDirectory+" --media-cache-dir=" + dataDirectory + " --disable-gpu-program-cache --disable-gpu-shader-disk-cache --disable-lru-snapshot-cache --disable-application-cache --use-fake-ui-for-media-stream --ignore-certificate-errors --test-type --disable-infobars --disable-session-crashed-bubble --disable-features=InfiniteSessionRestore  --disable-translate --disable-gpu --disable-software-rasterizer --no-sandbox --disable-setuid-sandbox ";*/
            var chromePath = _Cfg.GetValue("chromePath", @"C:\Program Files\Google\Chrome\Application\chrome.exe");
            LoggerAdapter.Debug("Chrome will start with:" + filePath + "Chrome path:" + chromePath);
            var user = _Cfg.GetValue("linuxuser", "");
            Process Chrome = null;
            //if (CommonContext.runtime == "win")
            //{
            //    var ret = ProcessExtensions.StartProcessAsCurrentUserWin(null, chromePath + " " + filePath);
            //    if (!ret)
            //    {
            //        CommonContext.EmseLogger.Error("Chrome cannot started.");
            //        throw new CussException(PlatformErrorTypes.ChromeProcessCannotStarted, chromePath + " " + filePath);
            //    }
            //    Chrome = Process.GetProcessesByName("chrome").OrderByDescending(x => x.StartTime).FirstOrDefault();
            //}
            //else
            //{
            Chrome = new Process
            {
                StartInfo =
                {
                    FileName =  chromePath,
                    Arguments =  filePath,
                    UseShellExecute = false,
                    CreateNoWindow=false,
                    WindowStyle = ProcessWindowStyle.Maximized,
                }
            };
            if (!string.IsNullOrEmpty(user))
                Chrome.StartInfo.UserName = user;
            try
            {
                Chrome.Start();
            }
            catch (Exception ex)
            {
                LoggerAdapter.Error(ex, "Chrome cannot started." + ex.Message);
                throw;
            }
            //}


            LoggerAdapter.Debug("Chrome started. ProcessId:" + Chrome.Id);
            return Chrome;
        }

        public Task<bool> StopChrome()
        {

            var chromeList = Process.GetProcessesByName("chrome");
            foreach (var ch in chromeList)
            {
                ch.Kill();
            }

            return _tu.Repeat<bool>((st, ct) =>
            {
                var chromeList = Process.GetProcessesByName("chrome");
                return chromeList.Count() == 0;

            }, null, 200, 20, CancellationToken.None, waitOnStart: true);
        }

    }
}
