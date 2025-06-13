namespace ID.Domain.Utility.Exceptions;
public class MyIdFileNotFoundException(string fileName) : MyIdException($"File, {fileName} was not found ;(")
{}