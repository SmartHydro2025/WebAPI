using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SmartHydro_API.Models
{
    public class AiEvent
    {


        [Key]
        public int ID { get; set; }

        [JsonPropertyName("mac")]
        public string Mac { get; set; }

        [JsonPropertyName("sensor")]
        public string Sensor { get; set; }

        [JsonPropertyName("message")] 
        public string Message { get; set; }

    }
}
