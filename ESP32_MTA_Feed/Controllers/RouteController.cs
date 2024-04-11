using Microsoft.AspNetCore.Mvc;
using ESP32_MTA_Feed.Services;

namespace ESP32_MTA_Feed.Controllers;

[ApiController]
[Route("/api/Route")]
public class RouteController(IConfiguration iConfig) : Controller
{
    [HttpPost]
    public JsonResult GetRoutes([FromBody]string[] ids)
    {
        var routeService = new RouteService(iConfig);
        return new JsonResult(routeService.GetRoutes(ids));
    }
    
    [HttpGet("/api/Route/{id}")]
    public JsonResult GetRoute(string id)
    {
        
        var routeService = new RouteService(iConfig);
        return new JsonResult(routeService.GetRoute(id));
        
    }
}