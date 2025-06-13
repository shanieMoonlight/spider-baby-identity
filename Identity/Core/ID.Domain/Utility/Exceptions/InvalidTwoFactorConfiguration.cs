using ID.Domain.Models;
using StringHelpers;

namespace ID.Domain.Utility.Exceptions;
public class InvalidTwoFactorConfigurationException(string message) : MyIdException(message)
{
    public InvalidTwoFactorConfigurationException(TwoFactorProvider provider, string missingProperty) 
        : this($"{missingProperty.ToTitleCase()} required for {provider} 2-Factor authentication.")
    {
    }
}
