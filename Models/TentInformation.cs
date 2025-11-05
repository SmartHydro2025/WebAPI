using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmartHydro_API.Models
{
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

        [JsonPropertyName("networkName")]
        public string networkName { get; set; }
    }
}
