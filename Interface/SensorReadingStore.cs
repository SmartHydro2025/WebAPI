using SmartHydro_API.Database;
using SmartHydro_API.Interface;

public class SensorReadingSqlStore : ISensorReadingStore
{
    private readonly SmartHydroDbContext _context;

    public SensorReadingSqlStore(SmartHydroDbContext context)
    {
        _context = context;
    }

    public void Update(SensorReading reading)
    {
        _context.SensorReadings.Add(reading);
        _context.SaveChanges();
    }

    public List<SensorReading> GetAll()
    {
        return _context.SensorReadings.OrderByDescending(r => r.Timestamp).ToList();
    }

    public SensorReading GetByMac(string mac)
    {
        return _context.SensorReadings
            .Where(r => r.Mac == mac)
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefault();
    }
}
