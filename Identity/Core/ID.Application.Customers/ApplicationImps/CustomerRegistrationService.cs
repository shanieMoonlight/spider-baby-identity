using ClArch.ValueObjects;
using ID.Application.Customers.Abstractions;
using ID.Application.Customers.Dtos.User;
using ID.Domain.Abstractions.Services.Teams;
using ID.Domain.Abstractions.Services.Teams.Subs;
using ID.Domain.AppServices.Abs;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams;
using ID.Domain.Utility.Messages;
using MyResults;

namespace ID.Application.Customers.ApplicationImps;
internal class IdCustomerRegistrationService(IIdentityTeamManager<AppUser> _teamMgr, ITeamBuilderService _teamBuilder) 
    : IIdCustomerRegistrationService
{

    public Task<GenResult<AppUser>> Register_NoPwd_Async(RegisterCustomer_NoPwdDto dto, CancellationToken cancellationToken) =>
        Register_NoPwd_Async(
            EmailAddress.Create(dto.Email),
            UsernameNullable.Create(dto.Username),
            PhoneNullable.Create(dto.Phone),
            FirstNameNullable.Create(dto.FirstName),
            LastNameNullable.Create(dto.LastName),
            TeamPositionNullable.Create(dto.TeamPosition),
            dto.SubscriptionPlanId,
            cancellationToken);

    //- - - - - - - - - - - - - - - - - - -//

    public async Task<GenResult<AppUser>> Register_NoPwd_Async(
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        TeamPositionNullable position,
        Guid? subscriptionPlanId = null,
        CancellationToken cancellationToken = default)
    {
        var teamName = GetTeamName(username.Value, email.Value);
        var team = await RegisterTeamAsync(teamName, cancellationToken);

        var newUser = AppUser.Create(
            team,
            email,
            username,
            phone,
            firstName,
            lastName,
            position);

        var createResult = await _teamMgr.RegisterMemberAsync(team, newUser);

        if (!createResult.Succeeded)
            return createResult.Convert<AppUser>();

        newUser = createResult.Value!;

        if (subscriptionPlanId is null)
            return GenResult<AppUser>.Success(newUser);

        var addSubResult = await AddSubscriptionToTeam(team, subscriptionPlanId.Value);

        await _teamMgr.UpdateAsync(team);
        return addSubResult.Convert(newUser);
    }

    //-------------------------------------//

    public Task<GenResult<AppUser>> RegisterAsync(RegisterCustomerDto dto, CancellationToken cancellationToken) =>
        RegisterAsync(
            EmailAddress.Create(dto.Email),
            UsernameNullable.Create(dto.Username),
            PhoneNullable.Create(dto.Phone),
            FirstNameNullable.Create(dto.FirstName),
            LastNameNullable.Create(dto.LastName),
            Password.Create(dto.Password),
            ConfirmPassword.Create(dto.ConfirmPassword),
            TeamPositionNullable.Create(dto.TeamPosition),
            dto.SubscriptionPlanId,
            cancellationToken);

    //- - - - - - - - - - - - - - - - - - -//

    public async Task<GenResult<AppUser>> RegisterAsync(
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        Password password,
        ConfirmPassword confirmPassword,
        TeamPositionNullable position,
        Guid? subscriptionPlanId = null,
        CancellationToken cancellationToken = default)
    {
        if (confirmPassword != password)
            return GenResult<AppUser>.BadRequestResult(IDMsgs.Error.Authorization.NON_MATCHING_PASSOWRDS);


        var teamName = GetTeamName(username.Value, email.Value);
        var team = await RegisterTeamAsync(teamName, cancellationToken);

        var newUser = AppUser.Create(
            team,
            email,
            username,
            phone,
            firstName,
            lastName,
            position);

        var createResult = await _teamMgr.RegisterMemberWithPasswordAsync(team, newUser, password.Value);

        if (!createResult.Succeeded)
            return createResult.Convert<AppUser>();

        newUser = createResult.Value!;

        if (subscriptionPlanId is null)
            return GenResult<AppUser>.Success(newUser);

        var addSubResult = await AddSubscriptionToTeam(team, subscriptionPlanId.Value);
        if (!addSubResult.Succeeded)
            return addSubResult.Convert<AppUser>();

        await _teamMgr.UpdateAsync(team);
        return addSubResult.Convert(newUser);
    }

    //-------------------------------------//

    public async Task<GenResult<AppUser>> RegisterOAuthAsync(
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        TeamPositionNullable position,
        OAuthInfo oAuthInfo,
        Guid? subscriptionPlanId,
        CancellationToken cancellationToken = default)
    {

        var teamName = GetTeamName(username.Value, email.Value);
        var team = await RegisterTeamAsync(teamName, cancellationToken);

        var newUser = AppUser.CreateOAuth(
            team,
            email,
            username,
            phone,
            firstName,
            lastName,
            position,
            oAuthInfo);

        var createResult = await _teamMgr.RegisterMemberAsync(team, newUser);

        if (!createResult.Succeeded)
            return createResult.Convert<AppUser>();

        newUser = createResult.Value!;

        if (subscriptionPlanId is null)
            return GenResult<AppUser>.Success(newUser);

        var addSubResult = await AddSubscriptionToTeam(team, subscriptionPlanId.Value);

        if(!addSubResult.Succeeded)
            return addSubResult.Convert<AppUser>();

        await _teamMgr.UpdateAsync(team);
        return addSubResult.Convert(newUser);
    }

    //-------------------------------------//

    private async Task<GenResult<Team>> AddSubscriptionToTeam(Team team, Guid subPlanId, int discount = 0)
    {
        var getServiceResult = await _teamMgr.GetSubscriptionServiceAsync(team);
        if (!getServiceResult.Succeeded)
            return getServiceResult.Convert<Team>();

        ITeamSubscriptionService service = getServiceResult.Value!;

        var addResult = await service.AddSubscriptionAsync(subPlanId, Discount.Create(discount));

        return addResult.Convert(s => service.Team);
    }

    //- - - - - - - - - - - - - - - - - - -//

    private async Task<Team> RegisterTeamAsync(string name, CancellationToken cancellationToken)
    {
        Team cusTeam = _teamBuilder.CreateCustomerTeam(
         Name.Create(name),
         DescriptionNullable.Create(null));

        return await _teamMgr.AddTeamAsync(cusTeam, cancellationToken);
    }

    //- - - - - - - - - - - - - - - - - - -//

    private static string GetTeamName(string? username, string email) =>
        "Team_" + (string.IsNullOrWhiteSpace(username) ? email : username);


    //-------------------------------------//

}
