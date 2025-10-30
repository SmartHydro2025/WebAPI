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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization; //New Line

// --- Data Models ---


//class to store details about component readings from arduino
public class SensorReading
{
    [Key]
    public int? Id { get; set; }

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

//class to store on and off status of hardware components
public class HardwareReading
{
    [Key]
    public int ID { get; set; }
    [JsonPropertyName("mac")]
    public string Mac { get; set; }
    [JsonPropertyName("grow_light_status")]
    public string GrowLightStatus { get; set; }
    [JsonPropertyName("extractor_fan_status")]
    public string ExtractorFanStatus { get; set; }
    [JsonPropertyName("circulation_fan_status")]
    public string CirculationFanStatus { get; set; }
    [JsonPropertyName("circulation_pump_status")]
    public string CirculationPumpStatus { get; set; }
    [JsonPropertyName("nutrient_pump_status")]
    public string NutrientPumpStatus { get; set; }
    [JsonPropertyName("water_pump_status")]
    public string WaterPumpStatus { get; set; }

    [Column("DateTime")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

}

//class to store details about which sensor had an ai event trigger
public class AiEvent
{
    [Key]
    public int ID { get; set; }

    [JsonPropertyName("mac")] 
    public string Mac { get; set; }

    [JsonPropertyName("sensor")] // ***VERIFY NAME IN JSON
    public string Sensor { get; set; }

    [JsonPropertyName("message")] // **VERIFY NAME IN JSON
    public string Message { get; set; }
}

//class to store if a command is executed
public class TentCommandResponse
{
    public Guid CommandId { get; set; }
    public bool Success { get; set; }
}

//class to store which component received a command
public class TentCommand
{
    
    [JsonPropertyName("mac")] // <-- Add this
    public string Mac { get; set; }
    [JsonPropertyName("component")] 
    public string Component { get; set; }
    [JsonPropertyName("action")] 
    public string Action { get; set; }
}

//class to store information about current tent open in android
public class TentInformation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    [JsonPropertyName("Name")]
    public string tentName { get; set; }

    [JsonPropertyName("Location")]
    public string tentLocation { get; set; }

    [JsonPropertyName("Mac")]
    public string Mac { get; set; }

  

}

#region MQTT
// MQTT Service 

public class MqttService : IHostedService, IDisposable
{
    private readonly ILogger<MqttService> _logger;
    private readonly IServiceScopeFactory _scopeFactory; // To resolve scoped services like DbContext
    private IManagedMqttClient _mqttClient;
    private readonly string _mqttBroker;
    private readonly string _mqttUsername;
    private readonly string _mqttPassword;
    private readonly LiveSensorCache _cache; //New Line
    private readonly LiveHardwareStatusCache _hardwarecache; //keeps track of hardware statuses
    private readonly AIEventCache _aieventcache; //logs ai events as they trigger
    private readonly LiveTentInformationCache _tentcache; //logs tent details

    public MqttService(ILogger<MqttService> logger, IServiceScopeFactory scopeFactory, LiveSensorCache cache, LiveHardwareStatusCache hardwarecache,
        AIEventCache aieventcache, LiveTentInformationCache tentcache)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _cache = cache;
        _hardwarecache = hardwarecache;
        _aieventcache = aieventcache;
        _tentcache = tentcache;

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
                    //var hardwareData = JsonSerializer.Deserialize<HardwareReading>(payload);

                    var jsonNode = JsonNode.Parse(payload);

                    var hardwareData = new HardwareReading
                    {
                        Mac = jsonNode["mac"]?.GetValue<string>(),
                        GrowLightStatus = jsonNode["grow_light_status"]?.GetValue<string>(),
                        ExtractorFanStatus = jsonNode["extractor_fan_status"]?.GetValue<string>(),
                        CirculationFanStatus = jsonNode["circulation_fan_status"]?.GetValue<string>(),
                        CirculationPumpStatus = jsonNode["circulation_pump_status"]?.GetValue<string>(),
                        NutrientPumpStatus = jsonNode["nutrient_pump_status"]?.GetValue<string>(),
                        WaterPumpStatus = jsonNode["water_pump_status"]?.GetValue<string>()
                    };

                    await HandleHardwareReadingAsync(hardwareData);
                    break;

                case "ai_events":
                    var aiEventData = JsonSerializer.Deserialize<AiEvent>(payload);
                    await HandleAIEventAsync(aiEventData);
                    break;

                case "tent_command_response":
                    var commandResponse = JsonSerializer.Deserialize<TentCommandResponse>(payload);
                    await HandleCommandResponseAsync(commandResponse);
                    break;

                case "tentCommands":
                    var tentCommandData = JsonSerializer.Deserialize<TentCommand>(payload);
                    if (tentCommandData != null)
                    {
                        _logger.LogInformation(
                            "Tent command received for MAC {Mac}: Component '{Component}' -> Action '{Action}'",
                            tentCommandData.Mac, tentCommandData.Component, tentCommandData.Action
                        );

                    }
                    break;

                case "tent_information":
                    var tentInformation = JsonSerializer.Deserialize<TentInformation>(payload);
                    await HandleTentInformationAsync(tentInformation);
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

    #endregion

    //logs sensor readings from arduino to db
    public async Task HandleSensorReadingAsync(SensorReading data)
    {
        _cache.Update(data);

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SmartHydroDbContext>();
        dbContext.SensorReadings.Add(data);
        await dbContext.SaveChangesAsync();
    }

    //logs hardware statuses to db
    public async Task HandleHardwareReadingAsync(HardwareReading data)
    {
        _hardwarecache.Update(data); //update in-memory cache 

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SmartHydroDbContext>(); //connects to db
        _logger.LogInformation("Storing hardware reading for tent {Mac}", data.Mac);
        dbContext.HardwareStatuses.Add(data); //stores hardware statuses in db
        await dbContext.SaveChangesAsync();
    }

    //logs when ai events are triggered
    public async Task HandleAIEventAsync(AiEvent data)
    {
        _aieventcache.Update(data); //update in-memory cache 

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SmartHydroDbContext>(); //connects to db
        _logger.LogInformation("Storing AI event for tent {Mac}", data.Mac); 
         dbContext.AiEvents.Add(data);
        await dbContext.SaveChangesAsync();
    }

    public async Task HandleCommandResponseAsync(TentCommandResponse data)
    {
        // TODO: Replace with your actual database logic
        _logger.LogInformation("Updating command {CommandId} with success status: {Success}", data.CommandId, data.Success);
        // await dbContext.TentCommands
        //     .Where(c => c.CommandId == data.CommandId)
        //     .ExecuteUpdateAsync(s => s.SetProperty(c => c.Success, data.Success));
        // await dbContext.SaveChangesAsync();
        await Task.CompletedTask; // Placeholder
    }

    //logs hardware statuses to db
    public async Task HandleTentInformationAsync(TentInformation data)
    {
        _tentcache.Update(data); //update in-memory cache 

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SmartHydroDbContext>(); //connects to db
        _logger.LogInformation("Storing for tent at mac address: {mac}", data.Mac);
        dbContext.TentInformation.Add(data); //stores tent information in db
        await dbContext.SaveChangesAsync();
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


