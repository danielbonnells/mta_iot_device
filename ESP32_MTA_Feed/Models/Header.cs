using System.Text.Json.Serialization; 
namespace ESP32_MTA_Feed.Models{ 

    public class Header
    {
        [JsonPropertyName("gtfs_realtime_version")]
        public string GtfsRealtimeVersion { get; set; }

        [JsonPropertyName("incrementality")]
        public string Incrementality { get; set; }

        [JsonPropertyName("timestamp")]
        public int Timestamp { get; set; }

        [JsonPropertyName("transit_realtime.mercury_feed_header")]
        public TransitRealtimeMercuryFeedHeader TransitRealtimeMercuryFeedHeader { get; set; }
    }

}