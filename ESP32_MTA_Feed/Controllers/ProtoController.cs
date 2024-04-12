using Microsoft.AspNetCore.Mvc;

namespace ESP32_MTA_Feed.Controllers;

public class ProtoController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}