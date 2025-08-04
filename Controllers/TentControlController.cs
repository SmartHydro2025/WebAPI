using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class TentControlController : ControllerBase
{
    private readonly MqttService _mqttService;

    public TentControlController(MqttService mqttService)
    {
        _mqttService = mqttService;
    }

    [HttpPost("send-command")]
    public async Task<IActionResult> SendCommand([FromBody] TentCommand command)
    {
        if (string.IsNullOrEmpty(command.Mac) || string.IsNullOrEmpty(command.Component) || string.IsNullOrEmpty(command.Action))
            return BadRequest("Invalid command format.");

        var payload = JsonSerializer.Serialize(command);
        await _mqttService.PublishAsync("tentCommands", payload);

        return Ok(new { message = "Command published successfully", command });
    }
}
