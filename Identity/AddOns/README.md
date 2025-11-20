# Optional Add-Ons (AddOns)

This folder contains optional add-on libraries that extend the base Identity solution with additional capabilities. Add-ons are implemented as small, focused libraries that follow the same conventions used in the main solution: DI registration via extension methods, `IOptions<T>` for configuration, `GenResult<T>` for rich results, and `MediatR` features for application flows.

This README documents the most relevant add-ons and how to use, test and extend them.

---

## Add-Ons overview

- `ID.PhoneConfirmation` — phone confirmation helpers and controllers.
- `ID.Email.SG`, `ID.Email.SMTP` — email providers and listener implementations.
- `ID.Msg.Twilio` — Twilio messaging add-on.
- OAuth providers (in `ID.OAuth.*`) — social sign-in add-ons. Currently implemented providers:
  - `ID.OAuth.Facebook` — Facebook Graph API support (debug token + profile, sign-in features).
  - `ID.OAuth.Amazon` — Login With Amazon (LWA) support (token introspection + profile, sign-in features)
  - `ID.OAuth.Google` — Google OAuth support (project file included in the repo)

Each OAuth add-on mirrors the same structure and patterns so they can be swapped or used side-by-side.

---

## OAuth add-on structure (common conventions)

Each provider add-on generally contains the following folders and types:

- `Setup` — DI and configuration registration, e.g. `AddMyIdFacebookOAuth`, `AddMyIdAmazonOAuth`.
  - Options type: `IdOAuth{Provider}Options` (bound via `IOptions<T>`).
  - Setup validator: `*SetupOptionsValidator` implements `IValidateOptions<T>` and runs on-start validation.

- `HttpService`
  - `Abs` — interface like `IFacebookHttpClient`, `IAmazonHttpClient`.
  - `Imps` — typed `HttpClient` implementation (`FacebookHttpClient`, `AmazonHttpClient`) using shared `JsonSerializerOptions` and `IOAuthHttpClientUtils` for consistent error mapping.
  - `*Api` files with base URLs/endpoints.

- `Services` — higher-level provider services that verify tokens and fetch profiles (`IAmazonAuthenticationService`, `IFacebookAuthenticationService`).

- `Features` — MediatR commands/handlers for sign-in flows (JWT and cookie flows), DTOs, and FluentValidation validators.

- `Tests` — unit tests mirror implementation folders. Tests use xUnit + Moq + Shouldly.

---

## How to register an OAuth add-on (DI)

Add an OAuth provider during application startup (e.g., in `Program.cs` or your composition root). There are two overloads for adding providers:

- Configure from `IConfiguration`:

```csharp
services.AddMyIdAmazonOAuth(Configuration, sectionName: "AmazonOAuth");
// or for Facebook
services.AddMyIdFacebookOAuth(Configuration, "FacebookOAuth");
```

- Configure with inline options:

```csharp
services.AddMyIdAmazonOAuth(opts => {
    opts.ClientId = "your-amazon-client-id";
    opts.ClientSecret = "your-amazon-client-secret"; // optional for some providers
    opts.ApiBaseUrl = "https://api.amazon.com/"; // optional override
    opts.RequestTimeoutSeconds = 30;
});
```

The `AddMyId{Provider}OAuth` helper will:
- Register provider options with validate-on-start using `*SetupOptionsValidator`.
- Register the typed `HttpClient` with resilience (`AddMyIdOauthStandardResilienceHandler`).
- Register `IAmazonHttpClient` / `IFacebookHttpClient` and the higher-level authentication service.
- Register MediatR handlers and FluentValidation validators from the provider assembly.

---

## Sample configuration keys (appsettings.json)

Add a configuration section for the provider, e.g. `appsettings.json`:

```json
"AmazonOAuth": {
  "ClientId": "amzn1.application-oa2-client.xxxxx",
  "ClientSecret": "<secret-if-needed>",
  "ApiBaseUrl": "https://api.amazon.com/",
  "RequestTimeoutSeconds": 30
}

"FacebookOAuth": {
  "AppId": "<facebook-app-id>",
  "AppSecret": "<facebook-app-secret>",
  "GraphApiBaseUrl": "https://graph.facebook.com",
  "GraphApiVersion": "v18.0",
  "RequestTimeoutSeconds": 30
}
```

Notes:
- `ClientSecret` may be optional for some introspection flows (e.g., LWA) — check provider docs.
- `ApiBaseUrl` / `GraphApiBaseUrl` allow overriding endpoints for testing or future changes.

---

## Required scopes and API notes

- Amazon (LWA)
  - Token introspection: `POST /auth/o2/tokeninfo` (we use `GET` query form for convenience) and `/user/profile` to fetch limited profile fields.
  - Recommended scopes: `profile` and `postal_code` and `email` if you need the user's email. Email may be missing unless `email` scope requested.
  - `tokeninfo.exp` may be returned as relative seconds — `AmazonTokenInfo.ExpiresAt` is computed from `expires_in`.

- Facebook
  - `debug_token` endpoint for token introspection + `/me` for profile with `fields`.
  - Email might be optional depending on requested scopes.

- Google
  - The Google add-on in this repository targets `.NET 8` and includes `Google.Apis.Auth` and the ASP.NET Core Google authentication package in its project file. See `Identity\AddOns\OAuth\ID.OAuth.Google\ID.OAuth.Google.csproj` for project-level details (it references core application projects and exposes internal visibility to test projects). Use `Google.Apis.Auth` helpers to validate ID tokens and fetch profile information when using Google Sign-In.

Always consult provider docs for the exact endpoints and field shapes.

---

## Tests

- Add-on unit tests are under `Identity\Tests\ID.OAuth.{Provider}.Tests` and follow the implementation structure.
- Tests use an in-memory `HttpMessageHandler` (`TestHttpMessageHandler`) to simulate provider responses and `IOAuthHttpClientUtils` is mocked for error mapping.
- Use `UnixEpochSecondsJsonConverter` from `ID.OAuth.Utils.Serialization` when dealing with epoch timestamps in tests.

If you add/modify provider models, add deserialization tests that verify mapping from sample JSON payloads.

---

## Find-or-Create strategy

Each add-on currently contains a provider-specific `FindOrCreateService` that maps a provider profile to application `AppUser` registration calls. This keeps provider quirks (e.g., how trustworthy `email_verified` is) isolated. If many providers converge to a common shape, consider extracting a shared implementation into `ID.OAuth.Common`.

File of interest: `ID.OAuth.{Provider}.Services.Imps.FindOrCreateService`.

---

## Contributing / Extending

- Follow the repository conventions in `.github/copilot-instructions.md` (C# 12, .NET 8, DI patterns, tests with Moq/Shouldly/xUnit).
- Add MediatR features under each provider's `Features` folder for sign-in flows and validations.
- Add unit tests for any new logic. Keep tests small and focused.
- Avoid committing secrets. Use local configuration or environment variables for testing against live providers.

---

## Where to look next (useful files)

- `Identity\AddOns\OAuth\ID.OAuth.Amazon\plan.md` — implementation plan and remaining tasks for Amazon add-on.
- `Identity\AddOns\OAuth\ID.OAuth.Amazon` — Amazon add-on source and tests.
- `Identity\AddOns\OAuth\ID.OAuth.Facebook` — Facebook add-on source and tests.
- `Identity\AddOns\OAuth\ID.OAuth.Utils` — shared utilities (serializers, HttpClient helpers).
- `Identity\AddOns\OAuth\ID.OAuth.Google\ID.OAuth.Google.csproj` — project file for the Google add-on (references `Google.Apis.Auth`, `MediatR`, and `Microsoft.AspNetCore.Authentication.Google`; targets `net8.0` and exposes internals to tests).

---

If you'd like, I can also add a short example `README` inside each provider folder (`ID.OAuth.Amazon/README.md`) that contains a smaller provider-specific configuration and sample curl calls for manual testing.

