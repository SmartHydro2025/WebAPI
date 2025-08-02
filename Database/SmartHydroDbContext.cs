using Microsoft.EntityFrameworkCore;

namespace SmartHydro_API.Database
{
    public class SmartHydroDbContext : DbContext
    {
        public SmartHydroDbContext(DbContextOptions<SmartHydroDbContext> options) : base(options) { }

        public DbSet<SensorReading> SensorReadings { get; set; }
    }
}