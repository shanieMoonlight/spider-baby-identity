using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;

namespace ControllerHelpers.Responses;

/// <summary>
/// Represents an Internal Server Error (500) response.
/// </summary>
[Description("Internal Server Error Response")]
public class InternalServerErrorResponse : ObjectResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InternalServerErrorResponse"/> class.
    /// </summary>
    /// <param name="value">The value to be returned in the response body.</param>
    public InternalServerErrorResponse(object value) : base(value) =>
        StatusCode = (int)HttpStatusCode.InternalServerError;
}
