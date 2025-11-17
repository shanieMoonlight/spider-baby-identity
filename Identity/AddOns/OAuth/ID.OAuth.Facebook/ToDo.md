# ID.OAuth.Facebook — To Do

This file lists recommended follow-up tasks prioritized by importance. Tackle them one-by-one and mark items off as completed.

## High priority

1. Add unit tests for core services
   - Targets: `FacebookClient`, `FacebookClientUtilities`, `FacebookTokenVerifier`.
   - Details: mock `HttpMessageHandler` to return known JSON for `debug_token` and `/me`. Assert parsing and GenResult values.
   - Reason: prevents regressions and verifies parsing/edge cases.


4. Add combined verification+profile helper
   - Targets: `FacebookTokenVerifier` (add `VerifyAndGetProfileAsync` or similar).
   - Details: call `GetDebugTokenAsync`, validate `app_id`, `user_id`, expiry, then fetch `/me`, verify `profile.Id` matches and return both token result and profile.
   - Reason: centralizes security checks and simplifies sign-in handlers.

5. Ensure all public methods return `GenResult<T>` consistently
   - Targets: `FacebookClient`, other public surfaces.
   - Details: convert any `null` returns to `GenResult<T>.Failure(...)` and include HTTP status/body information where appropriate.

6. Email verification policy
   - Targets: `FindOrCreateService`, `FacebookUserProfile` model.
   - Details: treat email as unverified unless provider explicitly provides a verified flag. Update `FindOrCreateService` to set email verified = false by default and trigger your normal verification flow.

## Medium priority

7. IOptions validation for `IdOAuthFacebookOptions`
   - Targets: `FacebookOAuthSetupExtensions`, `IdOAuthFacebookOptions`.
   - Details: use `services.AddOptions<IdOAuthFacebookOptions>().Validate(...)` to require `AppId` and `AppSecret` at startup.

8. Improve logging and error mapping
   - Targets: `FacebookClient`, `FacebookTokenVerifier`, handlers.
   - Details: include HTTP status code and response body in logs; return structured error codes in `GenResult` so handlers can map to proper HTTP responses (401 vs 400).

9. JSON serializer options
   - Targets: `FacebookClient` deserialization sites.
   - Details: use a shared `JsonSerializerOptions` (camelCase naming, ignore nulls) to avoid parsing surprises.

10. Add integration tests
    - Targets: sign-in handlers and overall flow.
    - Details: run against mocked Graph endpoints or a test harness to verify end-to-end flow (verify -> /me -> create user).

## Low priority / Nice to have

11. Add `AddMyIdOAuthFacebook` overload to match Google addon
    - Targets: `FacebookOAuthSetupExtensions`.
    - Details: provide an overload named to match the Google API surface. Optionally auto-register the ASP.NET Core Facebook handler.

12. API docs and README improvements
    - Targets: `README.md` and project docs.
    - Details: add example usage snippets (MyIdInstaller_Pg), required scopes, and notes about secrets/storage.

13. CI / build checks
    - Targets: repository CI pipeline.
    - Details: add build step and unit tests for the Facebook addon to run on PRs.

14. Add more robust error/result model
    - Targets: `GenResult<T>` usage.
    - Details: include ResultType/Code/Message/Exception/Info fields so callers can handle different failure modes easily.

## Quick wins (apply immediately)

- Add `ToString()` overrides on models (done).
- Normalize parameter names to `userAccessToken` (done).
- Ensure HttpClient base address includes version and trailing slash (check `FacebookHttpClientConfiguration`).

---

If you want I can implement items in order. Which task should I start with?