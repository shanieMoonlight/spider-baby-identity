using ID.Domain.Utility.Exceptions;
using ID.Email.SMTP.Setup;

namespace ID.Email.SMTP.Utility;

internal class IdEmailSmtpMissingSetupException(string missingProperty)
    : SetupDataException(nameof(IdEmailSmtpOptionsSetup), missingProperty)
{ }


internal class IdEmailSmtpInvalidSetupException(string msg)
    : MyIdException($"{nameof(IdEmailSmtpOptionsSetup)}: {msg}")
{ }
