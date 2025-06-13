using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Pagination.Utility.Exceptions;
public class PaginationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationException"/> class.
    /// </summary>
    public PaginationException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PaginationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public PaginationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The SerializationInfo that holds the serialized object data.</param>
    /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
    protected PaginationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}//Cls