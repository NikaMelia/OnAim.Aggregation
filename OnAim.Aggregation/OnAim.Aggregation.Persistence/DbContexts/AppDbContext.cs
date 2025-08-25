using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OnAim.Aggregation.Domain.Entities;

namespace OnAim.Aggregation.Persistence.DbContexts;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<AggregationResult> AggregationResults => Set<AggregationResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }}