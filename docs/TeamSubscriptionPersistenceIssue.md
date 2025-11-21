# Team subscription persistence issue

## Summary

During customer registration we sometimes get `DbUpdateConcurrencyException` when saving a newly added `TeamSubscription`. The repository currently contains a helper `TeamRepo.AddNewSubscriptionsToDbAsync` that calls `Db.Add(sub)` for subscriptions that are present on the in-memory `Team` but not in the database. That prevents the exception but feels like a workaround rather than a systematic fix.

This document describes the problem, why it happens, diagnostics to reproduce, and the possible solutions with pros/cons and recommended next steps.

---

## Reproduction (typical flow)
1. Create a `Team` and call `TeamManagerService.AddTeamAsync` (saved to DB). The returned `Team` instance is tracked by its `DbContext`/UoW during that request.
2. Create a `User` and add to the team (saved).
3. Call `GetSubscriptionServiceAsync(team)` and then `AddSubscriptionAsync(planId)` on the subscription service.
4. `Team.AddSubscription` creates a `TeamSubscription` with a client-generated GUID and adds it to the team's `_subscriptions` collection.
5. `SubscriptionService.UpdateAndSaveAsync` calls `_teamRepo.UpdateAsync(team)` and `uow.SaveChangesAsync()`.
6. EF issues an `UPDATE` for `team_subscription` instead of `INSERT` and the update affects 0 rows ? `DbUpdateConcurrencyException`.

---

## Root cause (plain)
Entity Framework determines INSERT vs UPDATE by the ChangeTracker state on the objects it knows about. When you attach a detached aggregate and mark the root `Modified` (via `Db.Entry(entity).State = EntityState.Modified`), EF will attempt UPDATEs for the root and its children unless children are explicitly marked as `Added`.

Because this codebase uses client-generated GUID primary keys, EF cannot rely on the "default key => Added" convention. If the aggregate instance passed into `UpdateAsync` is detached for the current `DbContext` (even if the UoW instance is the same), EF will attempt UPDATEs for those children and fail when the rows do not exist.

---

## Why the current workaround `AddNewSubscriptionsToDbAsync` feels like a hack
- It inspects DB state and then calls `Db.Add(sub)` for new children before setting the root Modified. That prevents the UPDATE/0-row problem but mixes domain and persistence concerns and requires carefully enumerating/handling every nested child type (subscriptions, devices, etc.).
- It compensates for detached-aggregate usage instead of addressing the root cause: inconsistent tracking or cross-context usage patterns.

---

## Possible solutions (ranked)

1) Preferred — operate on a tracked aggregate (recommended)
- Ensure the service that mutates the aggregate (adds subscriptions/members) loads the tracked `Team` from its own repository/UoW and mutates that tracked instance (e.g. `var team = await repo.FirstByIdWithSubscriptions(teamId)` then `team.AddSubscription(...)`). Call `SaveChanges()` on the same DbContext.
- Pros: Uses EF as intended; minimal manual state handling; easiest to reason about and test.
- Cons: Requires an extra DB lookup in some scenarios (small cost, safe).

2) Keep domain creation but make repository robust (current pattern, but improved)
- In `TeamRepo.UpdateAsync` only set `EntityState.Modified` if the entity is truly detached; always call helpers to `Db.Add(...)` any new child entities before SaveChanges. Improve detection and handle nested children. Also avoid overwriting states for already tracked entities.
- Pros: Preserves domain-first behavior; no extra read required when update call already has the full aggregate object.
- Cons: Complex to implement correctly and easy to miss nested cases; maintainability burden.

3) Change factory to resolve UoW per call or always use caller's UoW
- `TeamSubsriptionServiceFactory` should not capture a UoW at construction time or should provide a `GetServiceUsing(IIdUnitOfWork uow, Team team)` method. Alternatively, have `TeamManagerService.GetSubscriptionServiceAsync(Team team)` construct `new SubscriptionService(uow, team)` using its own `uow` rather than delegating to the factory.
- Pros: Ensures service uses same DbContext as manager when caller already has it.
- Cons: Small refactor; careful about factory usages elsewhere.

4) Use database-generated keys instead of client GUIDs
- Let DB create keys (identity, sequences). Then EF can use default-key conventions to detect `Added`. Changing primary key strategy is intrusive and impacts many entities and tests.
- Pros: Simplifies EF conventions.
- Cons: Large migration and refactor cost; test and domain impact.

5) Hybrid: explicit state tracking API
- Add helper API on UoW/repo to `AttachNewChildren(Team team)` or `AttachForUpdate(Team team)` and centralize state decisions there.
- Pros: Centralized logic, can be unit-tested.
- Cons: Still manual state management.

---

## Diagnostics to gather now
- Log ChangeTracker states immediately before SaveChanges in failure scenarios:
  - `Db.Entry(team).State` and foreach subscription `Db.Entry(sub).State`.
  - Log `RuntimeHelpers.GetHashCode(uow)` for all involved services to confirm UoW identity (done).
- Catch `DbUpdateConcurrencyException` at `SaveChangesAsync` and log `ex.Entries` with OriginalValues/CurrentValues.
- Inspect EF SQL logs to confirm whether `UPDATE` or `INSERT` was issued for `team_subscription` (you already have logs showing UPDATE).

---

## Recommended immediate action
1. Implement the minimal safe change: in `TeamManagerService.GetSubscriptionServiceAsync(Team team)` return `new SubscriptionService(uow, team)` (use the manager's `uow`) instead of delegating to `TeamSubsriptionServiceFactory.GetServiceAsync(team)`. This guarantees the subscription service uses the same DbContext instance when the manager already has it.
2. Add a defensive check in `TeamRepo.UpdateAsync`:
   - Only set the root `EntityState.Modified` when `Db.Entry(entity).State == EntityState.Detached`.
   - Keep `AddNewSubscriptionsToDbAsync` and `AddNewDevicesToDbAsync` for now, but add tests around them.
3. Add logs that output entity states before `SaveChangesAsync` to aid future debugging.

These changes are minimal, low-risk, and will likely eliminate the runtime exception while keeping the current domain design.

---

## Long term plan
- Prefer pattern (1): always operate on a tracked aggregate for business operations. Make factory APIs accept `teamId` and load tracked aggregate before mutation.
- Remove manual AddNew* helpers once tracked-aggregate usage is consistent.
- Consider adding integration tests that run the full registration flow to detect this class of bug.

---

## Tests to add
- Unit tests for `TeamRepo.UpdateAsync` that simulate a detached `Team` containing a new `TeamSubscription` and assert that `Db.Add` is called for new subscription and `SaveChanges` doesn't throw.
- Integration test for the registration flow covering: Add team, add user, add subscription in one request scope verifying no concurrency exception and that subscription row exists after Save.

---

## Notes
- The `AddNewSubscriptionsToDbAsync` helper is a practical mitigation and should remain while migrating to a cleaner pattern. But the goal should be to make the persistence approach explicit and predictable (tracked aggregate first) rather than relying on inspection heuristics across nested children.

