using ESP32_MTA_Feed.Services;
using ESP32_MTA_Feed.Models;
using MQTTnet;
using MQTTnet.LowLevelClient;
using System.Threading.Tasks;
using TransitRealtime;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic;

namespace ESP32_MTA_Feed;

public class MqttService : IDisposable
{
    public MqttService() { }
    private readonly IConfiguration _configuration;
    private readonly IMqttClient _mqttClient;

     private readonly MqttClientOptions _mqttClientOptions;

    public MqttService(IConfiguration configuration)
    {
        _configuration = configuration;
        var mqttFactory = new MqttClientFactory();
        _mqttClient = mqttFactory.CreateMqttClient();
        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_configuration["MQTT_HOST"], 8883)
            .WithClientId("DataService")
            .WithCredentials(_configuration["MQTT_USER"], _configuration["MQTT_PASS"])
            .WithTlsOptions(new MqttClientTlsOptions
            {
                UseTls = true,
                AllowUntrustedCertificates = false, 
                IgnoreCertificateRevocationErrors = false,
                IgnoreCertificateChainErrors = true,
                TargetHost = _configuration["MQTT_HOST"]
            })
            .WithCleanSession()
            .Build();
        
    }    
    
public async Task InitializeAsync()
{
    try
    {
        // ⭐️ ADD THIS LOG LINE ⭐️
        Console.WriteLine("MqttService Initialization started.");
            
        await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);

        if (!_mqttClient.IsConnected)
        {
            throw new Exception("Failed to connect MQTT client during initialization.");
        }
        
        // ⭐️ ADD THIS SUCCESS LOG LINE ⭐️
        Console.WriteLine("MqttService Initialization successful. Client connected.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"FATAL: MQTT INITIALIZATION FAILED: {ex.Message}");
        throw; // Force the app to crash if the connection fails
    }
}

    public void Dispose()
    {
        // Disconnect and dispose when the service object is shut down.
        // Since Dispose() is synchronous, we must use .Wait() here.
        // This is safe because it's called during application shutdown.
        _mqttClient?.DisconnectAsync().Wait(); 
        _mqttClient?.Dispose();
    }
    public async Task Publish_Application_Message()
    {

        var mqttFactory = new MqttClientFactory();

        using var mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_configuration["MQTT_HOST"], 8883)
            .WithClientId("DataService")
            .WithCredentials(_configuration["MQTT_USER"], _configuration["MQTT_PASS"])
            .WithTlsOptions(new MqttClientTlsOptions
            {
                UseTls = true,
                AllowUntrustedCertificates = false, 
                IgnoreCertificateRevocationErrors = false,
                IgnoreCertificateChainErrors = true,
                TargetHost = _configuration["MQTT_HOST"]

            })
            .WithCleanSession()
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("global/status")
            .WithPayload("This is a test message7")
            .Build();

        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

        await mqttClient.DisconnectAsync();

        Console.WriteLine("MQTT application message is published.");
    }

    public async Task Publish_Stop(string stopId)
    {

        var mqttFactory = new MqttClientFactory();

        using var mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_configuration["MQTT_HOST"], 8883)
            .WithClientId("DataService")
            .WithCredentials(_configuration["MQTT_USER"], _configuration["MQTT_PASS"])
            .WithTlsOptions(new MqttClientTlsOptions
            {
                UseTls = true,
                AllowUntrustedCertificates = false, 
                IgnoreCertificateRevocationErrors = false,
                IgnoreCertificateChainErrors = true,
                TargetHost = _configuration["MQTT_HOST"]

            })
            .WithCleanSession()
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);


        //Stop Logic
        var sS = new StopService(_configuration);
        FeedMessage feed = await sS.GetFeedMessageAsync("Yellow");
        
        GetStopsFromFeed(feed);
        

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("stations/" + stopId)
            .WithPayload("")
            .Build();

        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

        await mqttClient.DisconnectAsync();

        Console.WriteLine("MQTT application message is published.");
    }

    public async Task PublishMta()
    {
        if (!_mqttClient.IsConnected)
        {
           // Log the event for visibility
        Console.WriteLine("MQTT client disconnected. Attempting reconnection...");

        // Use a try-catch for the connection attempt itself
        try
        {
             await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
        }
        catch (Exception ex)
        {
            // If reconnection fails, throw a more specific error or handle gracefully
            throw new InvalidOperationException("MQTT client failed to reconnect.", ex);
        } new InvalidOperationException("MQTT client is not connected. Reconnection logic needed.");
        }

        Dictionary<string, string> feeds = _configuration.GetSection("MtaApiEndpoints:GTFS").Get<Dictionary<string, string>>();
        List<Task> tasks = new ();

        foreach (KeyValuePair<string, string> feed in feeds)
        {
           tasks.Add(Publish_Feeds(feed.Key));
        }

        try
        {
            await Task.WhenAll(tasks);
            // await Task.Delay(500); // Grace period
        }
        catch (AggregateException ae)
        {
            // Check inner exceptions for the actual MQTT/network errors
            foreach (var innerException in ae.InnerExceptions)
            {
                Console.WriteLine($"Error publishing MQTT message: {innerException.Message}");
            }
        }
        
    }
     public async Task Publish_Feeds(string feedName)
    {

        //Stop Logic
        var sS = new StopService(_configuration);
        FeedMessage feed = await sS.GetFeedMessageAsync(feedName);
        
        var routes = GetStopsFromFeed(feed);

        List<Task> tasks = new ();

        foreach (KeyValuePair<string,RouteTopic> route in routes)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("stations/" + route.Key)
            .WithPayload(System.Text.Json.JsonSerializer.Serialize(route.Value.ArrivalTimes))
            .Build();

            tasks.Add(_mqttClient.PublishAsync(applicationMessage, CancellationToken.None));
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (AggregateException ae)
        {
            // Check inner exceptions for the actual MQTT/network errors
            foreach (var innerException in ae.InnerExceptions)
            {
                Console.WriteLine($"Error publishing MQTT message: {innerException.Message}");
            }
        }


        Console.WriteLine("MQTT application message is published.");
    }

    //Helpers

    public Dictionary<string, RouteTopic> GetStopsFromFeed(FeedMessage feed)
    {
       //The key should be Route/StopId/Direction ex: 5/631/N
       var routeDictionary = new Dictionary<string, RouteTopic>();

        foreach (var entity in feed.Entity)
        {
            var routeId = entity?.TripUpdate?.Trip?.RouteId;

            //Console.WriteLine(routeId);

            var stops = entity?.TripUpdate?.StopTimeUpdate;

                if (stops != null && stops.Count > 0)
                {
                    
                    var index = 0;
                    foreach (var stop in stops)
                    {

                    string direction = "";
                    var stopIdNew = stop.StopId;
                    if (stop.StopId.EndsWith("N"))
                    {  
                        direction = "N";
                        stopIdNew = stop.StopId.Substring(0,3);

                    } else
                    {
                        direction = "S";
                        stopIdNew = stop.StopId.Substring(0,3);
                    }

                    var key = $"{routeId}/{stopIdNew}/{direction}";

                    if(!routeDictionary.ContainsKey(key))
                    {
                        var r = new RouteTopic()
                        {
                          RouteId = routeId,
                          StopId = stopIdNew,
                          Direction = direction
                        };

                        routeDictionary.Add(key, r);
                    }  
                    

                        if (stop.Arrival != null)
                        {
                            var date = GeneralService.UnixTimeStampToDateTime(stop.Arrival.Time);
                            routeDictionary[key].ArrivalTimes.Add(date);
                        }
                        index++;
                    }
                }
        }

        return routeDictionary;
        
    }

}

public class RouteTopic
{
    public RouteTopic()
    {
        
    }
    public string? StopName { get; set; }
    public string? RouteId { get; set; }
    public string? StopId { get; set; }
    public string? Direction { get; set; }
    public List<DateTime> ArrivalTimes { get; set; } = [];
    public string GetTopic()
    {
        //route/stopId/direction
        //R/R32/N
        return this.RouteId + "/" + this.StopId + "/" + this.Direction;
    }
}