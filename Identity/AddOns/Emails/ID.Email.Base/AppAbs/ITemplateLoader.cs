using MyResults;

namespace ID.Email.Base.AppAbs;
internal interface ITemplateLoader
{
    Task<GenResult<string>> LoadAsync(string templatePath);
}
