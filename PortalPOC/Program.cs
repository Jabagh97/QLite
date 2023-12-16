using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;

using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using PortalPOC;
using PortalPOC.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Correct indentation for AddDbContext
        builder.Services.AddDbContext<QuavisQorchAdminEasyTestContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        var app = builder.Build();

        // Initialize the database if InitDatabase is true
        var initDatabase = Convert.ToBoolean(app.Configuration["InitDatabase"]);
        if (initDatabase)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<QuavisQorchAdminEasyTestContext>();

                    // Apply any pending migrations to the database
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during database initialization
                    Console.WriteLine("An error occurred while initializing the database.");
                    Console.WriteLine(ex.Message);
                }
            }
        }


        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}