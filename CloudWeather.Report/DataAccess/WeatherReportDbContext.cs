using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Report.DataAccess
{
    public class WeatherReportDbContext : DbContext
    {
        public WeatherReportDbContext()
        {
        }

        public WeatherReportDbContext(DbContextOptions<WeatherReportDbContext> options) : base(options)
        {
        }

        public DbSet<WeatherReport> WeatherReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SnakeCaseIdentityTables(modelBuilder);
        }

        private static void SnakeCaseIdentityTables(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherReport>().ToTable("weather_reports");
        }

    }
}