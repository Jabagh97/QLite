using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLite.Data.Models.Auth;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace QLiteAuthenticationServer.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } // this is where cryptographic keys are stored

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
    }

}
