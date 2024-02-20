using Autofac.Extensions.DependencyInjection;
using KioskApp;
using Quavis.QorchLite.Hwlib;
using Serilog;
using QLite.Data.CommonContext;

public class Program
{

   
    private static void Main(string[] args)
    {
        try
        {
            if (args.Length > 0)
                CommonCtx.Env = args[0];


            ConfigureLogger();

            Log.Information("Kiosk App is starting... " + DateTime.Now.ToString());

            var host = CreateHostBuilder(args).Build();

           // CommonCtx.Container = host.Services.GetAutofacRoot();

          


            InitializeKioskHardware(host);
            host.Run();
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

      //  var hardwareManager = host.Services.GetRequiredService<HwManager>();
      //  hardwareManager.InitHardware();
    }


    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseStaticWebAssets();
            });
    }
}
