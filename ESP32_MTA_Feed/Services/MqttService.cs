using MQTTnet;
using MQTTnet.LowLevelClient;
using System.Threading.Tasks;

namespace ESP32_MTA_Feed;

public class MqttService
{
    public MqttService() { }
    private readonly IConfiguration _configuration;

    public MqttService(IConfiguration configuration)
    {
        _configuration = configuration;
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

}