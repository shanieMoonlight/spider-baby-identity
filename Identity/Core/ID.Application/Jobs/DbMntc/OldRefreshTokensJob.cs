using ID.Application.Jobs.Abstractions;
using ID.Domain.Repos;
using ID.GlobalSettings.Errors;
using ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;
using LoggingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ID.Application.Jobs.DbMntc;
internal sealed class OldRefreshTokensJob(IServiceProvider _serviceProvider, ILogger<OldRefreshTokensJob> logger)
 : AMyIdJobHandler("OLD_REFRESH_TOKENS_JOB")
{
    [MyIdDisableConcurrentExecution(300)]
    public async Task HandleAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IIdUnitOfWork>();
            IIdentityRefreshTokenRepo _repo = uow.RefreshTokenRepo;

            await _repo.RemoveRangeAsync(RefreshTokenExpiredSpec.Create());
            await uow.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.Jobs.DbMntc);
        }
    }


}//Cls
