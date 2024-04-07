using System.Text.Json.Serialization; 
namespace ESP32_MTA_Feed.Models{ 

    public class AffectedEntity
    {
        [JsonPropertyName("agency_id")]
        public string AgencyId { get; set; }

        [JsonPropertyName("stop_id")]
        public string StopId { get; set; }
    }

}