using QLiteDataApi.QueryFactory;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using QLiteDataApi.Services;
using QLite.Data;
using QLite.Data.Services;
using QLiteDataApi.Context;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Correct indentation for AddDbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Add services to the container.
        builder.Services.AddScoped<IDataService, DataService>();
        builder.Services.AddScoped<IModelTypeMappingService, ModelTypeMappingService>();
        builder.Services.AddScoped<IQueryFactory, QueryFactory>();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();


        // Perform warm-up activities
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                WarmUpEntity<KappRole>(dbContext);

                Console.WriteLine("Warming Up Done !!!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during warm-up.");
                Console.WriteLine(ex.Message);
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void WarmUpEntity<TEntity>(ApplicationDbContext dbContext) where TEntity : class
    {
        // Use LINQ dynamic to perform a query on the DbSet<TEntity>
        var query = dbContext.Set<TEntity>().AsQueryable();

        // Example: Retrieve the first record
        var firstRecord = query.FirstOrDefault();

        var filteredData = query.Where("Gcrecord == 1").ToList();

        // You can perform additional operations on the first record or customize the query as needed
        // For example: 
    }
}