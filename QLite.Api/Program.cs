using Microsoft.AspNetCore.Hosting;
using QLiteDataApi;
using Serilog;
using Serilog.Events;
using System.ServiceProcess;

internal class Program
{

    private static IHost _host;

    private static void Main(string[] args)
    {

       

        Log.Information("API is starting... " + DateTime.Now.ToString());


        _host = Host.CreateDefaultBuilder(args)
        .UseWindowsService(options =>
        {
            options.ServiceName = "QLiteAPI";
        })
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .WriteTo.File(
                    path: "Logs/log-.txt",
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
            webBuilder.UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location));
            webBuilder.UseKestrel();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"))
                .Build();
            var ipAddress = config["SiteDomain"];
            webBuilder.UseUrls(ipAddress);
        }).Build();

        _host.Run();


    }
   

}
