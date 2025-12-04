using ESP32_MTA_Feed.Models;
using ESP32_MTA_Feed.Services;
using Microsoft.AspNetCore.Mvc;

namespace ESP32_MTA_Feed.Controllers;
[ApiController]
[Route("/api/Stop")]
public class StopController : Controller
{

    private readonly ILogger<StopController> _logger;
    private readonly StopService _stopService;
    private readonly IConfiguration _configuration;
    public StopController(IConfiguration configuration, ILogger<StopController> logger, StopService stopService)
    {
        _configuration = configuration;
        _logger = logger;
        _stopService = stopService;
    }


    [HttpPost]
    public JsonResult GetStops([FromBody] string[] ids)
    {
        return new JsonResult(_stopService.GetStops(ids));
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }

    [HttpGet("/api/Stop/All")]
    public JsonResult GetAllStops()
    {
        try
        {
            var response = _stopService.GetAllStops();
            return new JsonResult(response);
        }
        catch (Exception e)
        {
            return new JsonResult(new ErrorResponse { Message = e.Message });
        }
    }

    // [HttpGet("/api/Stop/{id}")]
    // public JsonResult GetStopRT(string route, string id, string direction)
    // {
    //     try
    //     {
    //         if (route == null) return new JsonResult("Missing route or stop id.");
    //         //string direction = id.EndsWith("S") ? "Southbound" : id.EndsWith("N") ? "Northbound" : "";
    //         var stopService = new StopService(_configuration);
    //         var stopTimes = stopService.GetStopByStopId(route, id, direction).Result;
    //         stopTimes.Sort((a, b) => a.CompareTo(b));

    //         List<string> stopTimesList = new List<string>();
    //         DateTime now = DateTime.Now;
    //         stopTimes.ForEach(time =>
    //         {
    //             var each = time.Subtract(now).Minutes.ToString();
    //             each = $"There is a {direction} {route} train arriving in {each} minutes";
    //             stopTimesList.Add(each);
    //         });

    //         return new JsonResult(stopTimesList.Take(5));
    //     }
    //     catch (Exception e)
    //     {
    //         return new JsonResult(e.Message + e.Source + e.StackTrace);
    //     }
    // }

    [HttpGet("/api/Stop/ByName/{name}")]
    public JsonResult GetStopRTByName(string name, string direction = "BOTH")
    {
        try
        {
            var options = new ConfigOptions();
            var route = new Models.Route(name)
            {
                Direction = direction
            };
            options.Add(route);

            var stopTimes = _stopService.GetStopByName(options);

        

            return new JsonResult(stopTimes);
        }
        catch (Exception e)
        {
            return new JsonResult(e.Message + e.Source + e.StackTrace);
        }
    }

    [HttpGet("/api/Stop/Settings")]
    public JsonResult GetAppSettings()
    {
        try
        {

            return new JsonResult(_configuration["MtaApiEndpoints"]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving MTA endpoint.");
            return new JsonResult(500, "An error occurred while processing the request.");
        }
    }
}