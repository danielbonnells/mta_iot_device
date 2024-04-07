using System.Text.Json.Serialization; 
namespace ESP32_MTA_Feed.Models{ 

    public class StationAlternative
    {
        [JsonPropertyName("affected_entity")]
        public AffectedEntity AffectedEntity { get; set; }

        [JsonPropertyName("notes")]
        public Notes Notes { get; set; }
    }

}