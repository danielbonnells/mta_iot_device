using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace ESP32_MTA_Feed.Models{ 

    public class Notes
    {
        [JsonPropertyName("translation")]
        public List<Translation> Translation { get; set; }
    }

}