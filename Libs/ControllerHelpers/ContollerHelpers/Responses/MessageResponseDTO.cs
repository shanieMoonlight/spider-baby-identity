namespace ControllerHelpers.Responses;

/// <summary>
/// Used to send information back to the client
/// <para>JSON: </para>
/// <para>
/// {
///     "message": "Some message for the client"
/// }
/// </para>
/// </summary>
public class MessageResponseDto(string message)
{
    public string Message { get; set; } = message;

    public static MessageResponseDto Generate(string message) => 
        new(message);

}//Cls
