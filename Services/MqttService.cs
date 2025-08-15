using Microsoft.Extensions.DependencyInjection; // Required for IServiceScopeFactory
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using SmartHydro_API;
using SmartHydro_API.Database;
using SmartHydro_API.Interface;
using SmartHydro_API.LiveCache;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization; //New Line

// --- Data Models ---
// These classes represent the structure of the JSON data you expect from MQTT messages.
// Adjust them to match your exact data contracts.

public class SensorReading
{
    public int Id { get; set; }

    [JsonPropertyName("mac")]
    public string? Mac { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public double? Humidity { get; set; }

    [JsonPropertyName("light_level")]
    public double? LightLevel { get; set; }

    [JsonPropertyName("ph_level")]
    public double? PhLevel { get; set; }

    [JsonPropertyName("ec_level")]
    public double? EcLevel { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

//Testing purposes


public class HardwareReading
{
    public string Mac { get; set; }
    public bool GrowLightStatus { get; set; }
    public bool ExtractorFanStatus { get; set; }
    public bool CirculationFanStatus { get; set; }
    public bool CirculationPumpStatus { get; set; }
    public bool NutrientPumpStatus { get; set; }
    public bool WaterPumpStatus { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

}

public class AiEvent
{
    public string Mac { get; set; }
    public string Sensor { get; set; }
    public string Message { get; set; }
}

public class TentCommandResponse
{
    public Guid CommandId { get; set; }
    public bool Success { get; set; }
}


public class TentCommand
{
    [JsonPropertyName("Mac")] // <-- Add this
    public string Mac { get; set; }
    [JsonPropertyName("Component")] // <-- Add this
    public string Component { get; set; }
    [JsonPropertyName("Action")] // <-- Add this
    public string Action { get; set; }
}



// --- MQTT Service ---

public class MqttService : IHostedService, IDisposable
{
    private readonly ILogger<MqttService> _logger;
    private readonly IServiceScopeFactory _scopeFactory; // To resolve scoped services like DbContext
    private IManagedMqttClient _mqttClient;
    private readonly string _mqttBroker;
    private readonly string _mqttUsername;
    private readonly string _mqttPassword;
    private readonly LiveSensorCache _cache; //New Line

    public MqttService(ILogger<MqttService> logger, IServiceScopeFactory scopeFactory, LiveSensorCache cache)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _cache = cache;

        _mqttBroker = Environment.GetEnvironmentVariable("MQTT_BROKER") ?? "129.151.167.185";
        _mqttUsername = Environment.GetEnvironmentVariable("MQTT_USERNAME");
        _mqttPassword = Environment.GetEnvironmentVariable("MQTT_PASSWORD");

        InitializeMqttClient();
    }

    private void InitializeMqttClient()
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateManagedMqttClient();

        _mqttClient.ConnectedAsync += OnConnected;
        _mqttClient.DisconnectedAsync += OnDisconnected;
        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MQTT Service starting...");
        await ConnectAsync();
    }

    private async Task ConnectAsync()
    {
        var clientId = $"smart_hydro_api_{Guid.NewGuid()}";

        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(_mqttBroker)
            .WithClientId(clientId)
            .WithCleanSession();

        if (!string.IsNullOrEmpty(_mqttUsername))
        {
            optionsBuilder.WithCredentials(_mqttUsername, _mqttPassword);
        }

        var managedOptions = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
            .WithClientOptions(optionsBuilder.Build())
            .Build();

        try
        {
            await _mqttClient.StartAsync(managedOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not connect to MQTT broker.");
        }
    }

    private Task OnConnected(MqttClientConnectedEventArgs arg)
    {
        _logger.LogInformation("Successfully connected to MQTT broker.");
        // Use a fire-and-forget task to subscribe to topics
        _ = Task.Run(SubscribeToTopicsAsync);
        return Task.CompletedTask;
    }

    private async Task SubscribeToTopicsAsync()
    {
        var topics = new[]
        {
            "sensor_readings",
            "hardware_status",
            "ai_events",
            "tent_command_response",
            "tent_settings",
            "tentCommands"
        };

        foreach (var topic in topics)
        {
            try
            {
                await _mqttClient.SubscribeAsync(new[] { new MqttTopicFilterBuilder().WithTopic(topic).Build() });
                _logger.LogInformation("Subscribed to topic: {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing to topic {Topic}", topic);
            }
        }
    }

    private Task OnDisconnected(MqttClientDisconnectedEventArgs arg)
    {
        // The managed client handles reconnection automatically.
        _logger.LogWarning("Disconnected from MQTT broker. Reason: {Reason}. Will attempt to reconnect.", arg.Reason);
        return Task.CompletedTask;
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs arg)
    {
        var topic = arg.ApplicationMessage.Topic;
        var payload = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
        _logger.LogInformation("Received message on topic '{Topic}': {Payload}", topic, payload);

        try
        {
            await HandleMessageAsync(topic, payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MQTT message from topic {Topic}", topic);
        }
    }

    public async Task PublishAsync(string topic, string payload)
    {
        if (!_mqttClient.IsConnected)
        {
            _logger.LogWarning("Cannot publish message. MQTT client is not connected.");
            return;
        }

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();

        await _mqttClient.EnqueueAsync(message);
        _logger.LogInformation("Published message to topic '{Topic}'", topic);
    }

    private async Task HandleMessageAsync(string topic, string payload)
    {
        // Using IServiceScopeFactory to create a new scope for resolving scoped services
        // like a DbContext. This is the correct pattern for background services.
        using var scope = _scopeFactory.CreateScope();
        // var dbContext = scope.ServiceProvider.GetRequiredService<YourDbContext>();

        try
        {
            switch (topic)
            {
                case "sensor_readings":
                    var sensorData = JsonSerializer.Deserialize<SensorReading>(payload);
                    if (sensorData?.Mac == null)
                    {
                        _logger.LogWarning("Received sensor reading with NULL MAC: {Payload}", payload);
                    }
                    else
                    {
                        _logger.LogInformation("Deserialized SensorReading with MAC: {Mac}", sensorData.Mac);
                    }
                    await HandleSensorReadingAsync(sensorData);
                    break;

                case "hardware_status":
                    var hardwareData = JsonSerializer.Deserialize<HardwareReading>(payload);
                    await HandleHardwareReadingAsync(hardwareData /*, dbContext */);
                    break;
                case "ai_events":
                    var aiEventData = JsonSerializer.Deserialize<AiEvent>(payload);
                    await HandleAIEventAsync(aiEventData /*, dbContext */);
                    break;
                case "tent_command_response":
                    var commandResponse = JsonSerializer.Deserialize<TentCommandResponse>(payload);
                    await HandleCommandResponseAsync(commandResponse /*, dbContext */);
                    break;

                case "tent_settings":
                    // TODO: Implement handling for tent settings
                    _logger.LogInformation("Received tent_settings message. Handling not yet implemented.");
                    break;

                case "tentCommands":
                    var tentCommandData = JsonSerializer.Deserialize<TentCommand>(payload);
                    if (tentCommandData != null)
                    {
                        _logger.LogInformation(
                            "Tent command received for MAC {Mac}: Component '{Component}' -> Action '{Action}'",
                            tentCommandData.Mac, tentCommandData.Component, tentCommandData.Action
                        );

                        // TODO: If you want to store commands in DB:
                        // using var scope = _scopeFactory.CreateScope();
                        // var dbContext = scope.ServiceProvider.GetRequiredService<SmartHydroDbContext>();
                        // dbContext.TentCommands.Add(tentCommandData);
                        // await dbContext.SaveChangesAsync();
                    }
                    break;






                default:
                    _logger.LogWarning("Unknown message topic: {Topic}", topic);
                    break;
            }
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "Failed to deserialize JSON payload from topic {Topic}", topic);
        }
    }

    private async Task HandleSensorReadingAsync(SensorReading data)
    {
        _cache.Update(data); // ✅ Update in-memory cache first

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SmartHydroDbContext>();
        dbContext.SensorReadings.Add(data);
        await dbContext.SaveChangesAsync();
    }




    private async Task HandleHardwareReadingAsync(HardwareReading data /*, YourDbContext dbContext */)
    {
        // TODO: Replace with your actual database logic
        _logger.LogInformation("Storing hardware reading for tent {Mac}", data.Mac);
        // await dbContext.HardwareReadings.AddAsync(data);
        // await dbContext.Tents.Where(t => t.Mac == data.Mac).ExecuteUpdateAsync(s => s.SetProperty(t => t.LastSeen, DateTime.UtcNow));
        // await dbContext.SaveChangesAsync();
        await Task.CompletedTask; // Placeholder
    }

    private async Task HandleAIEventAsync(AiEvent data /*, YourDbContext dbContext */)
    {
        // TODO: Replace with your actual database logic
        _logger.LogInformation("Storing AI event for tent {Mac}", data.Mac);
        // await dbContext.AiEvents.AddAsync(data);
        // await dbContext.Tents.Where(t => t.Mac == data.Mac).ExecuteUpdateAsync(s => s.SetProperty(t => t.LastSeen, DateTime.UtcNow));
        // await dbContext.SaveChangesAsync();
        await Task.CompletedTask; // Placeholder
    }

    private async Task HandleCommandResponseAsync(TentCommandResponse data /*, YourDbContext dbContext */)
    {
        // TODO: Replace with your actual database logic
        _logger.LogInformation("Updating command {CommandId} with success status: {Success}", data.CommandId, data.Success);
        // await dbContext.TentCommands
        //     .Where(c => c.CommandId == data.CommandId)
        //     .ExecuteUpdateAsync(s => s.SetProperty(c => c.Success, data.Success));
        // await dbContext.SaveChangesAsync();
        await Task.CompletedTask; // Placeholder
    }


    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MQTT Service stopping...");
        await _mqttClient.StopAsync();
    }

    public void Dispose()
    {
        _mqttClient?.Dispose();
    }
}


