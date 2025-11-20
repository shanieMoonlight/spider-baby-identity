using ID.Application.Jobs.Abstractions;
using ID.Domain.Entities.Teams.Validators;
using ID.Domain.Repos;
using ID.Domain.Repos.Specs.Teams;
using ID.GlobalSettings.Errors;
using LoggingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;


namespace ID.Application.Jobs.DbMntc;
internal sealed class TeamLeaderMntcJob(IServiceProvider _serviceProvider, ILogger<TeamLeaderMntcJob> logger)
    : AMyIdJobHandler("TEAM_LEADER_MNTC_JOB")
{
    [MyIdDisableConcurrentExecution(timeoutInSeconds: 300)]
    [DisplayName("MyId - Missing team leader job")]
    public override async Task HandleAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IIdUnitOfWork>();

            IIdentityTeamRepo _repo = uow.TeamRepo;
            var teams = await _repo.ListAllAsync(new TeamsWithMissingLeadersSpec());
            foreach (var team in teams)
            {
                var highestPositionMember = team.Members
                    .OrderByDescending(m => m.TeamPosition)
                    .FirstOrDefault();

                if (highestPositionMember != null)
                {
                    var validationResult = TeamValidators.LeaderUpdate.Validate(team, highestPositionMember);
                    if (!validationResult.Succeeded)
                    {
                        logger.LogGenResultFailure(validationResult, IdErrorEvents.Jobs.DbMntc);
                        continue; // Skip this team if validation fails
                    }

                    var validationToken = validationResult.Value!; // Success is non-null
                    team.SetLeader(validationToken);
                }
            }

            await uow.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogException(e, IdErrorEvents.Jobs.DbMntc);
        }
    }


}//Cls
