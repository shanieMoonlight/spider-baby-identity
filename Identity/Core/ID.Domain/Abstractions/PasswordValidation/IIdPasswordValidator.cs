namespace ID.Domain.Abstractions.PasswordValidation;

/// <summary>
/// Validates passwords based on the settings defined in <see cref="IdInfrastructureSettings.PasswordSettings"/>.
/// </summary>
public interface IIdPasswordValidator
{
    /// <summary>
    /// Validates the specified password based on the criteria defined in <see cref="IdInfrastructureSettings.PasswordSettings"/>.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns><c>true</c> if the password meets all the criteria; otherwise, <c>false</c>.</returns>
    bool Validate(string? pssword);

}//Int
