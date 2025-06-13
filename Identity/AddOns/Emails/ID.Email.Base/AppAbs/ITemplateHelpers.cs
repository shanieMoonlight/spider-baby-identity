namespace ID.Email.Base.AppAbs;

/// <summary>
/// Interface for template operations to allow mocking
/// </summary>
internal interface ITemplateHelpers
{
    /// <summary>
    /// Generates an email details object with a callback URL.
    /// This method reads an email template, replaces placeholders with provided values,
    /// and constructs an email details object containing the email content, subject, and recipient information.
    /// </summary>
    /// <param name="toName">The name of the recipient.</param>
    /// <param name="toAddress">The email address of the recipient.</param>
    /// <param name="callbackUrl">The callback URL to be included in the email.</param>
    /// <param name="templatePath">The relative path to the email template file.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated email details.</returns>
    Task<IEmailDetails> GenerateTemplateWithCallback(
        string toName,
        string toAddress,
        string callbackUrl,
        string templatePath,
        string subject);

    //------------------------------------//

    /// <summary>
    /// Reads a template file and replaces placeholders with values.
    /// This method loads the content of a template file, replaces placeholders with the provided values,
    /// and returns the resulting string.
    /// </summary>
    /// <param name="templatePath">The relative path to the email template file.</param>
    /// <param name="placeholders">A dictionary of placeholder keys and their replacement values.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the processed template content.</returns>
    Task<string> ReadAndReplaceTemplateAsync(string templatePath, Dictionary<string, string> placeholders);
}
