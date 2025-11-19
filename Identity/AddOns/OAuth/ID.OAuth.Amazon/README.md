# ID.OAuth.Amazon

Login With Amazon (LWA) add-on for MyId. Provides token introspection and user profile fetching using Amazon's LWA endpoints and integrates with the project's MediatR-based sign-in flows.

## What this package does
- Uses Amazon LWA token introspection (`/auth/o2/tokeninfo`) and profile endpoint (`/user/profile`) to verify access tokens and obtain user profile information.
- Registers a typed `HttpClient` with resilience, shared `JsonSerializerOptions`, and provider services.
- Exposes `IAmazonAuthenticationService` and `IAmazonHttpClient` used by MediatR handlers for `AmazonSignIn` and `AmazonCookieSignIn` flows.

## Endpoints
Default base URL used by this add-on:
- `https://api.amazon.com/`

Relative endpoints used:
- `/auth/o2/tokeninfo` — token introspection
- `/user/profile` — user profile

## Configuration
The options type is `IdOAuthAmazonOptions`. The important properties:
- `ClientId` (required)
- `ClientSecret` (optional for current implementation)
- `ApiBaseUrl` (defaults to `https://api.amazon.com/`)
- `RequestTimeoutSeconds` (request timeout in seconds)

Sample `appsettings.json`:

```json
{
  "IdOAuthAmazon": {
    "ClientId": "amzn1.application-oa2-client.appid",
    "ClientSecret": "optional_client_secret",
    "ApiBaseUrl": "https://api.amazon.com/",
    "RequestTimeoutSeconds": 10
  }
}
```

## Registering services (DI)
Call the add-on registration during startup. The project provides `AddMyIdAmazonOAuth` to wire options, HttpClient and services.

```csharp
// in Program.cs or Startup.cs
services.AddMyIdAmazonOAuth(configuration.GetSection("IdOAuthAmazon"));
// Or register manually if needed:
// services.AddAmazonOAuthDI();
```

The options are validated by `AmazonOauthSetupOptionsValidator`. By default `ClientSecret` validation is commented out (optional).

## Required scopes
Typical scopes required to get profile and email:
- `profile`
- `email` (if you need the user's email address)

When configuring the Amazon app, request the appropriate scopes for your flow.

## Usage (example controller)
The add-on integrates with the existing MediatR commands. Example controller usage:

```csharp
[HttpPost("/signin/amazon")]
public async Task<IActionResult> SignInWithAmazon([FromBody] SignInDto dto)
{
    var cmd = new AmazonSignInCmd { AccessToken = dto.AccessToken };
    var result = await _mediator.Send(cmd);
    return result.Succeeded ? Ok(result.Value) : StatusCode(400, result);
}
```

Also a `AmazonCookieSignInCmd` exists for cookie-based sign-in flows.

## Testing
Unit tests exist under `Identity.Tests\ID.OAuth.Amazon.Tests`. Tests cover `AmazonHttpClient` deserialization and basic mappings. Add additional tests for `AmazonAuthenticationService`, `FindOrCreateService` and MediatR handlers as needed.

## Notes
- The validator currently does not require `ClientSecret` (commented out) because this implementation uses token introspection and profile endpoints. Re-enable if you later need confidential client flows.
- Consider documenting any provider-specific behavior (email verification semantics) in the README if you customize `FindOrCreateService`.

If you'd like, I can also add a short example showing how to wire the `AccountController` endpoints to the provided MediatR commands or add a README section for running the unit tests for this add-on.