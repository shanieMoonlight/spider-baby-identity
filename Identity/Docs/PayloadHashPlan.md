Payload storage & selector/validator plan
=========================================

Goal
----
Stop storing raw refresh-token bearer secrets in the DB. Use a secure pattern that:
- avoids an application-level secret where possible,
- keeps lookup fast and indexed,
- supports rotation and revocation,
- is easy to test and migrate before first public release.

Decision
--------
Implement the selector + validator pattern (recommended). This requires a small schema change but does not require an app secret and matches industry best practice. A short note about the alternative (HMAC) is kept at the end for reference.

High-level design
-----------------
- Client token = `selector + "." + validator`
  - `selector`: short public identifier (12–16 random bytes, base64url). Stored in DB plaintext and indexed.
  - `validator`: long secret (32+ random bytes, base64url). Only the client sees it.
- DB stores: `Selector` (plain), `ValidatorHash` (KDF output / PasswordHasher), expiration, user, device, revoked flags.
- Verification: split token, lookup by `Selector` (index), verify `validator` against `ValidatorHash` using a KDF (no app secret), then check expiry/revoked.

Why this approach
-----------------
- No application secret required (unlike HMAC approach). Use per?token salt + KDF (PBKDF2 / PasswordHasher) — same model as ASP.NET Identity password hashing.
- Fast indexed lookup by `Selector`.
- Supports rotation and per-token revocation.
- Well-understood, easy to test and reason about.

Detailed steps (what to implement)
----------------------------------
1) Domain model changes
   - Add `Selector : string` and `ValidatorHash : string` to `IdRefreshToken`.
   - Mark any in-memory-only raw values as `[NotMapped]` (for example a `ClientToken` property returned to callers).
   - Remove or repurpose the existing `Payload` property. (You mentioned you'll rename/remove `Payload` before publishing — do that first so code compiles.)

2) EF mapping
   - Map `Selector` (required) with a sensible max length (e.g. 32) and add a non-clustered index (unique if you guarantee uniqueness).
   - Map `ValidatorHash` (required) with enough length to hold the KDF output (e.g. 200+ chars depending on format).
   - Update any specs that previously queried `Payload` to query by `Selector`.

3) Token generation (JwtRefreshTokenService)
   - Generate `selector = Base64Url(RandomBytes(12..16))`.
   - Generate `validator = RandomTokenGenerator.Generate()` (or a shorter generator specialized for validator length).
   - Compute `validatorHash` using a KDF. Recommended: reuse `PasswordHasher<TUser>` or PBKDF2 with per-hash salt.
   - Persist `Selector` and `ValidatorHash` (and other metadata). Attach `ClientToken = selector + "." + validator` to return to the caller.
   - Provide an API that returns the raw client token to callers (controller/service) so it can be sent to clients. Keep existing methods for internal use if needed but prefer a new method or return DTO with `RawPayload` so tests and callers are explicit.

4) Token lookup & verification
   - Change `FindTokenWithUserAndDeviceAndTeamAsync` (or add a new method) to accept the client token string.
   - Split the token: `selector`, `validatorCandidate`.
   - Query by `Selector` (fast) and include User/Team/TrustedDevice.
   - If a row is found, verify `validatorCandidate` against `ValidatorHash` using the same KDF / PasswordHasher verify method.
   - If verify succeeds and token not expired/revoked, return the token entity; otherwise null or unauthorized.

5) Token rotation & revocation semantics
   - On refresh issue a new selector+validator and mark the old token revoked (or remove it) to detect reuse.
   - If a revoked token is used, treat as potential compromise — log & escalate.

6) Tests
   - Add unit tests for generator: returned client token matches stored `Selector` and `ValidatorHash` verifies for the raw validator.
   - Add tests for verification: valid token accepted, wrong validator rejected, expired/revoked tokens rejected.
   - Update any tests that referenced the old `Payload` field to use the new API or check `ValidatorHash` where appropriate.

7) Migration & rollout notes
   - You indicated this repo is not published yet. Good — remove/replace old migrations and create a fresh initial migration after these schema changes.
   - If supporting a pre-existing deployed DB becomes necessary in future, implement dual-lookup: try selector+hash verification first and fall back to legacy `Payload` equality match for a migration window.

8) Security notes
   - Use a cryptographically secure RNG (RandomNumberGenerator) for both selector and validator.
   - Store `ValidatorHash` using a slow KDF (PasswordHasher/PBKDF2/Argon2). Do not use a single deterministic hash without salt unless you intentionally use HMAC (which requires an app secret).
   - Do not log raw validator values. Only log selector or token identifiers.

9) Implementation checklist
   - [ ] Rename or remove `Payload` in `IdRefreshToken` (you will do this before publish).
   - [ ] Add `Selector` and `ValidatorHash` to `IdRefreshToken` and annotate `[NotMapped]` for any transient `ClientToken` property.
   - [ ] Update EF `RefreshTokenConfig` to map the new columns and add index on `Selector`.
   - [ ] Update `IJwtRefreshTokenService` either by overloading or by adding new methods returning the raw client token (e.g., `GenerateTokenAndRawAsync`), and update implementations.
   - [ ] Update `JwtRefreshTokenService.GenerateToken*` to produce selector+validator, persist hash, and return raw client token.
   - [ ] Update `FindTokenWithUserAndDeviceAndTeamAsync` to accept client token, split and verify (or add a new verification method and keep old API for compatibility).
   - [ ] Update domain factories and tests in `ID.Tests.Data.Factories` and anywhere `Payload` was referenced.
   - [ ] Add unit tests for generation & verification flows (Moq + Shouldly).
   - [ ] Create a fresh initial EF migration once all changes are in place.

Alternative: HMAC-in-place (smaller, but requires secret)
------------------------------------------------------
If you prefer a minimal code change and want to avoid schema changes, an alternative is to keep `Payload` but store `HMAC(secret, raw)` in `Payload` and hash incoming values before lookup. Downsides:
- Requires an application secret (RefreshTokenHmacKey) and secret management.
- Rotating that secret invalidates all tokens.
- Less flexible than selector+validator.

Given you plan to rename `Payload` and can regenerate migrations, selector+validator is the recommended approach.

If you confirm, I will implement the model, EF mapping, service generation & verification changes and tests. Which piece should I start with first? (I suggest: model + EF mapping + simple unit tests for generator/verify.)