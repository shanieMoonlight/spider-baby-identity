# ID.OAuth.Amazon — Implementation Plan

Goal: Implement a Login With Amazon (LWA) add-on that mirrors the structure and conventions used by `ID.OAuth.Facebook` so it plugs into the same sign-in/FindOrCreate flows and test harness.

High-level approach
- Use the token introspection (`/auth/o2/tokeninfo`) + profile (`/user/profile`) endpoints (LWA).
- Reuse shared utils from `ID.OAuth.Utils`: `IOAuthHttpClientUtils.MapResponseToResult<T>`, shared `JsonSerializerOptions`, and `GenResult<T>` factories.
- Mirror Facebook add-on structure: HttpService (Imps + Abs), Services (verifier/auth), Setup/DI, models, unit tests.

Decisions
- Use LWA tokeninfo + `/user/profile` (no local JWT validation for now).
- Reuse `IOAuthHttpClientUtils.MapResponseToResult<T>` for consistent error mapping and logging.
- Use `AddMyIdOauthStandardResilienceHandler` (from `ID.OAuth.Utils.HttpClient`) for HttpClient resilience policies.
- Defer provider-specific `FindOrCreateService` implementation — prefer a shared FindOrCreate service in a later change unless provider needs unique behavior.

Step-by-step tasks (current)
1) Project scaffolding (done)
   - `ID.OAuth.Amazon` project exists and has test project with InternalsVisibleTo configured.

2) Options and DI setup (done)
   - `IdOAuthAmazonOptions` created; setup extensions scaffolded (`AddMyIdAmazonOAuth`, `AddAmazonOAuthDI`).
   - `AddMyIdOAuthUtils()` is called to register shared serialization and utils.
   - `AddAmazonOAuthHttpClient()` registers typed `IAmazonHttpClient` with resilience handler and `User-Agent` header.

3) Models (done)
   - `AmazonTokenInfo` and `AmazonUserProfile` added. `ExpiresAt` computed from `expires_in`.

4) HttpService (done)
   - `IAmazonHttpClient` and `AmazonHttpClient` implemented, using injected `JsonSerializerOptions` and `IOAuthHttpClientUtils.MapResponseToResult<T>`.

5) Services (done)
   - `IAmazonAuthenticationService` and `AmazonAuthenticationService` implemented mirroring Facebook patterns and using `GenResult<T>` typed factories.

6) DI wiring (done)
   - Services and HttpClient registered; `AddMyIdOauthStandardResilienceHandler` applied to the typed client.

Remaining tasks
- Validation & DI checklist
  - Add `AmazonOauthSetupOptionsValidator` to validate `ClientId` (required) and `ApiBaseUrl` format optionally.
  - Ensure `AddAmazonOAuthHttpClient()` uses `IOptions<IdOAuthAmazonOptions>` when configuring base address/timeouts (already wired).

- Testing tasks
  - Fix broken Facebook tests (if any regressions): investigate duplicate `TestHttpMessageHandler` and missing `using` statements; ensure `ID.OAuth.Facebook.Tests` compiles and runs.
  - Add Amazon unit tests (in `ID.OAuth.Amazon.Tests`):
    - `AmazonHttpClientTests`: tokeninfo success, invalid (400/401), expired token, 429 rate-limit mapping, deserialization failure, log warnings.
    - `AmazonAuthenticationServiceTests`: verify success, wrong `client_id` -> Unauthorized, user id mismatch -> Unauthorized, expired token -> Unauthorized.
    - Reuse `TestHttpMessageHandler` and `VerifyWarningLogging` from shared test utilities.

- API & behavior notes
  - `tokeninfo` returns `expires_in` (relative seconds) not Unix epoch. Compute `ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expires_in)`.
  - Amazon LWA profile may omit `email` unless `email` scope was requested; treat email as optional/unverified.
  - No API versioning required for Amazon base endpoints; `ApiBaseUrl` option allows overrides if needed.
  - Respect `429` as rate-limited (map to `GenResult<T>.RateLimitExceededResult`), do not retry by default at application level — rely on HttpClient resilience policies.

- FindOrCreateService strategy
  - Recommendation: keep a shared `FindOrCreateService` in a common place (used by all providers) unless a provider needs unique behavior. Implement provider adapter if differences arise. Defer until Amazon sign-in flow wiring is complete.

Deliverables & acceptance criteria
- All unit tests for Facebook and Amazon pass locally.
- `ID.OAuth.Amazon` exposes `AddMyIdAmazonOAuth` setup method and registers HttpClient + services.
- `AmazonAuthenticationService.VerifyAndGetProfileAsync` returns appropriate typed `GenResult` values (Success/Unauthorized/BadRequest/RateLimit) and propagates statuses via `Convert<T>`.
- README/snippet documenting required config keys and scopes added to the add-on project.

Order of implementation (recommended)
1. Add `AmazonOauthSetupOptionsValidator` and wire validation-on-start.
2. Add unit tests for `AmazonHttpClient` and `AmazonAuthenticationService` and get them passing.
3. Fix any Facebook test regressions and ensure all tests in the solution pass.
4. Add README and CI updates.

Notes
- If you want, I can implement validators and the first set of unit tests now. Which should I do next?