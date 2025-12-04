namespace ESP32_MTA_Feed;

public class MqttPublishScheduler : BackgroundService
{
    private readonly MqttService _publisher;
    private readonly ILogger<MqttPublishScheduler> _logger;
    private readonly TimeSpan _period = TimeSpan.FromSeconds(30);

    // Inject your MtaDataPublisher (and any other dependencies)
    public MqttPublishScheduler(MqttService publisher, ILogger<MqttPublishScheduler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    // This method is the entry point for your continuous background task
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MTA Data Publisher is starting.");
        
        // ⚠️ Initialize the client first (using the pattern we discussed)
        await _publisher.InitializeAsync(); 
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting scheduled publication.");
                
                // Call the core publishing logic
                await _publisher.PublishMta();

                _logger.LogInformation("Scheduled publication complete. Waiting 30 seconds.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during scheduled MQTT publication.");
                // You might add logic here to wait longer or retry connection
            }

            // Wait for the defined period (30 seconds) before the next run
            await Task.Delay(_period, stoppingToken);
        }

        _logger.LogInformation("MTA Data Publisher is stopping.");
        
        // Ensure cleanup is called when the application shuts down
        if (_publisher is IDisposable disposablePublisher)
        {
            disposablePublisher.Dispose();
        }
    }
}