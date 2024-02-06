using Microsoft.AspNetCore.Identity;
using QLite.Data.Models.Auth;
using QLiteAuthenticationServer.Context;
using QLiteAuthenticationServer.Helpers;
using Serilog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace QLiteAuthenticationServer.Services
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


                    #region USER - BAYKUS

                    var baykus = userMgr.FindByNameAsync("JA").Result;
                    if (baykus == null)
                    {
                        baykus = new ApplicationUser
                        {
                            UserName = "JA",
                            Email = "auth@quavis.aero",
                            EmailConfirmed = true,
                            AccountType = AccountType.Adminpotal,
                            IsActive = true,
                            TwoFactorSecret = Utils.GenerateTwoFactorKey(),
                            TwoFactorEnabled = false,
                            QRless = true,
                        };
                        var result = userMgr.CreateAsync(baykus, "!Quavis_2023*").Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }

                        result = userMgr.AddClaimsAsync(baykus, new Claim[]{
                                    new Claim("asBaykus", "asBaykus")
                                }).Result;
                        if (!result.Succeeded)
                        {
                            throw new Exception(result.Errors.First().Description);
                        }
                        Log.Debug("Baykus created");
                    }
                    else
                    {
                        Log.Debug("Baykus already exists");
                    }

                    #endregion
                }
            }
        }
    }

}
