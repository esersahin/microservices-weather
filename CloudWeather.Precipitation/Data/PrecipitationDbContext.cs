using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Precipitation.Data
{
    public class PrecipitationDbContext : DbContext
    {
        public PrecipitationDbContext()
        {
        }

        public PrecipitationDbContext(DbContextOptions<PrecipitationDbContext> options) : base(options)
        {
        }

        public DbSet<Precipitation> Precipitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTables(modelBuilder);
        }

        private static void SnakeCaseIdentityTables(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Precipitation>().ToTable("precipitations");
        }
    }
}