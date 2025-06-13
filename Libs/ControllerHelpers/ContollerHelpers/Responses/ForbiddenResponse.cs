using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;

namespace ControllerHelpers.Responses;

/// <summary>
/// Represents a Forbidden (403) response.
/// </summary>
[Description("Forbidden (403) response")]
public class ForbiddenResponse : ObjectResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenResponse"/> class.
    /// </summary>
    /// <param name="value">The value to be returned in the response body.</param>
    public ForbiddenResponse(object value) : base(value) =>
        StatusCode = (int)HttpStatusCode.Forbidden;
}
