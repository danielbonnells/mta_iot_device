using System.Text.Json.Serialization; 
namespace ESP32_MTA_Feed.Models{ 

    public class Entity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("alert")]
        public Alert Alert { get; set; }
    }

}