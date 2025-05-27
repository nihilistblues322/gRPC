using Google.Protobuf.WellKnownTypes;
using MeterReader.gRPC;

namespace MeterReadingClient;

public class ReadingGen
{
    public async Task<ReadingMessage> GenerateAsync(int customerId)
    {
        var reading = new ReadingMessage()
        {
            CustomerId = customerId,
            ReadingTime = Timestamp.FromDateTime(DateTime.UtcNow),
            ReadingValue = new Random().Next(1000)
        };
        return await Task.FromResult(reading);
    }
}