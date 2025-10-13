using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("api/tents/{mac}")] // The base route for all tent controls
public class TentControlController : ControllerBase
{
    private readonly MqttService _mqttService;
    private readonly ILogger<TentControlController> _logger;

    public TentControlController(MqttService mqttService, ILogger<TentControlController> logger)
    {
        _mqttService = mqttService;
        _logger = logger;
    }

    // A private helper method to build and publish a command to the MQTT broker.
    private async Task<IActionResult> PublishCommandAsync(string mac, string component, string action)
    {
        var command = new TentCommand
        {
            Mac = mac,
            Component = component,
            Action = action
        };

        // Serialize the command object to a JSON string.
        var payload = JsonSerializer.Serialize(command);
        // Publish the JSON payload to the "tentCommands" MQTT topic.
        await _mqttService.PublishAsync("tentCommands", payload);

        _logger.LogInformation("Published command for MAC {Mac}: Component '{Component}' -> Action '{Action}'", mac, component, action);
        return Ok(new { message = $"Command '{action}' for component '{component}' sent successfully.", command });
    }

    //Grow Light Endpoints
    [HttpPost("grow-light/on")]
    public async Task<IActionResult> TurnLightOn(string mac) => await PublishCommandAsync(mac, "grow_light", "on");

    [HttpPost("grow-light/off")]
    public async Task<IActionResult> TurnLightOff(string mac) => await PublishCommandAsync(mac, "grow_light", "off");

    // --- Extractor Fan Endpoints ---
    [HttpPost("extractor-fan/on")]
    public async Task<IActionResult> TurnExtractorFanOn(string mac) => await PublishCommandAsync(mac, "extractor_fan", "on");

    [HttpPost("extractor-fan/off")]
    public async Task<IActionResult> TurnExtractorFanOff(string mac) => await PublishCommandAsync(mac, "extractor_fan", "off");

    // --- Circulation Fan Endpoints ---
    [HttpPost("circulation-fan/on")]
    public async Task<IActionResult> TurnCirculationFanOn(string mac) => await PublishCommandAsync(mac, "circulation_fan", "on");

    [HttpPost("circulation-fan/off")]
    public async Task<IActionResult> TurnCirculationFanOff(string mac) => await PublishCommandAsync(mac, "circulation_fan", "off");

    // --- Circulation Pump Endpoints ---
    [HttpPost("circulation-pump/on")]
    public async Task<IActionResult> TurnCirculationPumpOn(string mac) => await PublishCommandAsync(mac, "circulation_pump", "on");

    [HttpPost("circulation-pump/off")]
    public async Task<IActionResult> TurnCirculationPumpOff(string mac) => await PublishCommandAsync(mac, "circulation_pump", "off");

    // --- Water Pump Endpoints ---
    [HttpPost("water-pump/on")]
    public async Task<IActionResult> TurnWaterPumpOn(string mac) => await PublishCommandAsync(mac, "water_pump", "on");

    [HttpPost("water-pump/off")]
    public async Task<IActionResult> TurnWaterPumpOff(string mac) => await PublishCommandAsync(mac, "water_pump", "off");

    // --- Nutrient Pump Endpoints ---
    [HttpPost("nutrient-pump/on")]
    public async Task<IActionResult> TurnNutrientPumpOn(string mac) => await PublishCommandAsync(mac, "nutrient_pump", "on");

    [HttpPost("nutrient-pump/off")]
    public async Task<IActionResult> TurnNutrientPumpOff(string mac) => await PublishCommandAsync(mac, "nutrient_pump", "off");
}