using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnAim.Aggregation.Persistence.DbContexts;

namespace OnAim.Aggregation.Persistence;

public static class ApplicationExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        var mongoConnection = configuration["ConnectionStrings:Mongo"];
        var mongoDbName     = configuration["ConnectionStrings:Database"];
        services.AddSingleton(new MongoDbContext(mongoConnection!, mongoDbName!));

        return services;
    }
}