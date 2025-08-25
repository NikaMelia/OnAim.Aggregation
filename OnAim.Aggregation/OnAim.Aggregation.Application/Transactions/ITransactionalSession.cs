using Microsoft.EntityFrameworkCore.Storage;

namespace OnAim.Aggregation.Application.Transactions;

public interface ITransactionalSession
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}