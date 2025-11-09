using System.Text.Json.Serialization;

namespace SmartHydro_API.Models
{
    public class TentCommand
    {

        [JsonPropertyName("mac")]
        public string Mac { get; set; }
        [JsonPropertyName("component")]
        public string Component { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }

    }
}
