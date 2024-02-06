using DeskApp;
using DeskApp.Helpers;
using IdentityModel;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

#region Add services to the container.



#region OAuth & OIDC

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var openIdConnectConfig = builder.Configuration.GetSection("OpenIdConnect");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("cookies", options =>
    {
        options.Cookie.Name = "QuavisCookie";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.SessionStore = new MemoryCacheTicketStore();
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = openIdConnectConfig["Authority"];
        options.ClientId = openIdConnectConfig["ClientId"];
        options.ClientSecret = openIdConnectConfig["ClientSecret"];

        //PostLogOutUri
        options.SignedOutRedirectUri = builder.Configuration["SiteDomain"];

        /*
        Least Privilige blog alinti:
        Microsoft pre-populates the Scope collection on the OpenIdConnectOptions with the openid and the profile scope (don’t get me started). 
        This means if you only want to request openid, you first need to clear the Scope collection and then add openid manually.
        */
        //options.Scope.Clear();
        options.Scope.Add("openid");
        //options.Scope.Add("profile");

        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;

        options.SaveTokens = false; // you want to store tokens in the cookie

        //For Development (http)
        //options.RequireHttpsMetadata = false;

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

builder.Services.AddAuthorization(options =>
{
    // `asBaykus` only
    options.AddPolicy("BaykusOnly", policy =>
        policy.RequireClaim("asBaykus"));

    // either `asBaykus` OR `asSuperUser`
    options.AddPolicy("SuperUserOnly", policy =>
        policy.RequireAssertion(context => context.User.HasClaim(c =>
            c.Type == "asSuperUser" || c.Type == "asBaykus"
            )));
});

#endregion

builder.Services.AddControllersWithViews();

var app = builder.Build();

#endregion

#region Configure the HTTP request pipeline.

// logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 10)
    .CreateLogger();

Log.Information("Server is starting... " + DateTime.Now.ToString());

if (app.Environment.IsDevelopment())
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

app.UseHttpsRedirection(); // all incoming HTTP requests will be redirected to HTTPS

var provider = new FileExtensionContentTypeProvider(); // (to specify content type and charset for css responses)
provider.Mappings[".css"] = "text/css; charset=UTF-8";
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();

#endregion

