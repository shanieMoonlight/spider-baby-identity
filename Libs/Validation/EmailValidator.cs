using System.Text.RegularExpressions;
using System.Net.Mail;

namespace ValidationHelpers;


/// <summary>
/// Validates strings as properly formatted email addresses.
/// </summary>
/// <remarks>
/// This implementation uses .NET 8's source-generated regex capabilities for improved performance.
/// For additional validation options, consider using System.ComponentModel.DataAnnotations.EmailAddressAttribute
/// or System.Net.Mail.MailAddress for RFC compliance checks.
/// </remarks>
public partial class EmailValidator
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();


    [GeneratedRegex(@"(\.-)|(-\.)|^[-.]|[-.]$")]
    private static partial Regex EmailFinalCheckRegex();

    //-----------------------------------//

    public static bool IsValid(string? email, bool allowNulls = true)
    {
        if (string.IsNullOrWhiteSpace(email))
            return allowNulls;


        try
        {
            email = email.Trim();

            // Use the regex as a quick first filter
            if (!EmailRegex().IsMatch(email))
                return false;

            // The constructor of MailAddress will throw a FormatException
            // if the email address is not in a valid format according to RFCs.
            var mailAddress = new MailAddress(email);


            // 2. Optional: Check for hyphen at the start/end of domain labels
            // mailAddress.Host will be something like "example.com"
            // The regex below checks for hyphens at the beginning or end of a domain label.
            // Example: -example.com, example-.com, example.-com
            if (EmailFinalCheckRegex().IsMatch(mailAddress.Host))
                return false;


            return true;
        }
        catch (Exception)
        {
            // The email string was not in a valid format.
            return false;
        }

    }

}//Cls