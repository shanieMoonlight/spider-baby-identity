using ClArch.ValueObjects;
using ID.Domain.Entities.AppUsers;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Common;
using ID.Domain.Entities.SubscriptionPlans;
using ID.Domain.Entities.SubscriptionPlans.ValueObjects;
using ID.Domain.Entities.Teams.Events;
using ID.Domain.Entities.Teams.Validators;
using ID.Domain.Entities.Teams.ValueObjects;
using ID.Domain.Utility.Exceptions;
using ID.GlobalSettings.Constants;
using ID.GlobalSettings.Setup.Defaults;
using MassTransit;
using MyResults;
using System.ComponentModel.DataAnnotations.Schema;

namespace ID.Domain.Entities.Teams;

/// <summary>
/// A collection of users
/// </summary>
public class Team : IdDomainEntity
{
    /// <summary>
    /// Team name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Team description
    /// </summary>
    public string? Description { get; private set; }

    public TeamType TeamType { get; private set; }

    public int MinPosition { get; private set; } = IdGlobalDefaultValues.MIN_TEAM_POSITION;

    public int MaxPosition { get; private set; } = IdGlobalDefaultValues.MAX_TEAM_POSITION;

    /// <summary>
    /// Maximum number of users in this team
    /// Null means unlimited
    /// </summary>
    public int? Capacity { get; private set; } = null;


    [ForeignKey("Leader")]
    public Guid? LeaderId { get; private set; }

    /// <summary>
    /// Leader/Creator of the team
    /// </summary>
    public AppUser? Leader { get; private set; }

    /// <summary>
    /// App users in this team
    /// </summary>
    private readonly HashSet<AppUser> _members = [];
    public IReadOnlyCollection<AppUser> Members =>
        _members.ToList().AsReadOnly();


    private readonly HashSet<TeamSubscription> _subscriptions = [];
    public IReadOnlyCollection<TeamSubscription> Subscriptions =>
        _subscriptions.ToList().AsReadOnly();


    //------------------------//   


    #region EfCoreCtor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Team() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    private Team(
        Name name,
        DescriptionNullable description,
        TeamType teamType,
        MinTeamPosition minPosition,
        MaxTeamPosition maxPosition)
        : base(NewId.NextSequentialGuid())
    {
        Description = description.Value;
        Name = name.Value;
        TeamType = teamType;
        MaxPosition = maxPosition.Value;
        MinPosition = minPosition.Value;
    }


    //------------------------//   

    /// <summary>
    /// Creates a new customer team with the specified parameters
    /// </summary>
    /// <param name="name">The name for the customer team</param>
    /// <param name="description">Optional description for the team</param>
    /// <param name="capacity">Maximum number of users allowed in the team</param>
    /// <param name="minPosition">Lowest position allowed for team members</param>
    /// <param name="maxPosition">Highest position allowed for team members</param>
    /// <returns>A newly created customer team</returns>
    /// <exception cref="InvalidTeamNameException">Thrown if the name matches reserved system team names (Super or Maintenance)</exception>
    internal static Team CreateCustomerTeam(
        Name name,
        DescriptionNullable description,
        TeamCapacity capacity,
        MinTeamPosition minPosition,
        MaxTeamPosition maxPosition)
    {

        var team = new Team(
            name,
            description,
            TeamType.Customer,
            minPosition,
            maxPosition)
        {
            Capacity = capacity.Value
        };

        team.RaiseDomainEvent(new TeamCreatedDomainEvent(team.Id, team));

        return team;
    }


    //- - - - - - - - - - - - //   

    /// <summary>
    /// Creates a Maintenance team with default settings.
    /// Capacity is set to null (unlimited).
    /// </summary>
    /// <param name="minPosition">Lowest position allowed fro team member</param>
    /// <param name="maxPosition">Highest position allowed fro team member</param>
    /// <returns></returns>
    internal static Team CreateMntcTeam(MinTeamPosition minPosition, MaxTeamPosition maxPosition)
    {
        var team = new Team(
            ClArch.ValueObjects.Name.Create(IdGlobalConstants.Teams.MAINTENANCE_TEAM_NAME),
            DescriptionNullable.Create(IdGlobalConstants.Teams.MAINTENANCE_TEAM_DESCRIPTION),
            TeamType.Maintenance,
            minPosition,
            maxPosition);

        team.RaiseDomainEvent(new TeamCreatedDomainEvent(team.Id, team));
        return team;
    }


    //- - - - - - - - - - - - //     



    /// <summary>
    /// Creates a Super team with default settings.
    /// Capacity is set to null (unlimited).
    /// </summary>
    /// <param name="minPosition">Lowest position allowed fro team member</param>
    /// <param name="maxPosition">Highest position allowed fro team member</param>
    /// <returns></returns>
    internal static Team CreateSuperTeam(MinTeamPosition minPosition, MaxTeamPosition maxPosition)
    {
        var team = new Team(
            ClArch.ValueObjects.Name.Create(IdGlobalConstants.Teams.SUPER_TEAM_NAME),
            DescriptionNullable.Create(IdGlobalConstants.Teams.SUPER_TEAM_DESCRIPTION),
            TeamType.Super,
            minPosition,
            maxPosition);

        team.RaiseDomainEvent(new TeamCreatedDomainEvent(team.Id, team));
        return team;
    }


    //- - - - - - - - - - - - // 


    public Team Update(Name name, DescriptionNullable description)
    {
        Description = description.Value;
        Name = name.Value;
        RaiseDomainEvent(new TeamUpdatedDomainEvent(Id, this));
        return this;
    }

    //- - - - - - - - - - - - //    
    
    /// <summary>
    /// Sets a new leader for the team.
    /// </summary>
    /// <param name="leaderUpdateToken">
    /// Validation token containing the new leader information. 
    /// Obtain this token by calling <see cref="TeamValidators.LeaderUpdate.Validate(Team, AppUser)"/>.
    /// </param>
    /// <returns>A successful GenResult containing the updated team, or a failure result if validation fails.</returns>
    /// <remarks>
    /// The new leader's position will be automatically set to the team's maximum position.
    /// A <see cref="TeamLeaderUpdatedDomainEvent"/> will be raised upon successful completion.
    /// </remarks>
    public GenResult<Team> SetLeader(TeamValidators.LeaderUpdate.Token leaderUpdateToken)
    {

        var oldLeaderId = LeaderId;
        var newLeader = leaderUpdateToken.NewLeader;

        LeaderId = newLeader.Id;
        Leader = newLeader;
        newLeader.UpdatePosition(this, TeamPosition.Create(MaxPosition));

        RaiseDomainEvent(new TeamLeaderUpdatedDomainEvent(Id, this, newLeader.Id, oldLeaderId));
        return GenResult<Team>.Success(this);
    }

    //------------------------//
    
    /// <summary>
    /// Updates the team's position range (minimum and maximum positions for team members).
    /// </summary>
    /// <param name="validationToken">
    /// Validation token containing the new position range. 
    /// Obtain this token by calling <see cref="TeamValidators.PositionRangeUpdate.Validate(Team, PositionRange)"/>.
    /// </param>
    /// <returns>The updated team instance.</returns>
    /// <remarks>
    /// After updating the range, all existing team members' positions will be automatically adjusted 
    /// to ensure they fall within the new valid range. A <see cref="TeamPositionRangeUpdatedDomainEvent"/> 
    /// will be raised upon completion.
    /// </remarks>
    public Team UpdatePositionRange(TeamValidators.PositionRangeUpdate.Token validationToken)
    {
        MinPosition = validationToken.NewPositionRange.Value.Min;
        MaxPosition = validationToken.NewPositionRange.Value.Max;

        EnsureMembersHaveValidTeamPositions();

        RaiseDomainEvent(new TeamPositionRangeUpdatedDomainEvent(Id, MinPosition, MaxPosition));
        return this;
    }

    //- - - - - - - - - - - - //
    
    /// <summary>
    /// Updates a team member's position within the team hierarchy.
    /// </summary>
    /// <param name="updatePositionToken">
    /// Validation token containing the member and their new position. 
    /// Obtain this token by calling <see cref="TeamValidators.MemberPositionUpdate.Validate(Team, AppUser, TeamPosition)"/>.
    /// </param>
    /// <returns>The updated team instance.</returns>
    /// <remarks>
    /// The position will be automatically clamped to the team's valid position range if necessary.
    /// A <see cref="TeamMemberPositionUpdatedDomainEvent"/> will be raised upon completion.
    /// </remarks>
    public Team UpdateMemberPosition(TeamValidators.MemberPositionUpdate.Token updatePositionToken)
    {
        AppUser member = updatePositionToken.Member;
        TeamPosition position = updatePositionToken.ClampedPosition;

        member.UpdatePosition(this, position);

        RaiseDomainEvent(new TeamMemberPositionUpdatedDomainEvent(Id, member.Id, position.Value));
        return this;
    }

    //- - - - - - - - - - - - //

    public Team EnsureMembersHaveValidTeamPositions()
    {
        foreach (var mbr in Members)
        {
            EnsureMemberHasValidTeamPosition(mbr);
        }

        return this;
    }

    //- - - - - - - - - - - - //

    private Team EnsureMemberHasValidTeamPosition(AppUser mbr)
    {
        if (mbr.TeamPosition < MinPosition)
            mbr.UpdatePosition(this, TeamPosition.Create(MinPosition));
        else if (mbr.TeamPosition > MaxPosition)
            mbr.UpdatePosition(this, TeamPosition.Create(MaxPosition));

        return this;
    }

    //------------------------//
    
    /// <summary>
    /// Adds a new member to the team.
    /// </summary>
    /// <param name="additionToken">
    /// Validation token containing the member to add. 
    /// Obtain this token by calling <see cref="TeamValidators.MemberAddition.Validate(Team, AppUser)"/>.
    /// </param>
    /// <returns>The updated team instance.</returns>
    /// <remarks>
    /// The member's team reference will be set, their position will be validated and adjusted if necessary,
    /// and if the team has no leader, the new member will be automatically assigned as the leader.
    /// A <see cref="TeamMemberAddedDomainEvent"/> will be raised upon completion.
    /// </remarks>
    public Team AddMember(TeamValidators.MemberAddition.Token additionToken)
    {
        var member = additionToken.Member;

        // No validation needed - token guarantees correctness
        member.SetTeam(this);
        _members.Add(member);
        EnsureMemberHasValidTeamPosition(member);
        EnsureTeamHasLeader(member);

        RaiseDomainEvent(new TeamMemberAddedDomainEvent(this, member));

        return this;
    }


    //------------------------// 

    private Team EnsureTeamHasLeader(AppUser user)
    {
        var otherMembers = Members
          .Where(u => u.Id != user.Id);

        var currentLeader = otherMembers.FirstOrDefault(m => m.Id == LeaderId);

        if (currentLeader is not null)
            return this;

        LeaderId = user.Id;
        Leader = user;
        user.UpdatePosition(this, TeamPosition.Create(MaxPosition));

        return this;
    }

    //- - - - - - - - - - - - 
    
    /// <summary>
    /// Removes a member from the team.
    /// </summary>
    /// <param name="removalToken">
    /// Validation token containing the member to remove. 
    /// Obtain this token by calling <see cref="TeamValidators.MemberRemoval.Validate(Team, AppUser)"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the member was successfully removed; <c>false</c> if the member was not found in the team.
    /// </returns>
    /// <remarks>
    /// A <see cref="TeamMemberRemovedDomainEvent"/> will be raised only if the removal is successful.
    /// Business rules such as preventing removal of the last member (for certain team types) or 
    /// the team leader are enforced during validation before obtaining the token.
    /// </remarks>
    public bool RemoveMember(TeamValidators.MemberRemoval.Token removalToken)
    {
        AppUser member = removalToken.Member;

        var succeeded = _members.Remove(member);
        if (succeeded)
            RaiseDomainEvent(new TeamMemberRemovedDomainEvent(this, member));
        //else Already Deleted

        return succeeded;
    }

    //------------------------//
    
    /// <summary>
    /// Adds a subscription to the team.
    /// </summary>
    /// <param name="addSubToken">
    /// Validation token containing the subscription plan and optional discount. 
    /// Obtain this token by calling <see cref="TeamValidators.SubscriptionAddition.Validate(Team, SubscriptionPlan, Discount?)"/>.
    /// </param>
    /// <returns>
    /// The created <see cref="TeamSubscription"/> instance. If the subscription already exists, 
    /// the existing instance is returned.
    /// </returns>
    /// <remarks>
    /// A <see cref="TeamSubscriptionAddedEvent"/> will be raised only if a new subscription is created.
    /// Duplicate subscriptions (same plan) will not raise additional events.
    /// </remarks>
    public TeamSubscription AddSubscription(TeamValidators.SubscriptionAddition.Token addSubToken)
    {
        SubscriptionPlan plan = addSubToken.SubscriptionPlan;
        Discount? discount = addSubToken.Discount;
        var sub = TeamSubscription.Create(plan, this, discount);

        var succeeded = _subscriptions.Add(sub);
        if (succeeded)
            RaiseDomainEvent(new TeamSubscriptionAddedEvent(this, sub));
        //else Already Added

        return sub;
    }

    //- - - - - - - - - - - - //
    
    /// <summary>
    /// Removes a subscription from the team.
    /// </summary>
    /// <param name="subRemoveToken">
    /// Validation token containing the subscription to remove. 
    /// Obtain this token by calling <see cref="TeamValidators.SubscriptionRemoval.Validate(Team, TeamSubscription)"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the subscription was successfully removed; <c>false</c> if the subscription was not found.
    /// </returns>
    /// <remarks>
    /// A <see cref="TeamSubscriptionRemovedEvent"/> will be raised only if the removal is successful.
    /// Business rules such as preventing removal of required subscriptions are enforced during 
    /// validation before obtaining the token.
    /// </remarks>
    public bool RemoveSubscription(TeamValidators.SubscriptionRemoval.Token subRemoveToken)
    {
        TeamSubscription sub = subRemoveToken.Subscription;
        var succeeded = _subscriptions.Remove(sub);
        if (succeeded)
            RaiseDomainEvent(new TeamSubscriptionRemovedEvent(this, sub));
        //else Already Deleted

        return succeeded;
    }

    //------------------------// 

    /// <summary>
    /// Ensures that the specified position is within the valid range.
    /// Used by the <see cref="AppUser"/> to validate their team position.
    /// </summary>
    /// <param name="position">The position to validate. If <see langword="null"/>, the maximum position is returned.</param>
    /// <returns>The validated position. Returns <see cref="MinPosition"/> if the specified position is less than the minimum,
    /// <see cref="MaxPosition"/> if it exceeds the maximum, or the original position if it is within the valid range.</returns>
    internal int EnsureValidPosition(int? position)
    {
        if (!position.HasValue)
            return MaxPosition;

        if (position.Value < MinPosition)
            return MinPosition;

        if (position.Value > MaxPosition)
            return MaxPosition;

        return position.Value;
    }

    //------------------------// 

    #region EqualsAndHashcode
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != typeof(Team)) return false;

        var that = (Team)obj;
        return Id == that.Id;

    }

    //- - - - - - - - - - - - //     

    public override int GetHashCode() => HashCode.Combine(Id);
    #endregion

    //------------------------// 

}//Cls
