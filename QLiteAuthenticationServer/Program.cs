using QLiteAuthenticationServer;
using QLiteAuthenticationServer.Services;
using Serilog;
using Serilog.Events;
public class Program
{
    public static int Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"))
                .Build();
        var logPath = configuration["LogsPath"] ?? "Logs/log-.txt";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10)
            .CreateLogger();

        try
        {
            args = args.Except(new[] { "/seed" }).ToArray();

            var host = CreateHostBuilder(args, configuration).Build();

            #region SEED DATA
            Log.Information("Seeding database...");
            var config = host.Services.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("DefaultConnection");
            SeedData.EnsureSeedData(connectionString);
            Log.Information("Done seeding database.");
            #endregion

            Log.Information("Starting host...");
            host.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly.");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
        Host.CreateDefaultBuilder(args)
           .UseWindowsService(options =>
           {
               options.ServiceName = "QLiteAuthServer";
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
