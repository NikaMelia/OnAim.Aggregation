using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Application.Repositories;

public interface IAggregationResultsRepository
{
    Task<AggregationResult?> GetByPlayerId(string playerId, string provider,
        string eventType);
    Task<AggregationResult> Add(AggregationResult aggregationResult);
    Task Update(AggregationResult aggregationResult);
}