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

        public Startup(IConfiguration configuration)
        {
            CommonCtx.Config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Information("ConfigureServices Started");

            // Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(CommonCtx.Config.GetConnectionString("DefaultConnection")));

            // Scoped services
            RegisterScopedServices(services);

            // Controllers
            services.AddControllers();

            // Memory Cache
            services.AddMemoryCache(options => {
                options.SizeLimit = 1024; // Adjust based on tests
            });

            // API Documentation and Versioning
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Real-time communication
            services.AddSignalR();

            // Background services
            services.AddHostedService<DbBackupService>();

            // CORS Configuration
            ConfigureCors(services);

            // Additional configurations can be added here...
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            // Cache Warmup
            CacheWarmup(services);

            // Middleware configuration
            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

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

        private void RegisterScopedServices(IServiceCollection services)
        {
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IModelTypeMappingService, ModelTypeMappingService>();
            services.AddScoped<IQueryFactory, QLiteDataApi.QueryFactory.QueryFactory>();
            services.AddScoped<IKioskService, KioskService>();
            services.AddScoped<IDeskService, DeskService>();
        }

        private void ConfigureCors(IServiceCollection services)
        {
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

        private static void CacheWarmup(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                try
                {
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    var query = dbContext.Set<Country>().AsQueryable();
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
        }
    }
}
