using Google.Protobuf;
using System.Globalization;
using TransitRealtime;

namespace ESP32_MTA_Feed.Services
{
    public class ProtoService
    {
        public ProtoService(){}
        private static readonly HttpClient _client = new HttpClient();

        private readonly IConfiguration _configuration;
        public ProtoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static async Task GetFeedAsync()
        {
            var mtaEndpoint = "https://api-endpoint.mta.info/Dataservice/mtagtfsfeeds/nyct%2Fgtfs-g";

            try
            {
                var response = await _client.GetAsync(mtaEndpoint);
                response.EnsureSuccessStatusCode(); // Ensure success status code before processing

                var content = await response.Content.ReadAsByteArrayAsync();
                var result = GeneralService.ToObject<FeedMessage>(content);

                foreach (var entity in result.Entity)
                {
                    Console.WriteLine("--------------");
                    var tripId = entity?.TripUpdate?.Trip?.TripId;
                    var routeId = entity?.TripUpdate?.Trip?.RouteId;

                    // Console.WriteLine($"{tripId}");
                    // Console.WriteLine($"{routeId}");
                    Console.WriteLine(entity?.ToString());
                    // Check if Alert exists before accessing its properties
                    if (entity?.TripUpdate?.Trip?.StartTime != null && entity?.TripUpdate?.Trip?.StartDate != null)
                    {
                    DateTime timeStamp = DateTime.ParseExact(
                        string.IsNullOrWhiteSpace(entity.TripUpdate.Trip.StartDate)
                            ? DateTime.Today.ToString("yyyyMMdd")
                            : entity.TripUpdate.Trip.StartDate +
                            (string.IsNullOrWhiteSpace(entity.TripUpdate.Trip.StartTime)
                                ? " 00:00:00"
                                : " " + entity.TripUpdate.Trip.StartTime),
                        "yyyyMMdd HH:mm:ss",
                        CultureInfo.InvariantCulture);

                       
                        Console.WriteLine(timeStamp.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        
    }
}
