using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Persistence.DbContexts;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<AggregationConfig> AggregationConfigs
        => _database.GetCollection<AggregationConfig>("configs");
}