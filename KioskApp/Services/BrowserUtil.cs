using QLite.Data.CommonContext;
using Serilog;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace KioskApp.Services
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
                Log.Error(ex, "error extracting active nic macAddress");
                throw;
            }
        }
        public Process StartChrome(string url)
        {
            // Path to the Chrome executable
            string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

            var kioskMode = _Cfg.GetValue("KioskMode", false);



            // Additional command line arguments for Chrome
            string additionalArguments;

            if (kioskMode)
            {
                additionalArguments = "--kiosk --disable-features=InfiniteSessionRestore --disable-session-crashed-bubble --no-first-run --allow-insecure-localhost";
            }
            else 
            {
                additionalArguments = " --disable-features=InfiniteSessionRestore --disable-session-crashed-bubble --no-first-run --allow-insecure-localhost";
            }

            // Command line arguments for Chrome
            string arguments = $"{additionalArguments} \"{url}\""; // Enclose the URL in double quotes

            try
            {
              

                Process chromeProcess = new Process
                {
                    StartInfo =
                {
                    FileName = chromePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow=false,
                }
                };

                chromeProcess.Start();
                return chromeProcess;
            }
            catch (Exception ex)
            {
                // Handle exceptions if Chrome cannot be started
                Console.WriteLine("Chrome cannot be started: " + ex.Message);
                return null;
            }
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
