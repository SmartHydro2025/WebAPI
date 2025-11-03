using SmartHydro_API.Database;
using SmartHydro_API.Interface;

//ISensorReadingStore is implemented (Raveendran, 2022)
public class SensorReadingSqlStore : ISensorReadingStore
{
    private readonly SmartHydroDbContext _context;

    public SensorReadingSqlStore(SmartHydroDbContext context)
    {
        _context = context;
    }

    //(Raveendran, 2022)  Adds a new sensor reading to the database
    public void Update(SensorReading reading)
    {
        _context.SensorReadings.Add(reading);
        _context.SaveChanges();
    }

    //(Raveendran, 2022) Retrieves all sensor readings, ordered by most recent first.
    public List<SensorReading> GetAll()
    {
        return _context.SensorReadings.OrderByDescending(r => r.Timestamp).ToList();
    }

    //(Raveendran, 2022)  Gets the most recent sensor reading for a specific MAC address.
    public SensorReading GetByMac(string mac)
    {
        //(D, 2025; Vedpathak, 2024)
        return _context.SensorReadings
            .Where(r => r.Mac == mac)
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefault();
    }

    
}


/*
REFERENCES
====================
D, S. 2025. LINQ in C# Tutorial for Beginners: 101 C# LINQ Operations. [Online]. Available at:    https://www.c-sharpcorner.com/article/linq-in-c-sharp-tutorial-for-beginners-101-c-sharp-linq-operations/
Raveendran, P. 2022. Multiple Interface Implementations In ASP.NET Core. [Online]. Available at: https://www.c-sharpcorner.com/article/multiple-interface-implementations-in-asp-net-core/
Vedpathak, Y. 2024. Querying with LINQ. [Online]. Available at:  https://www.c-sharpcorner.com/blogs/querying-with-linq 
 */
