using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Application.Repositories;

public interface IAggregationConfigRepository
{
    Task<string> AddOrUpdate(AggregationConfig config, CancellationToken ct);
    Task<AggregationConfig?> GetActiveAsync(
        string providerService,
        string eventType,
        CancellationToken ct = default);

}