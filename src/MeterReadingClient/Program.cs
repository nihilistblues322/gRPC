using MeterReadingClient;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTransient<ReadingGen>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();