using StringHelpers;

namespace ID.Application.Setup;

public class IdApplicationOptions
{
    private string? _fromAppHeaderKey = IdApplicationDefaultValues.FROM_MOBILE_APP_KEY;
    /// <summary>
    /// Key for FROM_MOBILE_APP header. 
    /// Http Header Key to use to indicae that the request is from a mobile app.
    /// Default = <inheritdoc cref=" IdApplicationDefaultValues.FROM_MOBILE_APP_KEY"/>. 
    /// </summary>
    public required string? FromAppHeaderKey
    {
        get => _fromAppHeaderKey;
        set => _fromAppHeaderKey = value.IsNullOrWhiteSpace() 
            ? IdApplicationDefaultValues.FROM_MOBILE_APP_KEY
            : value;
    }


    private string? _fromAppHeaderValue;
    /// <summary>
    /// The Value that should match the value sent by the MobileApp
    /// </summary>
    public required string? FromAppHeaderValue
    {
        get => _fromAppHeaderValue;
        set => _fromAppHeaderValue = value;
    }



}//Cls
