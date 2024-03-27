using Microsoft.AspNetCore.Hosting;
using QLiteDataApi;
using Serilog;
using Serilog.Events;
using System.ServiceProcess;

internal class Program
{
    private static IHost _host;

    public static async Task Main(string[] args)
    {
        Log.Information("API is starting... " + DateTime.Now.ToString());

        try
        {
            _host = CreateHostBuilder(args).Build();
            await _host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService(options =>
            {
                options.ServiceName = "QLiteAPI";
            })
           .UseSerilog((hostingContext, loggerConfiguration) =>
           {
               var baseDir = AppDomain.CurrentDomain.BaseDirectory;
               var logPath = Path.Combine(baseDir, "Logs", "log-.txt");

               loggerConfiguration
                   .WriteTo.File(
                       path: logPath,
                       rollingInterval: RollingInterval.Day,
                       retainedFileCountLimit: 10)
                   .MinimumLevel.Information()
                   .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                   .Enrich.FromLogContext();
           })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseStaticWebAssets();
                webBuilder.UseWebRoot(@"wwwroot");
                webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                webBuilder.UseKestrel();

                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"))
                    .Build();
                var ipAddress = config["SiteDomain"];
                webBuilder.UseUrls(ipAddress);
            });
}