namespace ID.Domain.Utility.Exceptions;
public class MyIdDirectoryNotFoundException(string directoryName) : MyIdException($"Directory, {directoryName} was not found ;(")
{}