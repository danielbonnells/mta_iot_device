using ESP32_MTA_Feed.Models;

namespace ESP32_MTA_Feed.Services;

public class StopService
{
    public StopService(){}
    private readonly IConfiguration _configuration;
    public StopService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public List<string> GetStops(IEnumerable<string> ids)
    {
        var list = new List<string>();

        foreach (string id in ids)
        {
            list.AddRange(GetStop(id));
        }
        
        return list;
    }
    
    public List<string> GetStop(string id)
    {
        var client = new HttpClient();
        var mtaEndpoint = _configuration.GetSection("MtaApiEndpoints").GetSection("SubwayAlerts").Value;
        var response =
            client.GetAsync(mtaEndpoint);
        var content = response.Result.Content.ReadFromJsonAsync<Root>();
        var list = new List<string>();
    
        content?.Result?.Entity.ForEach(entity =>
        {
            if (entity.Alert.InformedEntity.Any(stop => stop.StopId == id))
            {
                list.Add(entity.Alert.HeaderText.Translation.First().Text);
            }
        });
        
        return list;
    }
}