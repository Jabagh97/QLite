using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;
using Serilog;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.IdentityModel.Tokens.Jwt;
using QLiteAuthenticationServer.Context;
using Microsoft.EntityFrameworkCore;
using QLite.Data.Models.Auth;
using QLiteAuthenticationServer.Services;
using QLiteAuthenticationServer.Helpers;
using System.Security.Cryptography.X509Certificates;

namespace QLiteAuthenticationServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //  X509Certificate2 _certificate = Utils.GetCertificateFromStore(Configuration["certificateName"]);

            if (Environment.IsDevelopment())
                IdentityModelEventSource.ShowPII = true;

            services.AddControllersWithViews();

            services.AddSameSiteCookiePolicy();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            // data protection service
            services.AddDataProtection()
                .SetApplicationName("asdqwe")
                .PersistKeysToDbContext<ApplicationDbContext>()
                .AddKeyManagementOptions(o => o.AutoGenerateKeys = Configuration.GetValue<bool>("AutoGenerateKeys"));
            //.ProtectKeysWithCertificate(_certificate);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();



            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            })
                          .AddInMemoryIdentityResources(Config.IdentityResources)
                          .AddInMemoryApiScopes(Config.ApiScopes)
                          .AddInMemoryClients(Config.Clients(Configuration))
                          .AddAspNetIdentity<ApplicationUser>()
                          .AddProfileService<MyProfileService>();

            services.AddAuthentication("QuavisCookie")
                .AddCookie("QuavisCookie", options =>
                {
                    options.Cookie.Name = "QuavisCookie";
                    //options.Cookie.Expiration = TimeSpan.FromHours(5);
                    options.ExpireTimeSpan = TimeSpan.FromHours(5);
                    options.SessionStore = new MemoryCacheTicketStore();
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    options.Cookie.SameSite = SameSiteMode.Strict;

                });

            services.AddAuthorization(options =>
            {
                
            });


            builder.Services.AddHttpClient(); 

           

            #region AUTO MIGRATION
            var temp1 = services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();
            temp1.Database.Migrate();
            temp1.Dispose();
            #endregion


            //builder.AddSigningCredential(_certificate);

            builder.AddDeveloperSigningCredential(); // won't work in production. replace with `AddSigningCredential` when deploying!

            #region REGISTER AUTOMAPPER
            // not sure why `AddAutoMapper` doesn't work, but this works too.
            var temp2 = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapperConfig());
            });
            var mapper = temp2.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            services.AddSingleton<EmailSender>(); // email sender service

            services.AddSingleton<QuavisEncryptionService>(); // encryption & decrypion service



        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, QuavisEncryptionService encService)
        {

            app.UseCookiePolicy();

            if (Environment.IsDevelopment())
            {
                Log.Information("Development environment detected. Setting up developer exception page and enabling PII...");

                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                IdentityModelEventSource.ShowPII = true; // Enable showing PII in development
            }
            else
            {
                Log.Information("Production environment detected. Disabling developer exception page and PII, enabling global exception handler.");


                // Use custom exception handling in production
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var errorCode = Guid.NewGuid().ToString();
                            var exception = exceptionHandlerFeature.Error;

                            #region try to get current user
                            var userName = "Anonymous";
                            if (context != null && context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
                                userName = context.User.Identity.Name;
                            #endregion

                            Log.Information("---------------------------------------------------------------------------------------------------------------------------------------------------");
                            Log.Error(exception, "GLOBAL EXCEPTION HANDLED. Error Code: {ErrorCode}, User: {UserName}, Time: {Time}", errorCode, userName, DateTime.Now);

                            if (Configuration["Customer"] == "THY")
                            {
                                context.Response.Redirect("/AlappAuth/home/error?customErrorCode=" + errorCode);
                            }
                            else
                                context.Response.Redirect("/home/error?customErrorCode=" + errorCode);
                        }
                    });
                });

                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            // Check if loggerFactory is null
            if (loggerFactory == null)
            {
                throw new InvalidOperationException("LoggerFactory is not available.");
            }
            var logger = loggerFactory.CreateLogger("UtilsLogger");
            Utils.Initialize(encService);
        }
    }

}
