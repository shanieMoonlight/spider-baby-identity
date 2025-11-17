namespace ID.OAuth.Facebook.Services;

using System;

internal interface IFacebookClientUtilities
{
    //string GetBaseUrl();
    string GenerateAppSecretProof(string userAccessToken);
    //UriBuilder BuildEndpointRoute(string endpoint);
}//Cls