using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartHydro_API.Models
{
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
}
