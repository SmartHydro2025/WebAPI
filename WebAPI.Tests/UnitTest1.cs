using Microsoft.EntityFrameworkCore;
using SmartHydro_API.Database;
using SmartHydro_API.LiveCache;
using Xunit;

namespace WebAPI.Tests;

public class UnitTest1
{
    private readonly SmartHydroDbContext _dbContext;
    private readonly LiveSensorCache _cache;
    private readonly AIEventCache _AIcache;

    [Fact]
    public void GetTents()
    {
        var tentDetails = _dbContext.TentInformation.ToList();
        Assert.NotNull(tentDetails);
    }

    [Fact]
    public void GetSensorReadings()
    {
        string mac = "BD:1D:B6:7C:E5:A4";
        var reading = _cache.GetLatest(mac);

        Assert.NotNull(reading);
    }

    [Fact]
    public void GetTempAIEvent()
    {
        string mac = "BD:1D:B6:7C:E5:A4";
        var aiEvent = _AIcache.GetLatest(mac);
        Assert.NotNull(aiEvent.Message);

        var dbEvent = _dbContext.AiEvents
                    .Where(e => e.Mac == mac && e.Sensor.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(e => e.ID) 
                    .FirstOrDefault();
        Assert.NotNull(dbEvent.Message);

    }
}
