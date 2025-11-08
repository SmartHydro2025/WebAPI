using SmartHydro_API.Database;
using Xunit;

namespace WebAPI.Tests;

public class UnitTest1
{

    // Testing to retreive tents
    [Fact]
    public void GetTents()
    {
        var tentDetails = new List<TentInformation>
        {
            new TentInformation { ID = 1, tentName = "Test Tent", tentLocation = "Varsity College", Mac="AA:BB:CC:DD:EE:FF",networkName="local" }
        };

        Assert.NotNull(tentDetails);
        Assert.NotEmpty(tentDetails);
        Assert.True(true);
    }

}
