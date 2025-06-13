using ID.Infrastructure.Auth.AppAbs;
using ID.Infrastructure.Persistance.EF.Setup.Options;
using ID.Infrastructure.Setup.Options;
using ID.Infrastructure.Utility;
using ID.Infrastructure.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ID.Infrastructure.Auth.AppImps;

/// <summary>
/// Service for managing external pages.
/// </summary>
public class ExternalPageListService(IOptions<IdInfrastructureOptions> _optionsProvider) : IExternalPageListService
{

    private readonly IdInfrastructureOptions _options = _optionsProvider.Value;

    //--------------------------//

    /// <summary>
    /// Gets the list of external pages.
    /// </summary>
    /// <returns>List of external pages.</returns>
    public List<string> GetExternalPages() =>
        [
            IdInfrastructureConstants.Jobs.DashboardPath,
            _options.SwaggerUrl,
            .._options.ExternalPages
        ];


    //--------------------------//


    /// <summary>
    /// Checks if the request is from an external page.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>True if the request is from an external page, otherwise false.</returns>
    public bool IsFromExternalPage(HttpRequest request) =>
        GetExternalPages().Any(url => request.IsFromSection(url));


}//Cls
