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
using KioskApp.Services;
using QLite.KioskLibrary.Display;
using QLite.KioskLibrary.Hardware;

namespace KioskApp
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            KioskContext.Config = configuration;            
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
            services.AddMemoryCache();
            services.AddControllersWithViews();
            services.AddHttpClient<ApiService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            app.UseStaticFiles();
            app.UseRouting();

            //app.UseCors("cors_policy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=KioskAuth}/{action=Authenticate}");
                endpoints.MapHub<KioskHub>("/kioskHub");
            });
        }




        public void ConfigureContainer(ContainerBuilder builder)
        {

            builder.RegisterType<HwManager>().AsSelf().SingleInstance();
            builder.RegisterType<KioskHubContext>().As<IHwHubContext>().SingleInstance();
            builder.RegisterType<EmsePrinter>().As<IPrinter>().SingleInstance();

            builder.RegisterType<EmseUsbPrinterDevice>().AsSelf().SingleInstance();

            builder.RegisterType<EmseDisplayGHid>().Named<IDisplay>("sgm").SingleInstance();
            builder.RegisterType<EmseDisplayHDot>().Named<IDisplay>("dot").SingleInstance();
            builder.RegisterType<EmseUsbQueNumDevice>().AsSelf().SingleInstance();


            builder.RegisterType<TimerUtil>().AsSelf().SingleInstance();
            builder.RegisterType<BrowserUtil>().AsSelf().SingleInstance();


        }

    }

}
