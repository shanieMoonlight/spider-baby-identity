namespace ID.Domain.Repos.Transactions;
public interface IIdExecutionStrategy
{

    /// <summary>
    /// Executes an async operation under the execution strategy and returns a result.
    /// </summary>
    Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an async operation under the execution strategy that returns no result.
    /// </summary>
    Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);

}//Int
