using MyResults;

namespace ID.Email.Base.LocalAbs;
internal interface ITemplateLoader
{
    Task<GenResult<string>> LoadAsync(string templatePath);
}
