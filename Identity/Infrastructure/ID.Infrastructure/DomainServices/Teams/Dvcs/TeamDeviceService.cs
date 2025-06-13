using ClArch.ValueObjects;
using ID.Domain.Abstractions.Services.Teams.Dvcs;
using ID.Domain.Entities.Devices;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Exceptions;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using MyResults;

namespace ID.Infrastructure.DomainServices.Teams.Dvcs;
internal class TeamDeviceService(IIdUnitOfWork uow, Team team, Guid subscriptionId) : ITeamDeviceService
{
    private readonly IIdentityTeamRepo _teamRepo = uow.TeamRepo;

    public Team Team { get; } = team;
    public TeamSubscription Subscription { get; } = team.Subscriptions.First(s => s.Id == subscriptionId);

    //- - - - - - - - - - - - - - - - - - //

    public Task<TeamDevice?> GetDeviceAsync(Guid? dvcId) =>
        Task.FromResult(Subscription.Devices.FirstOrDefault(s => s.Id == dvcId));

    //-----------------------//

    public async Task<GenResult<TeamDevice>> AddDeviceAsync(Name name, DescriptionNullable description, UniqueId uniqueId)
    {
        try
        {
            var dvc = Subscription.AddDevice(name, description, uniqueId);
            await UpdateAndSaveChangesAsync();
            return GenResult<TeamDevice>.Success(dvc);
        }
        catch (DeviceLimitExceededException ex)
        {
            return GenResult<TeamDevice>.BadRequestResult(ex.Message);
        }
    }

    //-----------------------//

    public async Task<GenResult<bool>> RemoveDeviceAsync(Guid? dvcId)
    {
        var dvc = Subscription.Devices.FirstOrDefault(s => s.Id == dvcId);

        if (dvc is null)
            return GenResult<bool>.NotFoundResult(IDMsgs.Error.NotFound<TeamDevice>(dvcId));

        var removeResult = await RemoveDeviceAsync(dvc);
        return GenResult<bool>.Success(removeResult);
    }

    //- - - - - - - - - - - - - - - - - - //

    public async Task<bool> RemoveDeviceAsync(TeamDevice dvc)
    {
        var removed = Subscription.RemoveDevice(dvc);
        if (removed) //Not already moved
            await UpdateAndSaveChangesAsync();

        return removed;
    }

    //-----------------------//

    /// <summary>
    /// Updates the device with the given id
    /// Can only upate Name and Description. If anything else needs to change Remove the device and add a new one
    /// </summary>
    /// <param name="dvcId">Device Identifier</param>
    /// <param name="name">New name</param>
    /// <param name="description">New description</param>
    /// <returns>Result with update device</returns>
    public async Task<GenResult<TeamDevice>> UpdateDeviceAsync(Guid? dvcId, Name name, DescriptionNullable description)
    {
        var dvc = Subscription.Devices.FirstOrDefault(s => s.Id == dvcId);

        if (dvc is null)
            return GenResult<TeamDevice>.NotFoundResult(IDMsgs.Error.NotFound<TeamDevice>(dvcId));

        dvc.Update(name, description);
        await UpdateAndSaveChangesAsync();
        return GenResult<TeamDevice>.Success(dvc);
    }

    //-----------------------//

    public async Task<Team> UpdateAndSaveChangesAsync()
    {
        var updatedTeam = await _teamRepo.UpdateAsync(Team);
        await uow.SaveChangesAsync();
        return updatedTeam;
    }

    //-----------------------//

}//Cls
