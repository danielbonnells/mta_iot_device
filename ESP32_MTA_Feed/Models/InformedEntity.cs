using System.Text.Json.Serialization; 
namespace ESP32_MTA_Feed.Models{ 

    public class InformedEntity
    {
        [JsonPropertyName("agency_id")]
        public string AgencyId { get; set; }

        [JsonPropertyName("route_id")]
        public string RouteId { get; set; }

        [JsonPropertyName("transit_realtime.mercury_entity_selector")]
        public TransitRealtimeMercuryEntitySelector TransitRealtimeMercuryEntitySelector { get; set; }

        [JsonPropertyName("stop_id")]
        public string StopId { get; set; }
    }

}