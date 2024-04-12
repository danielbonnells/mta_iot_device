using ESP32_MTA_Feed.Services;
using Microsoft.AspNetCore.Mvc;

namespace ESP32_MTA_Feed.Controllers;
[ApiController]
[Route("/api/Stop")]
public class StopController(IConfiguration iConfig) : Controller
{
    [HttpPost]
    public JsonResult GetStops([FromBody]string[] ids)
    {
        var routeService = new StopService(iConfig);
        return new JsonResult(routeService.GetStops(ids));
    }
    
    [HttpGet("/api/Stop/{id}")]
    public JsonResult GetStopRT(string route, string id)
    {
        try {
        var routeService = new StopService(iConfig);
        var stopTimes = routeService.GetStopRT(route, id).Result;
        stopTimes.Sort((a, b) => a.CompareTo(b));

        List<string> stopTimesList = new List<string>();
        DateTime now = DateTime.Now;
        stopTimes.ForEach(time => stopTimesList.Add(time.Subtract(now).Minutes.ToString()));

        return new JsonResult(stopTimesList.Take(5));
        } catch {
            throw;
        }

        
    }
}