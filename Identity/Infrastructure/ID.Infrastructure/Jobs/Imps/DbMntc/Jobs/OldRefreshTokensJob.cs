using Hangfire;
using ID.Application.Jobs.DbMntc;
using ID.Application.Utility;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.RefreshTokens;
using LoggingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ID.Infrastructure.Jobs.Imps.DbMntc.Jobs;
internal class OldRefreshTokensJob(IServiceProvider _serviceProvider, ILogger<OldRefreshTokensJob> logger) : AOldRefreshTokensJob
{
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    public override async Task HandleAsync(CancellationToken cancellationToken)
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
            logger.LogException(e, MyIdLoggingEvents.JOBS.DB_MNTC);
        }
    }


}//Cls
