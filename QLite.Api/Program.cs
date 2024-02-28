using Autofac.Core;
using Microsoft.EntityFrameworkCore;
using QLite.Data;
using QLite.Data.Models.Auth;
using QLite.Data.Services;
using QLiteDataApi;
using QLiteDataApi.Context;
using QLiteDataApi.QueryFactory;
using QLiteDataApi.Services;
using QLiteDataApi.SignalR;
using System.Linq;
using System.Linq.Dynamic.Core;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IDataService, DataService>();
        builder.Services.AddScoped<IModelTypeMappingService, ModelTypeMappingService>();
        builder.Services.AddScoped<IQueryFactory, QueryFactory>();

        builder.Services.AddScoped<IKioskService, KioskService>();
        builder.Services.AddScoped<IDeskService, DeskService>();


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();

     
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", builder =>
            {
                builder.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting(); 

        app.UseAuthorization();

        app.UseCors("AllowSpecificOrigin"); 

        app.MapControllers();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<CommunicationHub>("/communicationHub");
        });


        //Call Site Cache Warmup 
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                // Use LINQ dynamic to perform a query on the DbSet<TEntity>
                var query = dbContext.Set<Country>().AsQueryable();

                // Example: Retrieve the first record
                var firstRecord = query.FirstOrDefault();

                var filteredData = query.Where("Gcrecord == 1").ToList();

                Console.WriteLine("Warming Up Done !!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during warm-up.");
                Console.WriteLine(ex.Message);
            }
        }
        app.Run();

        var siteDomain = builder.Configuration["SiteDomain"];

        // app.Run(siteDomain);
    }

    private static void WarmUpEntity<TEntity>(ApplicationDbContext dbContext) where TEntity : class
    {
        // Use LINQ dynamic to perform a query on the DbSet<TEntity>
        var query = dbContext.Set<TEntity>().AsQueryable();

        // Example: Retrieve the first record
        var firstRecord = query.FirstOrDefault();

        var filteredData = query.Where("Gcrecord == 1").ToList();

       
    }
}
