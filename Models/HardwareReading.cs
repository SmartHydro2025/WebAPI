using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmartHydro_API.Models
{
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
}
