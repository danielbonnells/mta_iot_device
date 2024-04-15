using Microsoft.AspNetCore.Mvc;

namespace ESP32_MTA_Feed;

[ApiController]
[Route("/api/Alerts")]
public class AlertController : Controller
{

    private readonly ILogger<AlertController> _logger;

    private readonly IConfiguration _configuration;
    public AlertController(IConfiguration configuration, ILogger<AlertController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }


    [HttpGet("/api/Alerts/{id}")]
    public JsonResult GetAlerts(string route, string id)
    {
        try
        {
            if (route == null) return new JsonResult("Missing route or stop id.");
            //string direction = id.EndsWith("S") ? "Southbound" : id.EndsWith("N") ? "Northbound" : "";
            var alertService = new AlertService(_configuration);
            var alerts = alertService.GetAlerts(route, id).Result;
            // stopTimes.Sort((a, b) => a.CompareTo(b));

            // List<string> stopTimesList = new List<string>();
            // DateTime now = DateTime.Now;
            // stopTimes.ForEach(time =>
            // {
            //     var each = time.Subtract(now).Minutes.ToString();
            //     each = $"There is a {direction} {route} train arriving in {each} minutes";
            //     stopTimesList.Add(each);
            // });

            return new JsonResult(alerts);
        }
        catch (Exception e)
        {
            return new JsonResult(e.Message + e.Source + e.StackTrace);
        }


    }

}
