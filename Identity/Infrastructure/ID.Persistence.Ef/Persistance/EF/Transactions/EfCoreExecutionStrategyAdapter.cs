using ID.Domain.Repos.Transactions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ID.Infrastructure.DomainServices.Transactions;

internal class EfCoreExecutionStrategyAdapter(IExecutionStrategy strategy) : IIdExecutionStrategy
{
    private readonly IExecutionStrategy _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

    //---------------------------------//

    public Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        // EF Core's IExecutionStrategy.ExecuteAsync passes the DbContext as the first parameter to the delegate.
        // Map that signature to the simpler Func<CancellationToken, Task<TResult>> that the rest of the code expects.
        return _strategy.ExecuteAsync<object, TResult>(
            state: null,
            operation: (dbContext, state, ct) => operation(ct),
            verifySucceeded: null,
            cancellationToken: cancellationToken);
    }

    //---------------------------------//

    public Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        return _strategy.ExecuteAsync<object, object>(
            state: null,
            operation: async (dbContext, state, ct) =>
            {
                await operation(ct).ConfigureAwait(false);
                return null;
            },
            verifySucceeded: null,
            cancellationToken: cancellationToken);
    }

}//Cls