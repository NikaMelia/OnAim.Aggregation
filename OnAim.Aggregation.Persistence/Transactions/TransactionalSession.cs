using System.Data.Common;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;
using OnAim.Aggregation.Application.Transactions;
using OnAim.Aggregation.Persistence.DbContexts;

namespace OnAim.Aggregation.Persistence.Transactions;

public class TransactionalSession : ITransactionalSession
{
    private readonly AppDbContext _dbContext;
    private readonly ICapPublisher _capPublisher;

    public TransactionalSession(AppDbContext dbContext, ICapPublisher capPublisher)
    {
        _dbContext = dbContext;
        _capPublisher = capPublisher;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        return await _dbContext.Database.BeginTransactionAsync(_capPublisher, autoCommit: false, ct);
    }
}