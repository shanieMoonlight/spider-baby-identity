using System.Text;

namespace MyResults;


public partial class BasicResult
{
    /// <summary>
    /// Represents the status code for the result
    /// </summary>
    public enum ResultStatus
    {
        /// <summary>
        /// The operation was successful
        /// </summary>
        Success,

        /// <summary>
        /// The operation failed with a general error
        /// </summary>
        Failure,

        /// <summary>
        /// The requested resource was not found
        /// </summary>
        NotFound,

        /// <summary>
        /// Authentication is required to access the resource
        /// </summary>
        Unauthorized,

        /// <summary>
        /// The request was malformed or invalid
        /// </summary>
        BadRequest,

        /// <summary>
        /// The authenticated user doesn't have permission to access the resource
        /// </summary>
        Forbidden,

        /// <summary>
        /// Additional conditions must be satisfied before the request can be processed
        /// </summary>
        PreconditionRequired
    }
}