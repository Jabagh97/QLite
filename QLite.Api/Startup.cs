using Autofac.Core;
using Microsoft.EntityFrameworkCore;
using QLite.Data;
using QLite.Data.CommonContext;
using QLite.Data.Services;
using QLiteDataApi.Context;
using QLiteDataApi.QueryFactory;
using QLiteDataApi.Services;
using QLiteDataApi.SignalR;
using Serilog;
using System.Configuration;
using System.Linq.Dynamic.Core;

namespace QLiteDataApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            CommonCtx.Config = configuration;

        }

        public void ConfigureServices(IServiceCollection services)
        {

            Log.Information("ConfigureServices Started");

            // Add services to the container.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(CommonCtx.Config.GetConnectionString("DefaultConnection")));


            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IModelTypeMappingService, ModelTypeMappingService>();
            services.AddScoped<IQueryFactory,QLiteDataApi.QueryFactory.QueryFactory>();

            services.AddScoped<IKioskService, KioskService>();
            services.AddScoped<IDeskService, DeskService>();


            services.AddControllers();
            services.AddMemoryCache();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSignalR();
            services.AddHostedService<DbBackupService>();


            var allowedOrigins = CommonCtx.Config.GetSection("Cors:AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            // Cache Warmup
            using (var scope = services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    // Use LINQ dynamic to perform a query on the DbSet<TEntity>
                    var query = dbContext.Set<Country>().AsQueryable();

                    // Example: Retrieve the first record
                    var firstRecord = query.FirstOrDefault();

                    var filteredData = query.Where("Gcrecord == 1").ToList();

                    Console.WriteLine("Cache Warmup Done !!!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred during cache warm-up.");
                    Console.WriteLine(ex.Message);
                }
            }


            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            //app.MapControllers();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CommunicationHub>("/communicationHub");
            });
        }


    }
}
