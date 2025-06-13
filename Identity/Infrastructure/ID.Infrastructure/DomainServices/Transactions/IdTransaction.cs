using ID.Domain.Abstractions.Services.Transactions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ID.Infrastructure.DomainServices.Transactions;

internal class IdTransaction(IDbContextTransaction dbContextTransaction) : IIdTransaction
{
    /// <summary>
    ///     Commits all changes made to the Systen in the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    public async Task CommitAsync(CancellationToken cancellationToken = default) => 
        await dbContextTransaction.CommitAsync(cancellationToken);

    //---------------------------------//

    /// <summary>
    ///     Discards all changes made to the database in the current transaction asynchronously.
    /// </summary>
    /// <remarks>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    public async Task RollbackAsync(CancellationToken cancellationToken = default) =>
        await dbContextTransaction.RollbackAsync(cancellationToken);

    //---------------------------------//

    public void Dispose() => dbContextTransaction.Dispose();

}//Cls
