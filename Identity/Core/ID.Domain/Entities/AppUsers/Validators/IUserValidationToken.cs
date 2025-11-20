using ID.Domain.Abstractions.Events;
using ID.Domain.Entities.Teams;

namespace ID.Domain.Entities.AppUsers.Validators;

/// <summary>
/// Marker interface for validation tokens that prove business rules have been validated
/// </summary>
public interface IUserValidationToken
{
    /// <summary>
    /// The user this validation token applies to
    /// </summary>
    AppUser User { get; }

}
