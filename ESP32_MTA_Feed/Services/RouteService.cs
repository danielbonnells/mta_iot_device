using ESP32_MTA_Feed.Models;
namespace ESP32_MTA_Feed.Services;

public class RouteService
{
    public RouteService(){}
    private static readonly HttpClient _client = new HttpClient();

    private readonly IConfiguration _configuration;
    public RouteService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public List<string> GetRoutes(IEnumerable<string> ids)
    {
        var list = new List<string>();

        foreach (string id in ids)
        {
            list.AddRange(GetRoute(id));
        }
        
        return list;
    }
    
    public List<string> GetRoute(string id)
    {

        var mtaEndpoint = _configuration.GetSection("MtaApiEndpoints").GetSection("JSON").GetSection("SubwayAlerts").Value;
        var response =
            _client.GetAsync(mtaEndpoint);
        var content = response.Result.Content.ReadFromJsonAsync<Root>();
        var list = new List<string>();
    
        content?.Result?.Entity.ForEach(entity =>
        {
            if (entity.Alert.InformedEntity[0].RouteId == id)
            {
                list.Add(entity.Alert.HeaderText.Translation.First().Text);
            }
        });
        
        return list;
    }
    
}