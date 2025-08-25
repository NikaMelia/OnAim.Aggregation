using OnAim.Aggregation.Application.Repositories;
using OnAim.Aggregation.Domain.Entities;
using OnAim.Aggregation.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace OnAim.Aggregation.Persistence.Repositories;

public class AggregationResultsRepository : IAggregationResultsRepository
{
    private readonly AppDbContext _dbContext;

    public AggregationResultsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<AggregationResult?> GetByPlayerId(string playerId, string provider, string eventType)
    {
        return await _dbContext.AggregationResults
            .Where(x => x.PlayerId == playerId && x.ProviderService == provider && x.EventType == eventType)
            .FirstOrDefaultAsync();
    }

    public async Task<AggregationResult> Add(AggregationResult aggregationResult)
    {
        var result = await _dbContext.AggregationResults.AddAsync(aggregationResult);
        await _dbContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task Update(AggregationResult aggregationResult)
    {
        aggregationResult.UpdatedAt = DateTime.UtcNow;
        _dbContext.AggregationResults.Update(aggregationResult);
        await _dbContext.SaveChangesAsync();
    }
}