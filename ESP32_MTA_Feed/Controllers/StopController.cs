using ESP32_MTA_Feed.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;



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
    public JsonResult GetStops([FromBody]string[] ids)
    {
        var routeService = new StopService(_configuration);
        return new JsonResult(routeService.GetStops(ids));
    }
    
    [HttpGet("/api/Stop/{id}")]
    public JsonResult GetStopRT(string route, string id)
    {
        try
    {
        string mtaEndpoint = _configuration?[$"MtaApiEndpoints:GTFS:{route}"];
        // Log the configuration value
        _logger.LogInformation("MTA Endpoint: {MtaEndpoint}", mtaEndpoint);
        
        // Other logic
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while retrieving MTA endpoint.");
        return new JsonResult(500, "An error occurred while processing the request.");
    }

        try {
        var routeService = new StopService(_configuration);
        var stopTimes = routeService.GetStopRT(route, id).Result;
        stopTimes.Sort((a, b) => a.CompareTo(b));

        List<string> stopTimesList = new List<string>();
        DateTime now = DateTime.Now;
        stopTimes.ForEach(time => stopTimesList.Add(time.Subtract(now).Minutes.ToString()));

        return new JsonResult(stopTimesList.Take(5));
        } catch (Exception e){
            return new JsonResult(e.Message + e.Source + e.StackTrace);
        }

        
    }

    [HttpGet("/api/Stop/Settings")]
    public JsonResult GetAppSettings(){
        try
            {
                //return new JsonResult(AppContext.BaseDirectory);
               return new JsonResult(_configuration.GetSection("MtaApiEndpoints:GTFS:G").Value);
               // string mtaEndpoint = _configuration?[$"MtaApiEndpoints:GTFS:{route}"];

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving MTA endpoint.");
                return new JsonResult(500, "An error occurred while processing the request.");
            }
    }
}