using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;

namespace ControllerHelpers.Responses;

/// <summary>
/// Represents a RateLimitExceeded (429) response.
/// </summary>
[Description("RateLimitExceeded (429) response")]
public class RateLimitExceededResponse : ObjectResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitExceededResponse"/> class.
    /// </summary>
    /// <param name="value">The value to be returned in the response body.</param>
    public RateLimitExceededResponse(object value) : base(value) =>
        StatusCode = (int)HttpStatusCode.TooManyRequests;
}
