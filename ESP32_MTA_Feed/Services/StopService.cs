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
    /// <summary>
    /// Get the alerts for a single stop.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get the arrival times for a paticular stop for a particular route/train line.
    /// </summary>
    /// <param name="routeId"></param>
    /// <param name="route"></param>
    /// <returns></returns>
    public async Task<List<Models.Route>?> GetStopByStopId(string routeId, Models.Route route)
    {

        string routeEndpoint = GeneralService.MtaUri(routeId);
        var routes = new List<Models.Route>();
        //If both directions are requested, start with North
        string? direction = route.Direction == "BOTH" ? "N" : route.Direction;
        int count = 0;
        try
        {
            string? mtaEndpoint = _configuration?[$"MtaApiEndpoints:GTFS:{routeEndpoint}"];

            if (string.IsNullOrEmpty(mtaEndpoint)) return null;

            var response = await _client.GetAsync(mtaEndpoint);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = GeneralService.ToObject<FeedMessage>(content);

        secondDirection:
            var newRoute = new Models.Route(route.StopName)
            {
                RouteId = route.RouteId,
                StopId = route.StopId,
                Direction = route.Direction,
                ArrivalTimes = route.ArrivalTimes != null ? new List<DateTime>(route.ArrivalTimes) : null
            };
            newRoute.Direction = direction;
            newRoute.ArrivalTimes = [];

            foreach (var entity in result.Entity)
            {
                if (routeId == entity?.TripUpdate?.Trip?.RouteId)
                {
                    foreach (var stop in entity?.TripUpdate?.StopTimeUpdate)
                    {
                     Console.WriteLine(stop);
                        if (stop.StopId == route.StopId + direction)
                        {
                            if(stop.Arrival != null){
                            var date = GeneralService.UnixTimeStampToDateTime(stop.Arrival.Time);
                            newRoute.ArrivalTimes.Add(date);
                            }
                            
                        }
                    }
                }

            }

            if(route.Direction == "BOTH" && count < 1) {
                routes.Add(newRoute);
                direction = "S";
                count++;
                goto secondDirection;
            } else {
                routes.Add(newRoute);
            }


            return routes.Where(r => r.ArrivalTimes?.Count > 0).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Get all of the trains arriving to a given stop, by its name, with both northbound and southbound directions.
    /// </summary>
    /// <param name="options"></param>
    /// <returns>A list of Routes</returns>
    public List<Models.Route> GetStopByName(ConfigOptions options)
    {

        var db = new MtaFeedContext(_configuration);
        var list = new List<Models.Route>();

        try
        {
            foreach (Models.Route route in options.Routes)
            {
                var stops = db.SubwayStops.Where(stop => stop.StopName == route.StopName.Trim() && stop.ParentStation == "NULL").ToList();

                if(stops == null || stops.Count == 0) throw new Exception("No stops found by that name.");

                foreach (var stop in stops)
                {
                    var relatedLines = GeneralService.GetRelatedLines(stop.StopId[..1]);

                    foreach (var line in relatedLines)
                    {
                           var r = new Models.Route(route.StopName){
                            StopId = stop.StopId,
                            Direction = route.Direction,
                            RouteId = line
                        };

                        var newRoutes = GetStopByStopId(line, r).Result;
                        if(newRoutes != null){
                           list.AddRange(newRoutes);
                        }
                    }
                }
            }


            return list;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public List<SubwayStop> GetAllStops()
    {
        try
        {
            var db = new MtaFeedContext(_configuration);
            var list = db.SubwayStops.ToList();


            return list;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}