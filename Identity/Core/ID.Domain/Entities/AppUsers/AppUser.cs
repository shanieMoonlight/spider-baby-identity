using ClArch.ValueObjects;
using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.AppUsers.Events;
using ID.Domain.Entities.AppUsers.OAuth;
using ID.Domain.Entities.AppUsers.ValueObjects;
using ID.Domain.Entities.Avatars;
using ID.Domain.Entities.Common;
using ID.Domain.Entities.Teams;
using ID.Domain.Models;
using ID.Domain.Utility.Exceptions;
using ID.Domain.Utility.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ID.Domain.Entities.AppUsers;

public class AppUser : IdentityUser<Guid>, IIdDomainEventEntity, IIdAuditableDomainEntity
{
    [MaxLength(100)]
    public string? AdministratorUsername { get; private set; }
    [MaxLength(100)]
    public string? AdministratorId { get; private set; }

    public DateTime DateCreated { get; private set; }

    public DateTime? LastModifiedDate { get; private set; }

    //- - - - - - - - - - - - //

    [MaxLength(100)]
    public string FirstName { get; private set; } = string.Empty;

    [MaxLength(100)]
    public string LastName { get; private set; } = string.Empty;

    public IdentityAddress? Address { get; private set; } // This will be configured as an owned property

    /// <summary>
    /// This users avatar
    /// </summary>
    public Avatar? Avatar { get; private set; }

    /// <summary>
    /// The ID of the Team that this user is a member of
    /// </summary>
    public Guid TeamId { get; private set; }

    /// <summary>
    /// The Team that this user is a member of
    /// </summary>
    public Team? Team { get; private set; }

    /// <summary>
    /// The Position/Rank of this user is the Team
    /// </summary>
    public int TeamPosition { get; private set; }

    /// <summary>
    /// Verification token. Used for 2-Factor and other auth stuff
    /// </summary>
    public string? Tkn { get; private set; }

    public DateTime? TknModifiedDate { get; private set; }

    /// <summary>
    /// How will 2 factor be verified
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public TwoFactorProvider TwoFactorProvider { get; private set; } = TwoFactorProvider.Email; //Everyone has an email

    //Delete this????
    /// <summary>
    /// How will 2 factor be verified
    /// </summary>
    public string? TwoFactorKey { get; private set; }


    public OAuthInfo? OAuthInfo { get; set; }


    //public IdRefreshToken? IdRefreshToken{ get; set; }

    ///// <summary>
    ///// App users in this team
    ///// </summary>
    //private readonly HashSet<IdRefreshToken> _tokens = [];
    //public IReadOnlyCollection<IdRefreshToken> IdRefreshTokens =>
    //    _tokens.ToList().AsReadOnly();

    //public ICollection<IdRefreshToken> IdRefreshTokens { get; } = [];


    [NotMapped]
    public string FriendlyName
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(FirstName))
                return FirstName;
            if (!string.IsNullOrWhiteSpace(UserName))
                return UserName!;
            return Email ?? "Unknown name";
        }
    }

    //- - - - - - - - - - - - //

    #region EfCore

    /// <summary>
    /// Used by EfCore
    /// </summary>
    protected AppUser() { }

    #endregion

    protected AppUser(
        Team team,
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        TeamPositionNullable teamPosition)
    {
        UpdateEmailAddress(email);
        UpdatePhone(phone);
        UserName = string.IsNullOrWhiteSpace(username.Value) ? email.Value : username.Value?.Trim();
        FirstName = firstName.Value?.Trim() ?? string.Empty;
        LastName = lastName.Value?.Trim() ?? string.Empty;
        Id = NewId.NextSequentialGuid();

        TeamPosition = team.EnsureValidPosition(teamPosition.Value);
        TeamId = team.Id;
    }

    //- - - - - - - - - - - - //   

    /// <summary>
    /// Create new AppUser. Will default to <see cref="Team.MaxPosition"/> if TeamPosition is null
    /// </summary>
    public static AppUser Create(
        Team team,
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        TeamPositionNullable teamPosition)
    {
        var user = new AppUser(
                team,
                email,
                username,
                phone,
                firstName,
                lastName,
                teamPosition);

        user.SetCreated();
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user));

        return user;
    }

    //- - - - - - - - - - - - //   

    /// <summary>
    /// Create new Oauth AppUser. Will default to <see cref="Team.MaxPosition"/> if TeamPosition is null
    /// </summary>
    public static AppUser CreateOAuth(
        Team team,
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        TeamPositionNullable teamPosition,
        OAuthInfo oauthInfo)
    {
        var user = new AppUser(
                team,
                email,
                username,
                phone,
                firstName,
                lastName,
                teamPosition)
        {
            OAuthInfo = oauthInfo,
            EmailConfirmed = oauthInfo.EmailVerified ?? false,
        };

        user.SetCreated();
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id, user));

        return user;
    }

    //- - - - - - - - - - - - //   

    public AppUser Update(
        EmailAddress email,
        UsernameNullable username,
        PhoneNullable phone,
        FirstNameNullable firstName,
        LastNameNullable lastName,
        TwoFactorProvider? provider,
        bool twoFactoEnabled)
    {
        FirstName = firstName.Value ?? string.Empty;
        LastName = lastName.Value ?? string.Empty;
        UpdateEmailAddress(email);
        UpdatePhone(phone);
        UpdateUsername(username);
        Update2FactoEnabled(twoFactoEnabled);
        if (provider.HasValue)
            Update2FactorProvider(provider.Value);

        RaiseDomainEvent(new UserUpdatedDomainEvent(Id, this));

        return this;
    }

    //- - - - - - - - - - - - //  

    /// <summary>
    /// Add this user to <paramref name="team"/>
    /// Should only be called from Team
    /// </summary>
    internal AppUser SetTeam(Team team)
    {
        Team = team;
        TeamId = team.Id;

        return this;
    }

    //- - - - - - - - - - - - //    

    public AppUser UpdateAddress(IdentityAddress? address)
    {
        Address = address;
        RaiseDomainEvent(new UserAddressUpdatedDomainEvent(this, Address));
        return this;
    }

    //- - - - - - - - - - - - //   

    internal AppUser UpdateEmailAddress(EmailAddress email)
    {
        if (Email is null || !Email.Equals(email.Value, StringComparison.CurrentCultureIgnoreCase))
        {
            Email = email.Value.Trim();
            EmailConfirmed = false;
            RaiseDomainEvent(new UserEmailUpdatedDomainEvent(this));
        }

        return this;
    }

    //- - - - - - - - - - - - //     

    internal AppUser UpdateUsername(UsernameNullable username)
    {
        UserName = string.IsNullOrWhiteSpace(username.Value)
            ? Email
            : username.Value;
        return this;
    }

    //- - - - - - - - - - - - //

    internal AppUser UpdatePhone(PhoneNullable phone)
    {
        if (PhoneNumber != phone.Value)
        {
            PhoneNumber = string.IsNullOrWhiteSpace(phone.Value) ? null : phone.Value?.Trim();
            PhoneNumberConfirmed = false;
            RaiseDomainEvent(new UserPhoneUpdatedDomainEvent(this, PhoneNumber));
        }

        return this;
    }

    //- - - - - - - - - - - - // 

    public AppUser Update2FactoEnabled(bool enabled)
    {
        if (TwoFactorEnabled != enabled)
        {
            TwoFactorEnabled = enabled;
            RaiseDomainEvent(new User2FactorEnableChangedDomainEvent(this, TwoFactorEnabled));
        }

        return this;
    }

    //- - - - - - - - - - - - // 

    internal AppUser UpdatePosition(Team team, TeamPosition newPosition)
    {
        if (team.Id != TeamId)
            throw new NotTeamMemberException<AppUser>(team, this);

        if (TeamPosition != newPosition.Value)
            TeamPosition = team.EnsureValidPosition(newPosition.Value);

        return this;
    }

    //- - - - - - - - - - - - //   

    public AppUser Update2FactorProvider(TwoFactorProvider newProvider)
    {
        if (TwoFactorProvider != newProvider)
        {
            var canChangeResult = this.CanChangeToProvider(newProvider);
            if (!canChangeResult.Succeeded)
                throw new InvalidTwoFactorConfigurationException(canChangeResult.Info);
            TwoFactorProvider = newProvider;
            RaiseDomainEvent(new User2FactorUpdatedDomainEvent(this, newProvider));
        }

        return this;
    }

    //------------------------//  

    public IIdAuditableDomainEntity SetModified(string? username = null, string? userId = null)
    {
        LastModifiedDate = DateTime.Now;
        AdministratorUsername = username;
        AdministratorId = userId;
        return this;
    }

    //- - - - - - - - - - - - //

    public IIdAuditableDomainEntity SetCreated(string? username = null, string? userId = null)
    {
        DateCreated = DateTime.Now;
        AdministratorUsername = username;
        AdministratorId = userId;
        return this;
    }

    //- - - - - - - - - - - - //   

    public AppUser SetTkn(string? tkn)
    {
        Tkn = tkn;
        TknModifiedDate = DateTime.Now;
        return this;
    }

    //- - - - - - - - - - - - //   

    public AppUser SetTwoFactorKey(string key)
    {
        TwoFactorKey = key;
        return this;
    }

    //------------------------//

    #region DomainEvents
    protected readonly List<IIdDomainEvent> _domainEvents = [];

    //- - - - - - - - - - - - //

    protected void RaiseDomainEvent(IIdDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    //- - - - - - - - - - - - //

    public void ClearDomainEvents() => _domainEvents.Clear();

    //- - - - - - - - - - - - //

    public IReadOnlyList<IIdDomainEvent> GetDomainEvents() => [.. _domainEvents];
    #endregion

    //------------------------// 

    #region Equals
    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != typeof(AppUser)) return false;

        var that = (AppUser)obj;
        return Id == that.Id;

    }//Equals

    //- - - - - - - - - - - - // 

    public override int GetHashCode() => HashCode.Combine(Id);
    #endregion


}//Cls
