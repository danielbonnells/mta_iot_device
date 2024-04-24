using TransitRealtime;

namespace ESP32_MTA_Feed;

public class AlertService
{
    public AlertService() { }
    private static readonly HttpClient _client = new HttpClient();

    private readonly IConfiguration _configuration;

    public AlertService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<List<string>> GetAlerts(string routeId, string stopId)
    {

        try
        {

            var mtaEndpoint = _configuration?[$"MtaApiEndpoints:GTFS:SubwayAlerts"];
            var response = await _client.GetAsync(mtaEndpoint);
            response.EnsureSuccessStatusCode(); // Ensure success status code before processing
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = GeneralService.ToObject<FeedMessage>(content);

            var list = new List<string>();

            foreach (var entity in result.Entity)
            {

                    foreach (var alert in entity?.Alert?.InformedEntity)
                    {
                        if (alert.StopId == stopId)
                        {
                            
                            Console.WriteLine(entity.ToString());
                            list.Add("Stop Alert: " + entity.Alert.HeaderText?.Translation?.First()?.Text);
                        }
                    
                        // if (alert.RouteId == routeId)
                        // {
                        //     Console.WriteLine(entity.ToString());
                        //     list.Add("Route Alert: " + entity.Alert.HeaderText?.Translation?.First()?.Text);
                        // }

                }

            }


            return list;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

}
