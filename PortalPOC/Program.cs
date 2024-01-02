using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;

using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using PortalPOC;
using PortalPOC.Models;
using PortalPOC.Services;
using PortalPOC.Helpers;
using PortalPOC.QueryFactory;
using System.Reflection;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using PortalPOC.Controllers;

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

        builder.Services.AddScoped<IDataService, DataService>();
        builder.Services.AddScoped<IModelTypeMappingService, ModelTypeMappingService>();
        builder.Services.AddScoped<IDataTableRequestExtractor, DataTableRequestExtractor>();
        builder.Services.AddScoped<IQueryFactory, QueryFactory>();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        var app = builder.Build();

        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;



        // Perform warm-up activities
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<QuavisQorchAdminEasyTestContext>();
                WarmUpEntity<KappRole>(dbContext);

                Console.WriteLine("Warming Up Done !!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during warm-up.");
                Console.WriteLine(ex.Message);
            }
        }

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
    private static void WarmUpEntity<TEntity>(QuavisQorchAdminEasyTestContext dbContext) where TEntity : class
    {
        // Use LINQ dynamic to perform a query on the DbSet<TEntity>
        var query = dbContext.Set<TEntity>().AsQueryable();

        // Example: Retrieve the first record
        var firstRecord = query.FirstOrDefault();

        var filteredData = query.Where("Gcrecord == 1").ToList();

        // You can perform additional operations on the first record or customize the query as needed
        // For example: 
    }
    private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        // Handle the assembly load event
        Console.WriteLine($"Assembly loaded: {args.LoadedAssembly.FullName}");
        // Add your logic to process the loaded assembly
    }





}