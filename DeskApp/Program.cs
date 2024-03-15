using DeskApp;
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

        Log.Information("Desk App is starting... " + DateTime.Now.ToString());


        var host = CreateHostBuilder(args).Build();
        host.Run();


    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
       Host.CreateDefaultBuilder(args)
         .UseWindowsService(options =>
         {
             options.ServiceName = "QLiteDeskApp";
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
           });
}


