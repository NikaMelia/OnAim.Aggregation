using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Application.Services.AggregationConfiguration;

public interface IAggregationConfigure
{
    Task<string> AddAggregationConfig(AggregationConfig config, CancellationToken ct);
}