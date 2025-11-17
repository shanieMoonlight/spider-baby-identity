using Microsoft.AspNetCore.Http;

namespace ID.OAuth.Facebook.HttpService;
internal class GraphApi
{
    public const string BaseUrl = "https://graph.facebook.com";
    public const string Version  = "v24.0";


    internal class Endpoints
    {

        public static string DebugToken => "debug_token";

    }


}//Cls
