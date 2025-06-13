using Microsoft.AspNetCore.Identity;

namespace ID.Domain.Utility.Exceptions;
public class SeedingException(string msg) : MyIdDatabaseException(msg)
{
    public SeedingException(IEnumerable<IdentityError> errors) : this(string.Join($",{Environment.NewLine}", errors.Select(e => e.Description)))
    { }
}