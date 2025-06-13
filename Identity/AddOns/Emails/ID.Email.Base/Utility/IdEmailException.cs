using ID.Domain.Utility.Exceptions;

namespace ID.Email.Base.Utility;
public class IdEmailException(string fileName) : MyIdException($"File, {fileName} was not found ;(")
{ }