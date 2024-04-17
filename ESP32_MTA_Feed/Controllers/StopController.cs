using ESP32_MTA_Feed.Services;
using Microsoft.AspNetCore.Mvc;

namespace ESP32_MTA_Feed.Controllers;
[ApiController]
[Route("/api/Stop")]
public class StopController : Controller
{

    private readonly ILogger<StopController> _logger;

    private readonly IConfiguration _configuration;
    public StopController(IConfiguration configuration, ILogger<StopController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }


    [HttpPost]
    public JsonResult GetStops([FromBody] string[] ids)
    {
        var stopService = new StopService(_configuration);
        return new JsonResult(stopService.GetStops(ids));
    }

public class ErrorResponse
{
    public string Message { get; set; }
}

[HttpGet("/api/Stop/All")]
public JsonResult GetAllStops()
{
    try{


  
        return new JsonResult($@"
        {_configuration.GetConnectionString("MtaFeed")}
        {_configuration["Database:Pass"]}
        {_configuration["Database:User"]}
        ");

        var stopService = new StopService(_configuration);
        var response = stopService.GetAllStops();
        return new JsonResult(response);
    } catch (Exception e) {
        return new JsonResult(new ErrorResponse { Message = e.Message });
    }
}
    [HttpGet("/api/Stop/{id}")]
    public JsonResult GetStopRT(string route, string id)
    {
        try
        {
            if (route == null) return new JsonResult("Missing route or stop id.");
            string direction = id.EndsWith("S") ? "Southbound" : id.EndsWith("N") ? "Northbound" : "";
            var stopService = new StopService(_configuration);
            var stopTimes = stopService.GetStopRT(route, id).Result;
            stopTimes.Sort((a, b) => a.CompareTo(b));

            List<string> stopTimesList = new List<string>();
            DateTime now = DateTime.Now;
            stopTimes.ForEach(time => {
                var each = time.Subtract(now).Minutes.ToString();
                each = $"There is a {direction} {route} train arriving in {each} minutes";
                stopTimesList.Add(each);
                });

            return new JsonResult(stopTimesList.Take(5));
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