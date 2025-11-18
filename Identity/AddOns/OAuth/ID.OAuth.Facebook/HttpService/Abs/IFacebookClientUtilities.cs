namespace ID.OAuth.Facebook.HttpService.Abs;

using System;

internal interface IFacebookClientUtilities
{
    string GenerateAppSecretProof(string userAccessToken);
}//Cls