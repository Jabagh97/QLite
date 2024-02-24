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


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", builder =>
            {
                builder.WithOrigins("http://localhost:7227", "http://localhost:5144") // Replace with your actual origin
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

        app.UseRouting(); // Add this line to use routing

        app.UseAuthorization();

        app.UseCors("AllowSpecificOrigin"); // Apply CORS policy

        app.MapControllers();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<CommunicationHub>("/communicationHub");
        });

        app.Run();
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
