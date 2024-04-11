using ESP32_MTA_Feed.Services;
using Microsoft.AspNetCore.Mvc;

namespace ESP32_MTA_Feed.Controllers;
[ApiController]
[Route("/api/Stop")]
public class StopController(IConfiguration iConfig) : Controller
{
    [HttpPost]
    public JsonResult GetRoutes([FromBody]string[] ids)
    {
        var routeService = new StopService(iConfig);
        return new JsonResult(routeService.GetStops(ids));
    }
    
    [HttpGet("/api/Stop/{id}")]
    public JsonResult GetRoute(string id)
    {
        
        var routeService = new StopService(iConfig);
        return new JsonResult(routeService.GetStop(id));
        
    }
}