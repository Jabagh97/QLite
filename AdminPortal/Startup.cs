using AdminPortal.Helpers;
using IdentityModel;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PortalPOC.Helpers;
using QLite.Data.Services;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace AdminPortal
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
           // services.AddScoped<IApiService, ApiService>();

            services.AddScoped<IModelTypeMappingService, ModelTypeMappingService>();

            services.AddScoped<IDataTableRequestExtractor, DataTableRequestExtractor>();



            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var openIdConnectConfig = Configuration.GetSection("OpenIdConnect");


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookies";
                options.DefaultChallengeScheme = "oidc";
            })
               .AddCookie("cookies", options =>
               {
                   options.Cookie.Name = "QuavisCookie";
                   options.ExpireTimeSpan = TimeSpan.FromHours(5);
                   options.SessionStore = new MemoryCacheTicketStore();

                   options.Cookie.HttpOnly = false;
                   options.Cookie.SecurePolicy = CookieSecurePolicy.None;

               })
               .AddOpenIdConnect("oidc", options =>
               {

                   options.NonceCookie.SecurePolicy = CookieSecurePolicy.None;
                   options.NonceCookie.HttpOnly = true;
                   options.NonceCookie.SameSite = SameSiteMode.Lax;

                   options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.None;
                   options.CorrelationCookie.HttpOnly = true;
                   options.CorrelationCookie.SameSite = SameSiteMode.Lax;



                   options.RequireHttpsMetadata = false;


                   options.Authority = openIdConnectConfig["Authority"];
                   options.ClientId = openIdConnectConfig["ClientId"];
                   options.ClientSecret = openIdConnectConfig["ClientSecret"];


                   //PostLogOutUri
                   options.SignedOutRedirectUri = Configuration["SiteDomain"];

                   /*
                   Least Privilige blog alinti:
                   Microsoft pre-populates the Scope collection on the OpenIdConnectOptions with the openid and the profile scope (don�t get me started). 
                   This means if you only want to request openid, you first need to clear the Scope collection and then add openid manually.
                   */
                   //options.Scope.Clear();
                   options.Scope.Add("openid");
                   //options.Scope.Add("profile");

                   options.MapInboundClaims = false;
                   options.GetClaimsFromUserInfoEndpoint = true;

                   options.SaveTokens = false; // you want to store tokens in the cookie

                   /*
                   OIDC spec says:
                   The Claims requested by the profile, email, address, and phone scope values are returned from the UserInfo Endpoint, as described in Section 5.3.2, when a response_type value is used that results in an Access Token being issued. However, when no Access Token is issued (which is the case for the response_type value id_token), the resulting Claims are returned in the ID Token.
                   */

                   // by adding id_token the authorization endpoint will return the user identification (claimtype sub). Here you must include in the scope the openid scope
                   options.ResponseType = "id_token";

                   options.TokenValidationParameters = new TokenValidationParameters // bu olmazsa `User.Name` null oluyor, onemli
                   {
                       NameClaimType = "name",
                       RoleClaimType = "role"
                   };

                   options.UsePkce = true; // onemli

                   options.SaveTokens = true;

                   options.CallbackPath = "/signin-oidc";
                   options.AccessDeniedPath = "/AccessDenied";
                   options.SignedOutCallbackPath = new PathString("/signout-callback-oidc");

                   options.Events.OnRedirectToIdentityProvider = context =>
                   {
                       // only modify requests to the authorization endpoint
                       if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Authentication)
                       {
                           // generate code_verifier
                           var codeVerifier = CryptoRandom.CreateUniqueId(32);

                           // store codeVerifier for later use
                           context.Properties.Items.Add("code_verifier", codeVerifier);

                           // create code_challenge
                           string codeChallenge;
                           using (var sha256 = SHA256.Create())
                           {
                               var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                               codeChallenge = Base64Url.Encode(challengeBytes);
                           }

                           // add code_challenge and code_challenge_method to request
                           context.ProtocolMessage.Parameters.Add("code_challenge", codeChallenge);
                           context.ProtocolMessage.Parameters.Add("code_challenge_method", "S256");
                       }

                       return Task.CompletedTask;
                   };

               });

            services.AddAuthorization(options =>
            {

            });

            services.AddControllersWithViews();

            services.AddHttpClient<ApiService>();



        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Environment.IsDevelopment())
            {
                Log.Information("Development environment detected. Setting up developer exception page and enabling PII...");

                // Show detailed exception information in development
                app.UseDeveloperExceptionPage();
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

                            context.Response.Redirect("/home/error?errorCode=" + errorCode);
                        }
                    });
                });

                app.UseHsts();
            }
            var provider = new FileExtensionContentTypeProvider(); // (to specify content type and charset for css responses)
            provider.Mappings[".css"] = "text/css; charset=UTF-8";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

}
