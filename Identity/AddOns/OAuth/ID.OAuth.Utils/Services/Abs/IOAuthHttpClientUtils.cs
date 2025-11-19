namespace ID.OAuth.Utils.Services.Abs;

public interface IOAuthHttpClientUtils
{
    GenResult<T> MapResponseToResult<T>(HttpResponseMessage response, string provider, string endpoint, string body);
}