namespace ID.OAuth.Facebook.Services;

using System;

internal interface IFacebookClientUtilities
{
    string GenerateAppSecretProof(string userAccessToken);
}//Cls