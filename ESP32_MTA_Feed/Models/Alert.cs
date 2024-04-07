using System.Text.Json.Serialization; 
using System.Collections.Generic; 
namespace ESP32_MTA_Feed.Models{ 

    public class Alert
    {
        [JsonPropertyName("active_period")]
        public List<ActivePeriod> ActivePeriod { get; set; }

        [JsonPropertyName("informed_entity")]
        public List<InformedEntity> InformedEntity { get; set; }

        [JsonPropertyName("header_text")]
        public HeaderText HeaderText { get; set; }

        [JsonPropertyName("description_text")]
        public DescriptionText DescriptionText { get; set; }

        [JsonPropertyName("transit_realtime.mercury_alert")]
        public TransitRealtimeMercuryAlert TransitRealtimeMercuryAlert { get; set; }
    }

}