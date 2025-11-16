using System.Threading.Tasks;
using ID.Email.Base.Models;

namespace ID.Email.Base.LocalAbs;

/// <summary>
/// Interface for generating email details from specs.
/// </summary>
public interface IEmailDetailsTemplateGenerator
{
    /// <summary>
    /// Generate an email details object from a provided spec. The generator supplies shared services/options to the spec.
    /// </summary>
    Task<IEmailDetails> GenerateFromSpecAsync(IEmailSpec spec);
}
