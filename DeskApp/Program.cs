using DeskApp;
using Serilog;

internal class Program
{
    private static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"))
                 .Build();

        var logPath = configuration["LogsPath"] ?? "Logs/log-.txt";


        // logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10)
            .CreateLogger();

        Log.Information("Desk App is starting... " + DateTime.Now.ToString());


        var host = CreateHostBuilder(args, configuration).Build();
        host.Run();


    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
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
               var ipAddress = configuration["SiteDomain"];
               webBuilder.UseUrls(ipAddress);
           });
}


