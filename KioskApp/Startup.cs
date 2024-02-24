using Autofac;
using KioskApp.SignalR;
using Quavis.QorchLite.Hwlib.Printer;
using Quavis.QorchLite.Hwlib;
using System.Text.Json.Serialization;
using Autofac.Core;
using QLite.Data.Services;
using Serilog;
using Microsoft.IdentityModel.Logging;
using QLite.Data.CommonContext;
using QLite.Kio;
using QLite.Data;

namespace KioskApp
{
    public class Startup
    {


        public Startup(IConfiguration configuration)
        {
            CommonCtx.Config = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(20);
            });

            services.AddSession();

            services.AddControllersWithViews();
            services.AddHttpClient();

            services.AddScoped<IApiService, ApiService>();

        


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                Log.Information("Development environment detected. Setting up developer exception page and enabling PII...");

                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true; // Enable showing PII in development



            }
            else
            {
                app.UseExceptionHandler("/Error");
            }




            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors("cors_policy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<KioskHub>("/kioskHub");
            });
        }




        //public void ConfigureContainer(ContainerBuilder builder)
        //{

        //    builder.RegisterType<HwManager>().AsSelf().SingleInstance();
        //    builder.RegisterType<EmseUsbPrinterDevice>().AsSelf().SingleInstance();
        //    builder.RegisterType<EmsePrinter>().As<IPrinter>().SingleInstance();

        //}

    }

}
