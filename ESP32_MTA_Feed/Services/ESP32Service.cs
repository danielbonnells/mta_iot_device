using ESP32_MTA_Feed.Models;
using ESP32_MTA_Feed.Services;

namespace ESP32_MTA_Feed;

public class ESP32Service
{
    public ESP32Service() { }
    private static readonly HttpClient _client = new HttpClient();

    private readonly IConfiguration _configuration;

    public ESP32Service(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public class ESPResult{
        public List<string> MainDisplayText { get; set;}
        public  List<List<string>> SecondaryDisplayText { get; set;}

    }
    public ESPResult GetAllData(ConfigOptions configOptions){
        
            var stopService = new StopService(_configuration);
            var routes = stopService.GetStopByName(configOptions);
            List<string> stopTimesList = new List<string>();
            List<List<string>> alertsList = new ();
            string latestStopName = "";
            
            foreach (var route in routes){
                if(latestStopName != route.StopName) {
                    stopTimesList.Add(route.StopName + " >>> ");
                    latestStopName = route.StopName;
                }
                route.ArrivalTimes.Sort((a, b) => a.CompareTo(b));

                DateTime now = DateTime.Now;
                var mostRecentTimes = route.ArrivalTimes.Where(date => date > DateTime.Now).Take(3);
                string direction = route.Direction == "N" ? "UP" : "DOWN";
                var minutes = string.Empty;
                foreach (var time in mostRecentTimes){
                     var each = time.Subtract(now).Minutes.ToString();
                     minutes += each + ", ";
                }
                string textLine = $"{direction} {route.RouteId} in " + minutes.Substring(0, minutes.Length - 2) + " mins.";
                stopTimesList.Add(textLine);

                var alertService = new AlertService(_configuration);
                var alerts = alertService.GetAlerts(route.RouteId, route.StopId).Result;
                if(alerts.Count > 0){

                    if (!alertsList.Any(list => list.SequenceEqual(alerts)))
                    {
                        // If it doesn't exist, add it to the list of lists
                        alertsList.Add(alerts);
                    }
                }
            }

        return new ESPResult{
            MainDisplayText = stopTimesList,
            SecondaryDisplayText = alertsList,
        };

    }
}
