using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Temperature.DataAccess
{
    public class TemperatureDbContext : DbContext
    {
        // public TemperatureDbContext()
        // {
        // }

        public TemperatureDbContext(DbContextOptions<TemperatureDbContext> options) : base(options)
        {
        }

        public DbSet<Temperature> Temperatures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTables(modelBuilder);
        }

        private static void SnakeCaseIdentityTables(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>().ToTable("temperatures");
        }
    }
}