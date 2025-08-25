using Microsoft.Extensions.DependencyInjection;
using OnAim.Aggregation.Application.Messages.PlayerEvents;
using OnAim.Aggregation.Application.Services;
using OnAim.Aggregation.Application.Services.AggregationConfiguration;
using OnAim.Aggregation.Application.Services.AmountAggregation;
using OnAim.Aggregation.Application.Services.ClientServerEventProducer;
using OnAim.Aggregation.Application.Services.EventHandlers;

namespace OnAim.Aggregation.Application;

public static class ApplicationExtensions
{
    // Register services required in Application layer here.
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAggregationConfigure, AggregationConfigure>();
        services.AddScoped<IAmountAggregator, AmountAggregation>();
        services.AddScoped<IRandomEventProducer, RandomEventProducer>();
        services.AddTransient<PlayerEventsHandler>();
        //services.AddScoped<IRandomEventProducer, RandomEventProducer>();
        return services;
    }
}