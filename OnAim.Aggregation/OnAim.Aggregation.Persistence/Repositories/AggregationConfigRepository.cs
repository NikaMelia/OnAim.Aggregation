using MongoDB.Bson;
using MongoDB.Driver;
using OnAim.Aggregation.Application.Repositories;
using OnAim.Aggregation.Domain.Entities;
using OnAim.Aggregation.Persistence.DbContexts;

namespace OnAim.Aggregation.Persistence.Repositories;

public class AggregationConfigRepository : IAggregationConfigRepository
{
    private readonly IMongoCollection<AggregationConfig> _collection;

    public AggregationConfigRepository(MongoDbContext ctx)
    {
        _collection = ctx.AggregationConfigs;
    }

    public async Task<string> AddOrUpdate(AggregationConfig config, CancellationToken ct)
    {
        config.UpdatedAtUtc = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(x => x.Provider == config.Provider && x.EventType == config.EventType,
            config, new ReplaceOptions { IsUpsert = true }, ct);

        return config.Id.ToString();
    }

    public async Task<AggregationConfig?> GetActiveAsync(
        string providerService,
        string eventType,
        CancellationToken ct = default)
    {
        return await _collection
            .Find(x => x.IsEnabled &&
                       x.Provider == providerService &&
                       x.EventType == eventType)
            .FirstOrDefaultAsync(ct);
    }

}
