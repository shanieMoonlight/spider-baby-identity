namespace ID.Domain.Utility.Exceptions;

public sealed class InvalidTeamNameException(string name)
    : MyIdException($"The name {name} is already taken")
{ }