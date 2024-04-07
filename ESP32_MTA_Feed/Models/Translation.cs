using System.Text.Json.Serialization; 
namespace ESP32_MTA_Feed.Models{ 

    public class Translation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }
    }

}