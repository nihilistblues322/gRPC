using Grpc.Core;
using MeterReader.gRPC;
using static MeterReader.gRPC.MeterReadingService;

namespace MeterReader.Services;

public class MeterReadingService(
    IReadingRepository readingRepository,
    ILogger<MeterReadingService> logger)
    : MeterReadingServiceBase
{
    public override async Task<StatusMessage> AddReading(ReadingPacket request, ServerCallContext context)
    {
        logger.LogInformation("Received AddReading request with {ReadingCount} readings. Status: {Status}",
            request.Readings.Count, request.Successful);

        if (request.Successful != ReadingStatus.Success)
        {
            logger.LogWarning("Request status is not successful. Status: {Status}", request.Successful);
            return new StatusMessage
            {
                Message = "Invalid status on reading request",
                Status = ReadingStatus.Failure
            };
        }

        foreach (var reading in request.Readings)
        {
            logger.LogDebug("Processing reading: CustomerId={CustomerId}, Value={Value}, Time={Time}",
                reading.CustomerId, reading.ReadingValue, reading.ReadingTime);

            var readingValue = new MeterReading
            {
                CustomerId = reading.CustomerId,
                Value = reading.ReadingValue,
                ReadingDate = reading.ReadingTime.ToDateTime(),
            };

            readingRepository.AddEntity(readingValue);
        }

        var saveResult = await readingRepository.SaveAllAsync();

        if (saveResult)
        {
            logger.LogInformation("Successfully persisted {ReadingCount} readings to database", request.Readings.Count);
            return new StatusMessage
            {
                Message = "Successfully added to the database",
                Status = ReadingStatus.Success
            };
        }

        logger.LogError("Failed to persist readings to the database");
        return new StatusMessage
        {
            Message = "Failed to add the reading",
            Status = ReadingStatus.Failure
        };
    }
}