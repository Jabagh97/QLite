using Autofac.Extensions.DependencyInjection;
using KioskApp;
using Quavis.QorchLite.Hwlib;
using Serilog;
using QLite.Data.CommonContext;
using System.Reflection;
using KioskApp.Services;
using Microsoft.Extensions.Configuration;

public class Program
{
    static CancellationTokenSource cts;

    public static void Exit()
    {
        cts?.Cancel();
    }

    private static async Task Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args)
                .Build();

            CommonCtx.Container = host.Services.GetAutofacRoot();

            ConfigureLogger();

            Log.Information("Kiosk App is starting... " + DateTime.Now.ToString());
            cts = new CancellationTokenSource();
            var t = host.RunAsync(cts.Token);

            InitializeKioskHardware(host);

            var chromeUtil = host.Services.GetRequiredService<BrowserUtil>();
            var configuration = host.Services.GetRequiredService<IConfiguration>();

            string url = configuration.GetValue<string>("KioskAppURl") ?? "http://localhost:5144";
            chromeUtil.StartChrome(url);

            await t;
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

    private static void InitializeKioskHardware(IHost host)
    {
        var configuration = host.Services.GetRequiredService<IConfiguration>();
        CommonCtx.KioskHwId = configuration.GetValue<string>("KioskID");

        var hardwareManager = host.Services.GetRequiredService<HwManager>();
        hardwareManager.InitHardware();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseStaticWebAssets();
                webBuilder.UseContentRoot(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                webBuilder.UseKestrel();
                webBuilder.UseIIS();
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var ipAddress = config["KioskAppURl"];
                webBuilder.UseUrls(ipAddress);

            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                config.SetBasePath(assemblyLocation);
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
}