using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.Extensions.Logging; // Add this using statement

[ApiController]
[Route("api/tents/{mac}")] // A more RESTful route that includes the MAC address
public class TentControlController : ControllerBase
{
    private readonly MqttService _mqttService;
    private readonly ILogger<TentControlController> _logger;

    public TentControlController(MqttService mqttService, ILogger<TentControlController> logger)
    {
        _mqttService = mqttService;
        _logger = logger;
    }

    /// <summary>
    /// A private helper method to avoid duplicating code. It builds and publishes the command.
    /// </summary>
    private async Task<IActionResult> PublishCommandAsync(string mac, string component, string action)
    {
        // The MAC address is now taken from the URL route
        var command = new TentCommand
        {
            Mac = mac,
            Component = component,
            Action = action
        };

        var payload = JsonSerializer.Serialize(command);
        await _mqttService.PublishAsync("tentCommands", payload);

        _logger.LogInformation("Published command for MAC {Mac}: Component '{Component}' -> Action '{Action}'", mac, component, action);

        return Ok(new { message = $"Command '{action}' for component '{component}' published successfully.", command });
    }

    // --- Grow Light Endpoints ---

    [HttpPost("light/on")]
    public async Task<IActionResult> TurnLightOn(string mac)
    {
        return await PublishCommandAsync(mac, "grow_light", "on");
    }

    [HttpPost("light/off")]
    public async Task<IActionResult> TurnLightOff(string mac)
    {
        return await PublishCommandAsync(mac, "grow_light", "off");
    }

    // --- Extractor Fan Endpoints ---

    [HttpPost("extractor-fan/on")]
    public async Task<IActionResult> TurnExtractorFanOn(string mac)
    {
        return await PublishCommandAsync(mac, "extractor_fan", "on");
    }

    [HttpPost("extractor-fan/off")]
    public async Task<IActionResult> TurnExtractorFanOff(string mac)
    {
        return await PublishCommandAsync(mac, "extractor_fan", "off");
    }

    // --- Add other component endpoints here in the same pattern ---
    // Example: Water Pump
    // [HttpPost("water-pump/on")]
    // public async Task<IActionResult> TurnWaterPumpOn(string mac)
    // {
    //     return await PublishCommandAsync(mac, "water_pump", "on");
    // }
}