using ESP32_MTA_Feed.Models;
using Microsoft.AspNetCore.Mvc;

namespace ESP32_MTA_Feed.Controllers;

[ApiController]
[Route("[controller]")]

public class MtaFeedController
{


    // [HttpGet(Name = "Test")]
    // public string GetTest()
    // {
    //     return "hi";
    // }
    //
    [HttpGet(Name = "GetFeed")]
    public JsonResult GetFeed()
    {
    
        var client = new HttpClient();
    
        var response = client.GetAsync("https://api-endpoint.mta.info/Dataservice/mtagtfsfeeds/camsys%2Fsubway-alerts.json");
    
        var content2 = response.Result.Content.ReadFromJsonAsync<Root>();
        var list = new List<string>();
        //content2.Result.Entity.ForEach(entity => list.Add(entity.Alert.HeaderText.Translation.Where(translation => translation.Language == "en").Single().Text) );
    
        content2.Result.Entity.ForEach(entity =>
        {
            if (entity.Alert.InformedEntity[0].RouteId == "4")
            {
                list.Add(entity.Alert.HeaderText.Translation.First().Text);
            }
            
        });
    
        
        return new JsonResult(list);
        var content = new JsonResult(response.Result.Content.ReadAsStringAsync());
        
        return content;
        
        // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //     {
        //         Date = DateTime.Now.AddDays(index),
        //         TemperatureC = Random.Shared.Next(-20, 55),
        //         Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //     })
        //     .ToArray();
    }
    
}