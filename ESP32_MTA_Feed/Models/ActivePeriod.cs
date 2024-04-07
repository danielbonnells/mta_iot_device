using System.Text.Json.Serialization; 
namespace ESP32_MTA_Feed.Models{ 

    public class ActivePeriod
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("end")]
        public int? End { get; set; }
    }

}