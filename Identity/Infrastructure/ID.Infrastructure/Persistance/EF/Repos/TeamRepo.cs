using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using ID.Infrastructure.Persistance.Abstractions.Repos;
using ID.Infrastructure.Persistance.EF;
using ID.Infrastructure.Persistance.EF.Repos.Abstractions;
using Microsoft.EntityFrameworkCore;
using MyResults;
using Pagination;
using Pagination.Extensions;

namespace ID.Infrastructure.Persistance.EF.Repos;
internal class TeamRepo(IdDbContext db) : AGenCrudRepo<Team>(db), IIdentityTeamRepo
{

    public override async Task<Team> UpdateAsync(Team entity)
    {
        //Ef Change tracker is having problems finding new Devices and throwing an error on SaveChanges
        //If we make sure to add them first the problem goes away.
        await AddNewDevicesToDbAsync(entity);

        Db.Entry(entity).State = EntityState.Modified;

        return entity;
    }

    //- - - - - - - - - - - - - -//

    private async Task AddNewDevicesToDbAsync(Team entity)
    {
        var subIds = entity.Subscriptions.Select(ts => ts.Id) ?? [];
        var originalDevices = await Db.Set<TeamDevice>()
            .Where(d => subIds.Contains(d.SubscriptionId))
            .AsNoTracking()
            .ToListAsync();

        var currentDevices = entity.Subscriptions.SelectMany(ts => ts.Devices) ?? [];

        var newDevices = currentDevices.Except(originalDevices);
        foreach (var dvc in newDevices)
        {
            Db.Add(dvc);
        }
    }

    //-----------------------//

    /// <summary>
    /// Retrieves a paginated list of teams, excluding those of type 'Super'.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="sortList">A collection of sorting criteria.</param>
    /// <param name="filterList">A collection of filtering criteria (optional).</param>
    /// <returns>A paginated list of teams.</returns>
    public override Task<Page<Team>> PageAsync(int pageNumber, int pageSize, IEnumerable<SortRequest> sortList, IEnumerable<FilterRequest>? filterList = null)
    {
        var skip = (pageNumber - 1) * pageSize;

        var data = Db
           .Set<Team>()
           .Where(t => t.TeamType != TeamType.super)
           .AddFiltering(filterList, GetFilteringPropertySelectorLambda)
           .AddEfSorting(sortList, GetSortBuider())
           .AsNoTracking();   //, GetSortingPropertySelectorLambda);

        return Task.FromResult(new Page<Team>(data, pageNumber, pageSize));
    }

    //-----------------------//

    protected override Task<BasicResult> CanDeleteAsync(Team? dbTeam)
    {
        if (dbTeam == null)
            return Task.FromResult(BasicResult.Success());

        if(TeamType.super == dbTeam.TeamType)
            return Task.FromResult(BasicResult.BadRequestResult(IDMsgs.Error.Teams.CANNOT_DELETE_SUPER_TEAM));


        if (TeamType.maintenance == dbTeam.TeamType)
            return Task.FromResult(BasicResult.BadRequestResult(IDMsgs.Error.Teams.CANNOT_DELETE_MNTC_TEAM));


        var mbrCount = dbTeam.Members
            ?.Where(m => m.Id != dbTeam.LeaderId)
            ?.Count() ?? 0;
        var isAre = mbrCount > 1 ? "are" : "is";
        var mbr = mbrCount > 1 ? "members" : "member";

        if (dbTeam.TeamType != TeamType.customer)
            return Task.FromResult(BasicResult.BadRequestResult(IDMsgs.Error.Teams.CAN_ONLY_REMOVE_CUSTOMER_TEAM));

        var result = mbrCount > 0
            ? BasicResult.BadRequestResult($"There {isAre} {mbrCount} {mbr} connected to Team, {dbTeam.Name}. You must delete them or move them to a different Team before deleting this Team.")
            : BasicResult.Success();


        return Task.FromResult(result);
    }

    //-----------------------//

}//Cls
