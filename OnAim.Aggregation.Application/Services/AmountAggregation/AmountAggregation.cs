using OnAim.Aggregation.Application.Messages;
using OnAim.Aggregation.Application.Messages.AggregatedEvents;
using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Application.Services.AmountAggregation;

public class AmountAggregation : IAmountAggregator
{
    private readonly IEventBus _eventBus;

    public AmountAggregation(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<AggregationResult> AggregateAmount(AggregationResult existingResult, AggregationConfig config, decimal amount)
    {
        existingResult.AccumulatedAmount += amount;
        if (existingResult.AccumulatedAmount < config.Rule.PointThreshold)
            return existingResult;

        while (existingResult.AccumulatedAmount >= config.Rule.PointThreshold)
        {
            await _eventBus.PublishAsync(new PointsAccumulatedEvent()
            {
                PlayerId = existingResult.PlayerId,
                Provider = existingResult.ProviderService,
                Amount = config.Rule.AwardPoints,
                SubscriberKey = existingResult.SubscriberKey
            });
            existingResult.AccumulatedAmount -= config.Rule.PointThreshold;
            existingResult.TotalPoints += config.Rule.AwardPoints;
        }

        return existingResult;
    }
}