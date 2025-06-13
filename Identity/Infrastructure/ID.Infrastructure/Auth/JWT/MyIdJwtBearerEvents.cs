using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ID.Infrastructure.Auth.JWT;

internal class MyIdJwtBearerEvents
{
    /// <summary>
    /// Create custom JwtBearerEvents
    /// </summary>
    /// <returns></returns>
    public static JwtBearerEvents CreateCustomEvents() =>
        new()
        {
            OnMessageReceived = mrCtx =>
            {
                //Put your events here
                return Task.CompletedTask;
            }
        };

}//Cls