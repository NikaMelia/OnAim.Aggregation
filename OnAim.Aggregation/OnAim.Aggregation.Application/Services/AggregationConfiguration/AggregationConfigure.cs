using OnAim.Aggregation.Application.Repositories;
using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Application.Services.AggregationConfiguration;

public class AggregationConfigure : IAggregationConfigure
{
    private readonly IAggregationConfigRepository _repository;

    public AggregationConfigure(IAggregationConfigRepository repository)
    {
        _repository = repository;
    }

    public Task<string> AddAggregationConfig(AggregationConfig config, CancellationToken ct)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));

        return _repository.AddOrUpdate(config, ct);
    }
}