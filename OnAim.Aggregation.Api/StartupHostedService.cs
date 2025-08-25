using OnAim.Aggregation.Application.Repositories;
using OnAim.Aggregation.Application.Services.ClientServerEventProducer;
using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Api;

using Microsoft.Extensions.Hosting;

public sealed class StartupHostedService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<StartupHostedService> _logger;

    public StartupHostedService(
        IServiceProvider services,
        IHostApplicationLifetime lifetime,
        ILogger<StartupHostedService> logger)
    {
        _services = services;
        _lifetime = lifetime;
        _logger = logger;
    }
    public Task StartAsync(CancellationToken _)
    {
        _lifetime.ApplicationStarted.Register(async () =>
        {
            await RunAsync();
        });
        return Task.CompletedTask;
    }

    private async Task RunAsync()
    {
        _logger.LogInformation("Startup seeder invoked");

        using var scope = _services.CreateScope();
        var producer = scope.ServiceProvider.GetRequiredService<IRandomEventProducer>();
        var repo     = scope.ServiceProvider.GetRequiredService<IAggregationConfigRepository>();

        try
        {
            foreach (var cfg in GetConfigSeed())
            {
                _logger.LogInformation("Seeding config {Provider}/{EventType}", cfg.Provider, cfg.EventType);
                await repo.AddOrUpdate(cfg, CancellationToken.None);
            }

            await producer.ProduceRandomEvent(CancellationToken.None);
            _logger.LogInformation("Startup event published.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Startup seeder failed.");
        }
    }
    
    private List<AggregationConfig> GetConfigSeed()
    {
        return new List<AggregationConfig>()
        {
            new()
            {
                Provider = "ClientA",
                Owner = "System" ,
                Rule = new AggregationRule()
                {
                    AwardPoints = 10,
                    PointThreshold = 100
                },
                Filters = new List<FilterClause>()
                {
                    new()
                    {
                        Field = "Amount",
                        Op = FilterOperator.Gt,
                        Value = 5
                    }
                },
                Subscriber = "Aggregation",
                EventType = "players.bet-placed",
                SubscriberKey = "bets-placed-subscriber-key",
                IsEnabled = true
            },
            new()
            {
                Provider = "ClientA",
                Owner = "System" ,
                Rule = new AggregationRule()
                {
                    AwardPoints = 10,
                    PointThreshold = 100
                },
                Filters = new List<FilterClause>()
                {
                    new()
                    {
                        Field = "Amount",
                        Op = FilterOperator.Lt,
                        Value = 30
                    }
                },
                Subscriber = "Aggregation",
                EventType = "players.deposit-made",
                SubscriberKey = "deposit-made-subscriber-key",
                IsEnabled = true
            }
        };
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
