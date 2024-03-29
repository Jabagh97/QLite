using Autofac.Extensions.DependencyInjection;
using KioskApp;
using Quavis.QorchLite.Hwlib;
using Serilog;
using QLite.Data.CommonContext;
using System.Reflection;
using KioskApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public class Program
{
    static CancellationTokenSource cts;

    public static void Exit()
    {
        cts?.Cancel();
    }

    public static void Main(string[] args)
    {
        try
        {
            if (args.Length > 0)
                KioskContext.Env = args[0];

            using (var host = CreateHostBuilder(args)
                .UseEnvironment(KioskContext.Env)
                .Build())
            {
                KioskContext.Container = host.Services.GetAutofacRoot();
                var configuration = host.Services.GetRequiredService<IConfiguration>();

                ConfigureLogger();

                Log.Information("Kiosk App is starting... " + DateTime.Now.ToString());
                cts = new CancellationTokenSource();
                var t = host.RunAsync(cts.Token);

                KioskContext.KioskHwId = configuration.GetValue<string>("KioskID");

                using (var scope = host.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var hardwareManager = serviceProvider.GetRequiredService<HwManager>();
                    hardwareManager.InitHardware();

                    var chromeUtil = serviceProvider.GetRequiredService<BrowserUtil>();

                    string url = configuration.GetValue<string>("KioskAppURl") ?? "http://localhost:5144";
                    chromeUtil.StartChrome(url);
                }

                t.Wait();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Kiosk App terminated unexpectedly");
        }
    }
    private static void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                path: "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10)
            .CreateLogger();
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseStaticWebAssets();
            webBuilder.UseContentRoot(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                config.SetBasePath(assemblyLocation);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            });
            webBuilder.ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var ipAddress = configuration.GetValue<string>("KioskAppURl") ?? "http://localhost:5144";
                webBuilder.UseUrls(ipAddress);
            });
        })
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());
}