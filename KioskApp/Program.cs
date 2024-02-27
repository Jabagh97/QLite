using Autofac.Extensions.DependencyInjection;
using KioskApp;
using Quavis.QorchLite.Hwlib;
using Serilog;
using QLite.Data.CommonContext;
using System.Reflection;

public class Program
{
    static CancellationTokenSource cts;

    public static void Exit()
    {
        cts?.Cancel();
    }

    private static void Main(string[] args)
    {
        try
        {
            if (args.Length > 0)
                CommonCtx.Env = args[0];
            var host = CreateHostBuilder(args)
                   .UseEnvironment(CommonCtx.Env)
                   .Build();

            CommonCtx.Container = host.Services.GetAutofacRoot();

            ConfigureLogger();

            Log.Information("Kiosk App is starting... " + DateTime.Now.ToString());
            cts = new CancellationTokenSource();
            var t = host.RunAsync(cts.Token);

            InitializeKioskHardware(host);
            t.Wait();
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


    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseStaticWebAssets();
                webBuilder.UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location));
                webBuilder.UseKestrel();
                webBuilder.UseIIS();

            })
             .ConfigureAppConfiguration(ConfigureAppConfig)
             .UseServiceProviderFactory(new AutofacServiceProviderFactory());

    }
    private static void ConfigureAppConfig(HostBuilderContext h, IConfigurationBuilder arg2)
    {
        var a = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        arg2.SetBasePath(a);
        //arg2.AddJsonFile("appsettings.json");
    }
}
