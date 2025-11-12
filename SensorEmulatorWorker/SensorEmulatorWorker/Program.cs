using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SensorProcessor.Services;
using Microsoft.Extensions.Http;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddHttpClient();
        builder.Services.AddHostedService<SensorProcessor.Services.SensorEmulatorWorker>();

        var host = builder.Build();
        await host.RunAsync();
    }
}