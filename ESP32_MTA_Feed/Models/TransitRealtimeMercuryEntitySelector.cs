using System.Text.Json.Serialization; 
namespace ESP32_MTA_Feed.Models{ 

    public class TransitRealtimeMercuryEntitySelector
    {
        [JsonPropertyName("sort_order")]
        public string SortOrder { get; set; }
    }

}