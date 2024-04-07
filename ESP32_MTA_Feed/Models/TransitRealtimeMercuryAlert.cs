using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace ESP32_MTA_Feed.Models{ 

    public class TransitRealtimeMercuryAlert
    {
        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public int UpdatedAt { get; set; }

        [JsonPropertyName("alert_type")]
        public string AlertType { get; set; }

        [JsonPropertyName("display_before_active")]
        public int DisplayBeforeActive { get; set; }

        [JsonPropertyName("human_readable_active_period")]
        public HumanReadableActivePeriod HumanReadableActivePeriod { get; set; }

        [JsonPropertyName("clone_id")]
        public string CloneId { get; set; }

        [JsonPropertyName("service_plan_number")]
        public List<string> ServicePlanNumber { get; set; }

        [JsonPropertyName("general_order_number")]
        public List<string> GeneralOrderNumber { get; set; }

        [JsonPropertyName("station_alternative")]
        public List<StationAlternative> StationAlternative { get; set; }
    }

}