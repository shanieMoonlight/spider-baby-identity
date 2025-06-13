namespace ID.Domain.Entities.Teams.Validators;

/// <summary>
/// Marker interface for validation tokens that prove business rules have been validated
/// </summary>
public interface IValidationToken
{
    /// <summary>
    /// The team this validation token applies to
    /// </summary>
    Team Team { get; }

}
