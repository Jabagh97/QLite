using Microsoft.EntityFrameworkCore;

namespace PortalPOC.Context
{
 
   

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        
        //public DbSet<YourEntity1> Entity1 { get; set; }
        //public DbSet<YourEntity2> Entity2 { get; set; }
        




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entity relationships, constraints, etc.
        }


    }

}
