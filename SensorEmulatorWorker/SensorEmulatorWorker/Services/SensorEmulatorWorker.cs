using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace SensorProcessor.Services
{
    public class SensorEmulatorWorker : BackgroundService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SensorEmulatorWorker> _logger;
        private readonly Random _random = new Random();
        private readonly IConfiguration _configuration;

        public SensorEmulatorWorker(HttpClient httpClient, ILogger<SensorEmulatorWorker> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string url = _configuration["URI:SensorProcessor"];

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    for (int sensorId = 1; sensorId <= 3; sensorId++)
                    {
                        var sensorData = new
                        {
                            SensorId = sensorId,
                            Value = _random.NextDouble() * 100,
                            Timestamp = DateTime.UtcNow
                        };

                        var json = JsonSerializer.Serialize(sensorData);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync($"{url}/api/sensor/data", content, stoppingToken);

                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation($"Data sent successfully for sensor {sensorId}");
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to send data for sensor {sensorId}. Status: {response.StatusCode}");
                        }
                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in sensor emulator service");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}
