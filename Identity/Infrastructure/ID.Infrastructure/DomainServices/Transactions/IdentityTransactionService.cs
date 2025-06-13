using ID.Domain.Abstractions.Services.Transactions;
using ID.Infrastructure.Persistance.Abstractions.Repos;

namespace ID.Infrastructure.DomainServices.Transactions;
internal class IdentityTransactionService(IIdUnitOfWork uow) : IIdentityTransactionService
{
    public async Task<IIdTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        await uow.BeginTransactionAsync(cancellationToken);

    //---------------------------------//

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await uow.SaveChangesAsync(cancellationToken);

    //---------------------------------//

}//Cls
