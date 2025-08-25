using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Application.Services.AmountAggregation;

public interface IAmountAggregator
{
    Task<AggregationResult> AggregateAmount(AggregationResult existingResult, AggregationConfig config, decimal amount);
}