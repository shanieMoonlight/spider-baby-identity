# ID.OAuth.Amazon — Implementation Plan

Goal: Implement a Login With Amazon (LWA) add-on that mirrors the structure and conventions used by `ID.OAuth.Facebook` so it plugs into the same sign-in/FindOrCreate flows and test harness.

High-level approach
- Use the token introspection (`/auth/o2/tokeninfo`) + profile (`/user/profile`) endpoints (LWA).
- Reuse shared utils from `ID.OAuth.Utils`: `IOAuthHttpClientUtils.MapResponseToResult<T>`, shared `JsonSerializerOptions`, and `GenResult<T>` factories.
- Mirror Facebook add-on structure: HttpService (Imps + Abs), Services (verifier/auth), Setup/DI, models, unit tests.

Step-by-step tasks

1) Project scaffolding
- Ensure `ID.OAuth.Amazon` project exists (done).
- Ensure `ID.OAuth.Amazon.Tests` project exists (done).
- Add `InternalsVisibleTo` attributes in the Amazon csproj for tests and Moq if needed. (DONe, Done and Done!)

2) Options and DI setup
- Create `IdOAuthAmazonOptions` with properties: `ClientId`, `ClientSecret` (optional for LWA), `ApiBaseUrl` (default `https://api.amazon.com/`), `RequestTimeoutSeconds`.
- Add `AmazonOAuthSetupExtensions` with `AddMyIdAmazonOAuth(IConfiguration)` and `AddMyIdAmazonOAuth(Action<IdOAuthAmazonOptions>)` methods mirroring `FacebookOAuthSetupExtensions` and calling:
  - `services.AddMyIdOAuthUtils()` to register `IOAuthHttpClientUtils` and `JsonSerializerOptions`.
  - `services.AddAmazonOAuthHttpClient()` (named HttpClient registration with base address & resilience, see FacebookHttpClientConfiguration).
  - `services.AddAmazonOAuthServices()` to register `IAmazonHttpClient`, `IAmazonAuthenticationService` implementations.

3) Models
- `AmazonTokenInfo` (properties matching tokeninfo response): `client_id`, `expires_in` (int?), `scope`, `user_id`. Add computed `DateTimeOffset? ExpiresAt` set after deserialization: `UtcNow + TimeSpan.FromSeconds(expires_in)`.
- `AmazonUserProfile` matching `user/profile`: `user_id`, `name`, `email`, `postal_code`, etc. Keep email optional and mark as unverified by default unless provider claims otherwise.

4) HttpService
- Add abstraction `IAmazonHttpClient` with methods:
  - `Task<GenResult<AmazonTokenInfo>> GetTokenInfoAsync(string accessToken, CancellationToken ct = default)`
  - `Task<GenResult<AmazonUserProfile>> GetUserProfileAsync(string accessToken, CancellationToken ct = default)`
- Implement `HttpService.Imps.AmazonHttpClient`:
  - Inject `HttpClient`, `IOAuthHttpClientUtils`, `IOptions<IdOAuthAmazonOptions>`, `ILogger<>`, and `JsonSerializerOptions`.
  - `GetTokenInfoAsync` -> GET `auth/o2/tokeninfo?access_token={token}` (use `MapResponseToResult<T>` for non-200).
  - `GetUserProfileAsync` -> GET `user/profile` with `Authorization: Bearer {token}` (use `MapResponseToResult<T>`).
  - Deserialize with injected `JsonSerializerOptions`.

5) Services
- Add `IAmazonAuthenticationService` with `VerifyTokenAsync`, `GetUserProfileAsync`, `VerifyAndGetProfileAsync` similar to Facebook's service signatures.
- Implement `AmazonAuthenticationService`:
  - `VerifyTokenAsync` calls `GetTokenInfoAsync`, checks `client_id` matches configured `ClientId`, calculates expiry from `expires_in`, returns typed `GenResult` status (Unauthorized/BadRequest/RateLimit/etc.).
  - `VerifyAndGetProfileAsync` orchestrates verify -> profile -> id match and uses `Convert<T>` to propagate status where appropriate.

6) DI wiring
- Register the named HttpClient with resilience (mirroring FacebookHttpClientConfiguration) in `AddAmazonOAuthHttpClient()`.
- Register services and validators in `AddAmazonOAuthDI()`.

7) Unit tests
- Add tests in `ID.OAuth.Amazon.Tests` mirroring Facebook tests:
  - `AmazonHttpClientTests` (tokeninfo success, invalid token, expired token, 429 rate-limit, deserialization failure, log warnings).
  - `AmazonAuthenticationServiceTests` (verify+get profile success, invalid token -> Unauthorized status, profile id mismatch -> Unauthorized, expired token -> Unauthorized).
- Reuse `TestHttpMessageHandler`, `VerifyWarningLogging`, shared `JsonSerializerOptions` registration from `ID.OAuth.Utils`.

8) Integration / Manual verification (later)
- Optional: create an integration harness that calls real Amazon LWA endpoints with a test app.

9) Docs & plan
- Add `plan.md` (this file) to `ID.OAuth.Amazon` and add a short README with required configuration keys and scopes (`profile`, `postal_code`, `profile` + `email` if needed).

Implementation notes & gotchas
- `tokeninfo` returns `expires_in` (relative seconds) not Unix epoch. Compute `ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expires_in)`.
- Email on Amazon may be absent unless the `email` scope is granted. Treat email as optional/unverified.
- Reuse `IOAuthHttpClientUtils.MapResponseToResult<T>` for consistent error mapping and logging.
- Respect `429` responses as rate-limited; currently map to `GenResult<T>.RateLimitExceededResult(...)` and do not retry by default. If you later want retries, implement a resilience policy in the named HttpClient.

Order of implementation (minimal incremental commits)
1. Create options + DI setup skeleton (AddAmazonOAuthSetupExtensions, options class)
2. Add models and interfaces (`IAmazonHttpClient`, `IAmazonAuthenticationService`)
3. Implement `AmazonHttpClient` and wire named HttpClient
4. Implement `AmazonAuthenticationService`
5. Add unit tests for HttpClient and Service
6. Polish DI, validators, README, and register tests in CI

If you want I can scaffold the files now (options, models, interfaces, http client, service, setup) and add the first unit tests. Proceed to scaffold?"



 Implement FindOrCreateService for Amazon OAuth  or move to Shared Utils????
What about Amazon Api versioning???
Fix broken tests : FacebookClientRateLimitTests, FacebookHttpClientTests