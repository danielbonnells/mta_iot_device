using System.Text.Json;
using ESP32_MTA_Feed.Models;
using ESP32_MTA_Feed.Services;
using Microsoft.AspNetCore.Mvc;


namespace ESP32_MTA_Feed.Controllers;
[ApiController]
[Route("/api/ESP32")]
public class ESP32Controller : Controller
{

    private readonly ILogger<ESP32Controller> _logger;

    private readonly IConfiguration _configuration;
    public ESP32Controller(IConfiguration configuration, ILogger<ESP32Controller> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }


    public class ErrorResponse
    {
        public required string Message { get; set; }
    }


    [HttpPost]
    public async Task<IActionResult> PostConfigOptions([FromBody] ConfigOptions configOptions)
    {
        try
        {
            var esp32Service = new ESP32Service(_configuration);
            return new JsonResult(esp32Service.GetAllData(configOptions));
        }
        catch (JsonException)
        {
            // If there's an error deserializing the JSON, return a bad request response
            return BadRequest("Invalid JSON format");
        }
    }

}
