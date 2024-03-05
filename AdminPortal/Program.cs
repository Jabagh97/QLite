using AdminPortal;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        // logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                path: "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10)
            .CreateLogger();

        Log.Information("Portal is starting... " + DateTime.Now.ToString());


        var host = CreateHostBuilder(args).Build();
        host.Run();


    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
           // .UseSerilog()
           .ConfigureWebHostDefaults(webBuilder =>
           {
               webBuilder.UseStartup<Startup>();
               var config = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
               var ipAddress = config["SiteDomain"];
               webBuilder.UseUrls(ipAddress);
           });
}