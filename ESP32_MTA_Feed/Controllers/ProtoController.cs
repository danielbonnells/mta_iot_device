using ESP32_MTA_Feed.Services;
using Microsoft.AspNetCore.Mvc;

namespace ESP32_MTA_Feed.Controllers;
[ApiController]
[Route("/api/Proto")]
public class ProtoController : Controller
{
    [HttpGet()]
    public async void GetRoute()
    {
        await ProtoService.GetFeedAsync();
        //return new JsonResult("hi");
        
    }
}