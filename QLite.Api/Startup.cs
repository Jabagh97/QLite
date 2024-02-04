using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLite.Data.Models.Auth;
using QLite.Data.Services;
using QLiteDataApi.Context;
using QLiteDataApi.Helpers;
using QLiteDataApi.QueryFactory;
using QLiteDataApi.Services;
using System.Data;
using System.Reflection;

namespace QLiteDataApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["DefaultConnection"];
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // EF Core
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlite(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(migrationsAssembly);
                });
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                
            });

            // Identity

            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AuthDbContext>()
                    .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddAspNetIdentity<AppUser>()
                .AddConfigurationStore<AuthConfigurationDbContext>(options =>
                {
                    // options.DefaultSchema = "Identity";
                    options.ConfigureDbContext = b => b.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore<AuthPersistedGrantDbContext>(options =>
                {
                    // options.DefaultSchema = "Identity";
                    options.ConfigureDbContext = b => b.UseSqlite(connectionString, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                })
                .AddDeveloperSigningCredential();

            services.AddCors(confg =>
                confg.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

            services.AddControllersWithViews();

            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IModelTypeMappingService, ModelTypeMappingService>();

            services.AddScoped<IQueryFactory, QLiteDataApi.QueryFactory.QueryFactory>();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           DataSeed.SeedDatabase(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

}
