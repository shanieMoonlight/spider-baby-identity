namespace ID.Domain.Utility.Exceptions;

public class CantDeleteException(string property, string message) : MyIdException(message)
{
    public CantDeleteException(string item) : this(item, $"You can't delete this {item}") { }

    public string Property { get; set; } = property;
} 