using ID.OAuth.Utils.Abs;
using Microsoft.Extensions.Logging;

namespace ID.OAuth.Utils.Imps;

public class OAuthHttpClientUtils(ILogger<OAuthHttpClientUtils> _logger) : IOAuthHttpClientUtils
{

    public GenResult<T> MapResponseToResult<T>(HttpResponseMessage response, string provider,  string endpoint, string body)
    {
        // Log details to aid debugging
        _logger.LogWarning("{provider} request failed. StatusCode: {StatusCode}, Endpoint: {Endpoint}, Response: {Response}", provider, response.StatusCode, endpoint, body);

        var info = $"StatusCode: {(int)response.StatusCode}. Body: {body}";

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return GenResult<T>.UnauthorizedResult(info);

        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            return GenResult<T>.ForbiddenResult(info);

        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            return GenResult<T>.RateLimitExceededResult($"rate_limited: {info}");

        if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            return GenResult<T>.BadRequestResult(info);

        return GenResult<T>.Failure($"Request failed. {info}");
    }

}//Cls
