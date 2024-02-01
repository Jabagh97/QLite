using Quavis.QorchLite.Hwlib;

internal class Program
{
    static CancellationTokenSource cts;

    public static void Exit()
    {
        cts?.Cancel();
    }

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register your services
        ConfigureServices(builder.Services);

        var app = builder.Build();

        Configure(app, builder.Environment);

        cts = new CancellationTokenSource();
        var t = app.RunAsync(cts.Token);

        // Additional code if needed

        t.Wait();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Add your services to the container
        services.AddControllersWithViews();


        // Add any other services you may need
    }

    private static void Configure(WebApplication app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline
        if (!env.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html");
    }
}
