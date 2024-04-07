using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace ESP32_MTA_Feed.Models{ 

    public class Root
    {
        [JsonPropertyName("header")]
        public Header Header { get; set; }

        [JsonPropertyName("entity")]
        public List<Entity> Entity { get; set; }
    }

}