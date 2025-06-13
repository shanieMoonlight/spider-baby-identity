using ID.Domain.Abstractions.PasswordValidation;
using ID.Infrastructure.Setup.Options;
using ID.Infrastructure.Setup.Passwords;
using Microsoft.Extensions.Options;
using StringHelpers;

namespace ID.Infrastructure.Services.PasswordValidation;

/// <summary>
/// Validates passwords based on the settings defined in <see cref="_pwdOptions"/>.
/// </summary>
internal class IdPasswordValidator(IOptions<IdPasswordOptions> _pwdOptionsProvider) : IIdPasswordValidator
{
    private readonly IdPasswordOptions _pwdOptions = _pwdOptionsProvider.Value;


    //-------------------------------------//


    /// <summary>
    /// Validates the specified password based on the criteria defined in <see cref="IdPasswordOptions"/>.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns><c>true</c> if the password meets all the criteria; otherwise, <c>false</c>.</returns>
    public bool Validate(string? password)
    {
        if (password.IsNullOrWhiteSpace())
            return false;

        if (password!.Length < _pwdOptions.RequiredLength)
            return false;

        if (_pwdOptions.RequireLowercase)
            if (!password.Any(char.IsLower))
                return false;

        if (_pwdOptions.RequireUppercase)
            if (!password.Any(char.IsUpper))
                return false;

        if (_pwdOptions.RequireDigit)
            if (!password.Any(char.IsDigit))
                return false;

        if (_pwdOptions.RequireNonAlphanumeric)
            if (password.All(char.IsLetterOrDigit))
                return false;

        return true;
    }

}//Cls
