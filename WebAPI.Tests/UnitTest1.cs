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

}
