using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;

namespace ControllerHelpers.Responses;
/// <summary>
/// Represents an HTTP 428 Precondition Required response.
/// </summary>
[Description("HTTP 428 Precondition Required response")]
public class PreconditionRequiredResponse : ObjectResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PreconditionRequiredResponse"/> class.
    /// </summary>
    /// <param name="value">The value to be included in the response body.</param>
    public PreconditionRequiredResponse(object value) : base(value) =>
        StatusCode = (int)HttpStatusCode.PreconditionRequired;
}
