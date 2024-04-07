using System.Text.Json.Serialization; 
namespace ESP32_MTA_Feed.Models{ 

    public class TransitRealtimeMercuryFeedHeader
    {
        [JsonPropertyName("mercury_version")]
        public string MercuryVersion { get; set; }
    }

}