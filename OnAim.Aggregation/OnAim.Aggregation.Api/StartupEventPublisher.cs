using OnAim.Aggregation.Application.Services.ClientServerEventProducer;

namespace OnAim.Aggregation.Api;

using Microsoft.Extensions.Hosting;

public sealed class StartupEventPublisher : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<StartupEventPublisher> _logger;

    public StartupEventPublisher(
        IServiceProvider services,
        IHostApplicationLifetime lifetime,
        ILogger<StartupEventPublisher> logger)
    {
        _services = services;
        _lifetime = lifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken ct)
    {
        _lifetime.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(async () =>
            {
                using var scope = _services.CreateScope();
                var producer = scope.ServiceProvider.GetRequiredService<IRandomEventProducer>();
                try
                {
                    await producer.ProduceRandomEvent(CancellationToken.None);
                    _logger.LogInformation("Startup event published.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish startup event");
                }
            }, ct);
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
