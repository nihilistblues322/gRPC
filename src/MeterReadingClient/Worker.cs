using Grpc.Net.Client;
using MeterReader.gRPC;

namespace MeterReadingClient;

public class Worker(ILogger<Worker> logger, ReadingGen gen, IConfiguration configuration) : BackgroundService
{
    private int _customerId = configuration.GetValue<int>("CustomerId");
    private string _serviceUrl = configuration["ServiceUrl"]!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = GrpcChannel.ForAddress(_serviceUrl);
        var client = new MeterReadingService.MeterReadingServiceClient(channel);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var packet = new ReadingPacket()
            {
                Successful = ReadingStatus.Success
            };

            for (var i = 0; i < 5; i++)
            {
                var reading = await gen.GenerateAsync(_customerId);
                packet.Readings.Add(reading);
            }

            var status = client.AddReading(packet);
            if (status.Status == ReadingStatus.Success)
            {
                logger.LogInformation("Successfully called grpc");
            }
            else
            {
                logger.LogError("Failed grpc");
            }
        }

        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
    }
}