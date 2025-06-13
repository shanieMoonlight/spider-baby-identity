using Microsoft.EntityFrameworkCore.Storage;

namespace ID.Domain.Abstractions.Services.Transactions;

/// <summary>
/// Interface for handling identity transactions.
/// </summary>
public interface IIdentityTransactionService
{
    //- - - - - - - - - - - - //

    /// <summary>
    /// Begins a new transaction asynchronously.
    /// Use when you want the ability to roll back database operations.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the transaction.</returns>
    Task<IIdTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - //

    /// <summary>
    /// Saves any changes that have occured so far.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    //- - - - - - - - - - - - //
}
