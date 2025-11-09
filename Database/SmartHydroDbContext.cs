using Microsoft.EntityFrameworkCore;
using SmartHydro_API.Models;

namespace SmartHydro_API.Database
{
    public class SmartHydroDbContext : DbContext
    {
        public SmartHydroDbContext(DbContextOptions<SmartHydroDbContext> options) : base(options) { }

        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<HardwareReading> HardwareStatuses { get; set; }
        public DbSet<AiEvent> AiEvents { get; set; }
        public DbSet<TentInformation> TentInformation { get; set; }


    }
}