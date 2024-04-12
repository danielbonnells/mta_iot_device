using System.Globalization;
using ESP32_MTA_Feed.Models;
using TransitRealtime;
using Microsoft.Extensions.Configuration;

namespace ESP32_MTA_Feed.Services;

public class StopService
{
    public StopService() { }
    private static readonly HttpClient _client = new HttpClient();

    private readonly IConfiguration _configuration;

    public StopService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public List<string> GetStops(IEnumerable<string> ids)
    {
        var list = new List<string>();

        foreach (string id in ids)
        {
            list.AddRange(GetStop(id));
        }

        return list;
    }

    public List<string> GetStop(string id)
    {

        var mtaEndpoint = _configuration.GetSection("MtaApiEndpoints").GetSection("JSON").GetSection("SubwayAlerts").Value;
        var response =
            _client.GetAsync(mtaEndpoint);
        var content = response.Result.Content.ReadFromJsonAsync<Root>();
        var list = new List<string>();

        content?.Result?.Entity.ForEach(entity =>
        {
            if (entity.Alert.InformedEntity.Any(stop => stop.StopId == id))
            {
                list.Add(entity.Alert.HeaderText.Translation.First().Text);
            }
        });

        return list;
    }

    public async Task<List<DateTime>> GetStopRT(string routeId, string stopId)
    {
        List<DateTime> dates = [];
        // string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        // string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
            string mtaEndpoint = _configuration[$"MtaApiEndpoints:GTFS:{routeId}"];

        try
        {
            

            // var config = new ConfigurationBuilder()
            //     .SetBasePath(strWorkPath)
            //     .AddJsonFile("appsettings.json").Build();
           
            //string mtaEndpoint = "https://api-endpoint.mta.info/Dataservice/mtagtfsfeeds/nyct%2Fgtfs-g";
            // var mtaEndpoint = _configuration.GetSection("MtaApiEndpoints").GetSection("GTFS").GetSection(routeId).Value;
            var response = await _client.GetAsync(mtaEndpoint);
            response.EnsureSuccessStatusCode(); // Ensure success status code before processing
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = ProtoService.ToObject<FeedMessage>(content);

            foreach (var entity in result.Entity)
            {
               
                if (routeId == entity?.TripUpdate?.Trip?.RouteId){

                    foreach(var stop in entity?.TripUpdate?.StopTimeUpdate){
                        if (stop.StopId == stopId){
                             Console.WriteLine(entity.ToString());
                            var date = UnixTimeStampToDateTime(stop.Arrival.Time);
                            dates.Add(date);
                            Console.WriteLine($"{date}");

                        }
                    }
                }

            }

             return dates;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            //return dates;
            throw new Exception($"Attempted uri: " + mtaEndpoint);
        }
    }

    public static DateTime UnixTimeStampToDateTime( double unixTimeStamp )
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
        return dateTime;
    }
}