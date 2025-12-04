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

    private readonly MqttService _mqttService;
        private readonly StopService _stopService;


    private readonly IConfiguration _configuration;
    public ESP32Controller(IConfiguration configuration, ILogger<ESP32Controller> logger, MqttService mqttService, StopService stopService)
    {
        _configuration = configuration;
        _logger = logger;
        _mqttService = mqttService;
        _stopService = stopService;
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
            var esp32Service = new ESP32Service(_configuration, _stopService);
            return new JsonResult(esp32Service.GetAllData(configOptions));
        }
        catch (JsonException)
        {
            // If there's an error deserializing the JSON, return a bad request response
            return BadRequest("Invalid JSON format");
        }
    }

   [HttpGet]
    public async Task<IActionResult> PostConfigOptionsParams (string stopName, string direction)
    {
        try
        {
            var configOptions = new ConfigOptions();
            configOptions.Routes.Add(new Models.Route(stopName){
                Direction = direction
            });
            var esp32Service = new ESP32Service(_configuration, _stopService);
            return new JsonResult(esp32Service.GetAllData(configOptions));
        }
        catch (JsonException)
        {
            return BadRequest("Invalid parameters.");
        }
    }

    [HttpGet("publish-message")]
    public async Task<IActionResult> PublishMessage ()
    {
        try
        {
            // Call the static method directly on the type; no instance required
            await _mqttService.Publish_Application_Message();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest("Invalid parameters.");
        }
    }

    [HttpGet("publish-mta")]
    public async Task<IActionResult> PublishMta ()
    {
        try
        {
            // Call the static method directly on the type; no instance required
            await _mqttService.PublishMta();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest("Invalid parameters.");
        }
    }

}
