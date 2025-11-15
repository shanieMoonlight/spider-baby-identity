using ID.Domain.Repos;
using ID.Domain.Repos.Transactions;

namespace ID.Persistence.Ef.Transactions;
internal class IdEfTransactionService(IIdUnitOfWork uow) : IIdentityTransactionService
{

    public Task<IIdExecutionStrategy> CreateExecutionStrategyAsync() =>
        uow.CreateExecutionStrategy();

    //------------------------//

    public async Task<IIdTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        await uow.BeginTransactionAsync(cancellationToken);

    //---------------------------------//

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await uow.SaveChangesAsync(cancellationToken);


}//Cls
