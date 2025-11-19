namespace ID.OAuth.Amazon.HttpService;
internal class AmazonApi
{
    public const string BaseUrl = "https://api.amazon.com";
    //public const string Version  = "v24.0";  TODO: Decide if Amazon API versioning is needed is 'o2'  the version ???


    internal class Endpoints
    {

        public static string TokenInfo => "tokeninfo";
        public static string UserProfile => "user/profile";

    }


}//Cls
