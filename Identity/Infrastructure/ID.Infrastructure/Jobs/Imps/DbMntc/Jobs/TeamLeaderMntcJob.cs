using Hangfire;
using ID.Application.Utility;
using ID.Domain.Entities.Teams.Validators;
using ID.GlobalSettings.Jobs.DbMntc;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF.Repos.Specs.Teams;
using LoggingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;


namespace ID.Infrastructure.Jobs.Imps.DbMntc.Jobs;
internal class TeamLeaderMntcJob(IServiceProvider _serviceProvider, ILogger<TeamLeaderMntcJob> logger) : ATeamLeaderMntcJob
{
    [DisableConcurrentExecution(timeoutInSeconds: 300)]
    [DisplayName("MyId - Missing team leader job")]
    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IIdUnitOfWork>();

            IIdentityTeamRepo _repo = uow.TeamRepo;
            var teams = await _repo.ListAllAsync(new TeamsWithMissingLeadersSpec(), cancellationToken);
            foreach (var team in teams)
            {
                var highestPositionMember = team.Members
                    .OrderByDescending(m => m.TeamPosition)
                    .FirstOrDefault();

                if (highestPositionMember != null)
                {
                    var validationResult = TeamValidators.LeaderUpdate.Validate(team, highestPositionMember);
                    if (!validationResult.Succeeded){
                        logger.LogGenResultFailure(validationResult, MyIdLoggingEvents.JOBS.DB_MNTC);
                        continue; // Skip this team if validation fails
                    }

                    var validationToken = validationResult.Value!; // Success is non-null
                    team.SetLeader(validationToken);
                }
            }

            await uow.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogException(e, MyIdLoggingEvents.JOBS.DB_MNTC);
        }
    }


}//Cls
