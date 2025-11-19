# ID.OAuth.Amazon — Implementation Plan

Goal: Implement a Login With Amazon (LWA) add-on that mirrors the structure and conventions used by `ID.OAuth.Facebook` so it plugs into the same sign-in/FindOrCreate flows and test harness.

High-level approach
- Use the token introspection (`/auth/o2/tokeninfo`) + profile (`/user/profile`) endpoints (LWA).
- Reuse shared utils from `ID.OAuth.Utils`: `IOAuthHttpClientUtils.MapResponseToResult<T>`, shared `JsonSerializerOptions`, and `GenResult<T>` factories.
- Mirror Facebook add-on structure: HttpService (Imps + Abs), Services (verifier/auth), Setup/DI, models, MediatR Features (commands/handlers/validators), unit tests.

Decisions
- Use LWA `tokeninfo` + `/user/profile` (no local JWT validation for now).
- Reuse `IOAuthHttpClientUtils.MapResponseToResult<T>` for consistent error mapping and logging.
- Use `AddMyIdOauthStandardResilienceHandler` (from `ID.OAuth.Utils.HttpClient`) for HttpClient resilience policies.
- Keep provider-specific `FindOrCreateService` for now (copied from Facebook) and revisit moving to a shared implementation later once provider differences are clearer.

Current status (what's implemented)
- Project scaffolding: `ID.OAuth.Amazon` and test project created with `InternalsVisibleTo`.
- Options & DI: `IdOAuthAmazonOptions`, setup extensions (`AddMyIdAmazonOAuth`, `AddAmazonOAuthDI`) and `AmazonOauthSetupOptionsValidator` implemented.
- Models: `AmazonTokenInfo`, `AmazonUserProfile` added (ExpiresAt computed from `expires_in`).
- Http Service: `IAmazonHttpClient` + `AmazonHttpClient` implemented; typed HttpClient registered with resilience and `User-Agent` header via `AddAmazonOAuthHttpClient()`.
- Services: `IAmazonAuthenticationService` + `AmazonAuthenticationService` implemented.
- FindOrCreate: provider-specific `FindOrCreateService` for Amazon implemented (copied/adapted from Facebook, marks email unverified for now) and registered in DI.
- MediatR Features scaffolded: Amazon SignIn commands, validators and handlers (JWT and cookie flows) created.

Files added (high level)
- `Setup`: `IdOAuthAmazonOptions`, `AmazonOAuthSetupExtensions`, `AmazonOauthSetupOptionsValidator`, `AmazonHttpClientConfiguration`
- `Data`: `AmazonTokenInfo`, `AmazonUserProfile`
- `HttpService`: `IAmazonHttpClient`, `AmazonHttpClient`, `AmazonApi`
- `Services`: `IAmazonAuthenticationService`, `AmazonAuthenticationService`, `FindOrCreateService`, DI extension
- `Features`: `AmazonSignIn` and `AmazonCookieSignIn` DTOs, commands, validators, handlers
- `AccountController` (stub) under `ID.OAuth.Amazon` (needs wiring to commands)

Remaining tasks
- Validation & DI cleanup
  - Consider relaxing `AmazonOauthSetupOptionsValidator` requirement for `ClientSecret` if not needed for LWA.
  - Confirm `ApiBaseUrl` and `AmazonApi.BaseUrl` consistency (base URL currently set; endpoints are relative).

- Testing (deferred until API stabilizes)
  - Add unit tests in `ID.OAuth.Amazon.Tests` (HttpClient + Service + handlers): success, invalid token, expired token, 429 mapping, deserialization failures, logging assertions.
  - Fix any Facebook test regressions if they appear when running full suite.

- Documentation
  - Add README with configuration snippet and required scopes (suggest `profile` and `email` if you need email addresses).

- Future improvements
  - Consider moving `FindOrCreateService` into a shared package (e.g., `ID.OAuth.Common`) if multiple providers can reuse the same logic without dragging domain dependencies into `ID.OAuth.Utils`.
  - Revisit email verification semantics once Amazon profile fields are confirmed; update `FindOrCreateService` to use provider `email_verified` if trustworthy.
  - Add integration tests/harness once credentials and a test app are available.

Acceptance criteria
- DI: `AddMyIdAmazonOAuth` registers HttpClient, serializer options and services.
- Authentication flow: `AmazonAuthenticationService.VerifyAndGetProfileAsync` returns typed `GenResult` statuses and can be consumed by the MediatR handlers.
- Provider Sign-in handlers exist and follow the same JWT/cookie signing patterns as Facebook.
- Tests: will be added after API contracts stabilize.

Next immediate steps I can take for you
1. Wire `AccountController` to send the Amazon MediatR commands.
2. Add unit tests for `AmazonHttpClient` and `AmazonAuthenticationService`.
3. Add a README with configuration examples and required scopes.

1. 
Make validator test more comprehensive .


•	A. Implement full AmazonAuthenticationService tests (recommended first).
•	B. Implement FindOrCreateService tests (requires stubbing app-layer repos/services).
•	C. Implement both.