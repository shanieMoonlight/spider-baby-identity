# ID.OAuth.Facebook — To Do

This file lists recommended follow-up tasks prioritized by importance. Tackle them one-by-one and mark items off as completed.

## High priority


6. Email verification policy
   - Targets: `FindOrCreateService`, `FacebookUserProfile` model.
   - Details: treat email as unverified unless provider explicitly provides a verified flag. Update `FindOrCreateService` to set email verified = false by default and trigger your normal verification flow.

## Medium priority


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


## Quick wins (apply immediately)

- Add `ToString()` overrides on models (done).
- Normalize parameter names to `userAccessToken` (done).
- Ensure HttpClient base address includes version and trailing slash (check `FacebookHttpClientConfiguration`).

---



Dom't include body:   var info = $"StatusCode: {(int)response.StatusCode}. Body: {body}";