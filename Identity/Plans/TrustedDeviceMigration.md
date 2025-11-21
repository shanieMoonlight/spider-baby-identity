# Trusted Device Feature Migration Plan

**Purpose**: Add capability for users to mark devices as "trusted" and skip MFA on subsequent logins from those devices.

**Architecture Decision**: Trusted devices belong to `AppUser` (not `Team`) since MFA bypass is a personal security decision.

---

## Phase 1: Domain Layer (Identity\Core\ID.Domain)

### Step 1.1: Create TrustedDevice Value Objects
**Location**: `Identity\Core\ID.Domain\Entities\AppUsers\TrustedDevices\ValueObjects\`

Create the following value objects:
- [x] `DeviceFingerprint.cs` - validates device identifier (max 512 chars, non-empty)
- [x] `DeviceName.cs` - user-friendly device name (max 100 chars, non-empty)
- [x] `UserAgent.cs` - browser/OS info (max 500 chars, nullable)
- [x] `TrustedUntil.cs` - expiry date (nullable DateTime, must be future if set)

**Pattern**: Follow `ClArch.ValueObjects` pattern (see `EmailAddress`, `Name` in existing code).

---

### Step 1.2: Create TrustedDevice Aggregate
**Location**: `Identity\Core\ID.Domain\Entities\AppUsers\TrustedDevices\TrustedDevice.cs`

**Properties**:
```csharp
public class TrustedDevice : IdDomainEntity
{
    public Guid UserId { get; private set; }
    public AppUser? User { get; private set; }
    public string DeviceFingerprint { get; private set; }
    public string Name { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime? TrustedUntil { get; private set; }
    public DateTime LastUsedDate { get; private set; }
}
```

**Methods**:
- [x] `internal static TrustedDevice Create(AppUser user, DeviceFingerprint fingerprint, DeviceName name, UserAgent userAgent, TrustedUntil? trustedUntil)`
- [x] `internal TrustedDevice UpdateLastUsed()` - sets `LastUsedDate = DateTime.UtcNow`
- [x] `public bool IsExpired()` - checks if `TrustedUntil < DateTime.UtcNow`
- [x] `internal TrustedDevice Revoke()` - sets `TrustedUntil = DateTime.UtcNow` (soft delete pattern)

**Domain Events**:
- Raise `TrustedDeviceAddedDomainEvent(UserId, TrustedDevice)` on create
- Raise `TrustedDeviceRevokedDomainEvent(UserId, DeviceFingerprint)` on revoke
- Raise `TrustedDeviceUsedDomainEvent(UserId, DeviceFingerprint)` on `UpdateLastUsed()`

---

### Step 1.3: Create TrustedDevice Validators
**Location**: `Identity\Core\ID.Domain\Entities\AppUsers\TrustedDevices\Validators\TrustedDeviceValidators.cs`

Follow the `TeamValidators` pattern (see `TeamValidators.MemberAddition.cs`):

- [ ] `TrustedDeviceValidators.Addition` - validates user can add device
  - Business rule: Max 10 trusted devices per user
  - Business rule: Device fingerprint not already trusted by this user
  - Returns `GenResult<Addition.Token>`

- [ ] `TrustedDeviceValidators.Revocation` - validates user can revoke device
  - Business rule: Device belongs to user
  - Business rule: Device is not already revoked
  - Returns `GenResult<Revocation.Token>`

**Pattern**: Each validator has:
```csharp
public sealed class Token : IValidationToken { }
public static GenResult<Token> Validate(...) { }
```

---

### Step 1.4: Create Domain Events
**Location**: `Identity\Core\ID.Domain\Entities\AppUsers\Events\`

- [ ] `TrustedDeviceAddedDomainEvent.cs`
- [ ] `TrustedDeviceRevokedDomainEvent.cs`
- [ ] `TrustedDeviceUsedDomainEvent.cs`

**Pattern**: `public sealed record TrustedDeviceAddedDomainEvent(Guid UserId, TrustedDevice Device) : IIdDomainEvent`

---

### Step 1.5: Update AppUser Aggregate
**Location**: `Identity\Core\ID.Domain\Entities\AppUsers\AppUser.cs`

Add navigation property:
```csharp
private readonly HashSet<TrustedDevice> _trustedDevices = [];
public IReadOnlyCollection<TrustedDevice> TrustedDevices =>
    _trustedDevices.ToList().AsReadOnly();
```

Add methods:
- [ ] `public TrustedDevice TrustDevice(TrustedDeviceValidators.Addition.Token token)` - calls `TrustedDevice.Create`, adds to `_trustedDevices`
- [ ] `public bool RevokeTrustedDevice(TrustedDeviceValidators.Revocation.Token token)` - calls `device.Revoke()`
- [ ] `public TrustedDevice? FindTrustedDevice(string deviceFingerprint)` - queries `_trustedDevices`

---

## Phase 2: Repository Layer (Identity\Core\ID.Domain.Repos)

### Step 2.1: Create Repository Interface
**Location**: `Identity\Core\ID.Domain.Repos\ITrustedDeviceRepo.cs`

```csharp
internal interface ITrustedDeviceRepo : IGenCrudRepo<TrustedDevice>
{
    Task<TrustedDevice?> GetByUserAndDeviceAsync(Guid userId, string deviceFingerprint, CancellationToken ct = default);
    Task<List<TrustedDevice>> ListByUserAsync(Guid userId, CancellationToken ct = default);
    Task<bool> IsDeviceTrustedAsync(Guid userId, string deviceFingerprint, CancellationToken ct = default);
    Task<int> RemoveExpiredAsync(CancellationToken ct = default);
    Task<int> CountByUserAsync(Guid userId, CancellationToken ct = default);
}
```

---

## Phase 3: Persistence Layer (Identity\Infrastructure\ID.Persistence.Ef)

### Step 3.1: Create EF Core Configuration
**Location**: `Identity\Infrastructure\ID.Persistence.Ef\Config\TrustedDeviceConfig.cs`

```csharp
internal class TrustedDeviceConfig : IEntityTypeConfiguration<TrustedDevice>
{
    public void Configure(EntityTypeBuilder<TrustedDevice> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => new { x.UserId, x.DeviceFingerprint })
            .IsUnique()
            .HasFilter("TrustedUntil IS NULL OR TrustedUntil > GETUTCDATE()");
        
        builder.Property(x => x.DeviceFingerprint)
            .IsRequired()
            .HasMaxLength(512);
            
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);
            
        builder.HasOne(x => x.User)
            .WithMany(u => u.TrustedDevices)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

### Step 3.2: Implement Repository
**Location**: `Identity\Infrastructure\ID.Persistence.Ef\Repos\TrustedDeviceRepo.cs`

Inherit from `AGenCrudRepo<TrustedDevice>` and implement custom methods:

- [ ] `GetByUserAndDeviceAsync` - `FirstOrDefaultAsync(x => x.UserId == userId && x.DeviceFingerprint == fingerprint)`
- [ ] `ListByUserAsync` - `Where(x => x.UserId == userId).OrderByDescending(x => x.LastUsedDate).ToListAsync()`
- [ ] `IsDeviceTrustedAsync` - check exists + not expired
- [ ] `RemoveExpiredAsync` - delete where `TrustedUntil < DateTime.UtcNow`
- [ ] `CountByUserAsync` - `CountAsync(x => x.UserId == userId && (x.TrustedUntil == null || x.TrustedUntil > DateTime.UtcNow))`

---

### Step 3.3: Register Repository in DI
**Location**: `Identity\Infrastructure\ID.Persistence.Ef\Setup\IdPersistenceSetup.cs`

Add:
```csharp
services.AddScoped<ITrustedDeviceRepo, TrustedDeviceRepo>();
```

---

### Step 3.4: Create EF Migration
**Terminal Command** (from `ID.Persistence.Ef.Postgres` or `ID.Persistence.Ef.SQL`):
```powershell
dotnet ef migrations add AddTrustedDevices
```

Review migration, then apply:
```powershell
dotnet ef database update
```

---

## Phase 4: Application Layer (Identity\Core\ID.Application)

### Step 4.1: Create DTOs
**Location**: `Identity\Core\ID.Application\Features\Account\Dtos\TrustedDevices\`

- [ ] `TrustedDeviceDto.cs` - for queries
  ```csharp
  public record TrustedDeviceDto(
      Guid Id,
      string Name,
      string DeviceFingerprint,
      string? UserAgent,
      DateTime? TrustedUntil,
      DateTime LastUsedDate);
  ```
  
- [ ] `TrustDeviceDto.cs` - for trust command (used in MFA verification)
  ```csharp
  public record TrustDeviceDto(
      string DeviceFingerprint,
      string Name,
      int? TrustDurationDays = 30);
  ```

---

### Step 4.2: Update Existing MFA DTOs
**Location**: `Identity\Core\ID.Application\Features\Account\Cmd\Mfa\TwoFactorVerify\`

Update `Verify2FactorDto.cs`:
```csharp
public class Verify2FactorDto
{
    [Required]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    public string Token { get; set; } = string.Empty;
    
    public string? DeviceId { get; set; }
    
    public bool TrustDevice { get; set; } = false; // NEW
    public string? DeviceName { get; set; }        // NEW (optional, use User-Agent if null)
    public int? TrustDurationDays { get; set; }    // NEW (nullable, use default 30)
}
```

Update `Verify2FactorCookieDto.cs` similarly.

---

### Step 4.3: Update PreSignInService
**Location**: `Identity\Core\ID.Application\AppImps\SignIn\PreSignInService.cs`

**Changes**:
1. Inject `ITrustedDeviceRepo`
2. In `Authenticate` method, after password check (line ~40), add:
   ```csharp
   // Check if device is trusted
   if (!string.IsNullOrWhiteSpace(dto.DeviceId))
   {
       var isTrusted = await _trustedDeviceRepo.IsDeviceTrustedAsync(user.Id, dto.DeviceId, cancellationToken);
       if (isTrusted)
       {
           // Update last used date
           var trustedDevice = await _trustedDeviceRepo.GetByUserAndDeviceAsync(user.Id, dto.DeviceId, cancellationToken);
           if (trustedDevice != null)
           {
               trustedDevice.UpdateLastUsed();
               await _trustedDeviceRepo.UpdateAsync(trustedDevice, cancellationToken);
           }
           
           return MyIdSignInResult.Success(user, user.Team!);
       }
   }
   
   // Existing MFA check
   var tfEnabled = await _2FactorService.IsTwoFactorEnabledAsync(user);
   if (tfEnabled && !_fromAppService.IsFromApp)
       return await SendTwoFactor(user, user.Team!);
   ```

---

### Step 4.4: Update MFA Verification Handlers
**Location**: `Identity\Core\ID.Application\Features\Account\Cmd\Mfa\TwoFactorVerify\Verify2FactorHandler.cs`

After successful MFA verification (line ~44), add:
```csharp
// Trust device if requested
if (dto.TrustDevice && !string.IsNullOrWhiteSpace(dto.DeviceId))
{
    var deviceName = string.IsNullOrWhiteSpace(dto.DeviceName) 
        ? "Trusted Device" 
        : dto.DeviceName;
    
    var trustDuration = dto.TrustDurationDays ?? 30;
    var trustedUntil = DateTime.UtcNow.AddDays(trustDuration);
    
    var addToken = TrustedDeviceValidators.Addition.Validate(
        user, 
        DeviceFingerprint.Create(dto.DeviceId),
        DeviceName.Create(deviceName),
        UserAgent.CreateNullable(null), // TODO: extract from HttpContext if needed
        TrustedUntil.CreateNullable(trustedUntil)
    );
    
    if (addToken.Succeeded)
    {
        var trustedDevice = user.TrustDevice(addToken.Value!);
        await _trustedDeviceRepo.AddAsync(trustedDevice, cancellationToken);
    }
    // Log warning if validation failed but don't block login
}
```

Repeat for `Verify2FactorCookieCmdHandler.cs`.

---

### Step 4.5: Create Management Commands/Queries

#### List Trusted Devices Query
**Location**: `Identity\Core\ID.Application\Features\Account\Qry\TrustedDevices\ListTrustedDevices\`

- [ ] `ListTrustedDevicesQuery.cs` - `public record ListTrustedDevicesQuery() : AIdUserAwareQuery<List<TrustedDeviceDto>>;`
- [ ] `ListTrustedDevicesHandler.cs` - queries repo, maps to DTOs

#### Revoke Trusted Device Command
**Location**: `Identity\Core\ID.Application\Features\Account\Cmd\TrustedDevices\RevokeTrustedDevice\`

- [ ] `RevokeTrustedDeviceDto.cs` - `public record RevokeTrustedDeviceDto(Guid DeviceId);`
- [ ] `RevokeTrustedDeviceCmd.cs` - `public record RevokeTrustedDeviceCmd(RevokeTrustedDeviceDto Dto) : AIdUserAwareCommand<AppUser, BasicResult>;`
- [ ] `RevokeTrustedDeviceCmdHandler.cs` - validates, calls `user.RevokeTrustedDevice()`, saves

---

## Phase 5: Presentation/API Layer (Identity\API\ID.API or ID.Presentation)

### Step 5.1: Create Controller Endpoints
**Location**: `Identity\API\ID.API\Controllers\TrustedDevicesController.cs` or add to existing `AccountController.cs`

- [ ] `GET /api/account/trusted-devices` - calls `ListTrustedDevicesQuery`
- [ ] `DELETE /api/account/trusted-devices/{deviceId}` - calls `RevokeTrustedDeviceCmd`

**Authorization**: Require authenticated user (`[Authorize]`)

---

## Phase 6: Infrastructure - Scheduled Jobs (Identity\Infrastructure\ID.Jobs.Quartz)

### Step 6.1: Create Cleanup Job
**Location**: `Identity\Infrastructure\ID.Jobs.Quartz\Jobs\CleanupExpiredTrustedDevicesJob.cs`

```csharp
public class CleanupExpiredTrustedDevicesJob : IJob
{
    private readonly ITrustedDeviceRepo _repo;
    
    public async Task Execute(IJobExecutionContext context)
    {
        var removed = await _repo.RemoveExpiredAsync(context.CancellationToken);
        // Log result
    }
}
```

---

### Step 6.2: Register Job in Quartz
**Location**: `Identity\Infrastructure\ID.Jobs.Quartz\Setup\QuartzSetup.cs`

Add job to run daily at 2 AM:
```csharp
q.AddJob<CleanupExpiredTrustedDevicesJob>(j => j.WithIdentity("CleanupExpiredTrustedDevices"))
  .AddTrigger(t => t
      .ForJob("CleanupExpiredTrustedDevices")
      .WithCronSchedule("0 0 2 * * ?") // Daily at 2 AM
  );
```

---

## Phase 7: Testing

### Step 7.1: Domain Tests
**Location**: `Identity\Tests\ID.Domain.Tests\Entities\AppUsers\TrustedDevices\`

- [ ] `TrustedDevice_Create_Tests.cs` - test factory method
- [ ] `TrustedDevice_IsExpired_Tests.cs` - test expiry logic
- [ ] `TrustedDevice_Revoke_Tests.cs` - test revocation
- [ ] `TrustedDeviceValidators_Addition_Tests.cs` - test business rules
- [ ] `TrustedDeviceValidators_Revocation_Tests.cs`
- [ ] `AppUser_TrustDevice_Tests.cs` - test aggregate methods

**Pattern**: Follow `Team_AddMember_Tests.cs` patterns with `//--------------------//` separator between tests.

---

### Step 7.2: Application Tests
**Location**: `Identity\Tests\Id.Application.Tests\Features\Account\Cmd\TrustedDevices\`

- [ ] `RevokeTrustedDeviceCmdHandlerTests.cs` - test command handler

**Location**: `Identity\Tests\Id.Application.Tests\Features\Account\Qry\TrustedDevices\`

- [ ] `ListTrustedDevicesHandlerTests.cs` - test query handler

---

### Step 7.3: Update PreSignInService Tests
**Location**: `Identity\Tests\Id.Application.Tests\ApplicationImps\SignIn\PreSignInServiceTests.cs`

Add new tests:
- [ ] `Authenticate_TrustedDevice_BypassesMFA_ReturnsSuccess`
  - Mock `_trustedDeviceRepo.IsDeviceTrustedAsync` → `true`
  - Mock `_2FactorService.IsTwoFactorEnabledAsync` → `true`
  - Assert: `result.Succeeded == true`, MFA was NOT sent
  
- [ ] `Authenticate_UntrustedDevice_TwoFactorEnabled_ReturnsTwoFactorRequired`
  - Mock `_trustedDeviceRepo.IsDeviceTrustedAsync` → `false`
  - Mock `_2FactorService.IsTwoFactorEnabledAsync` → `true`
  - Assert: `result.TwoFactorRequired == true`
  
- [ ] `Authenticate_ExpiredTrustedDevice_RequiresMFA`
  - Mock `_trustedDeviceRepo.IsDeviceTrustedAsync` → `false` (expired)
  - Assert: MFA is required

---

### Step 7.4: Update MFA Verification Tests
**Location**: `Identity\Tests\Id.Application.Tests\Features\Account\Cmd\Mfa\TwoFactorVerify\`

Update `Verify2FactorHandlerTests.cs`:
- [ ] `Handle_TrustDeviceTrue_AddsDeviceToRepo`
  - Set `dto.TrustDevice = true`
  - Verify `_trustedDeviceRepo.AddAsync` was called

---

## Phase 8: Documentation & Configuration

### Step 8.1: Update README/Docs
**Location**: `Identity\NEXT_STEPS.md` or `Identity\README.md`

Add section:
```markdown
## Trusted Devices

Users can mark devices as trusted after successful MFA verification. Trusted devices bypass MFA for a configurable duration (default 30 days).

**Endpoints**:
- `GET /api/account/trusted-devices` - List user's trusted devices
- `DELETE /api/account/trusted-devices/{id}` - Revoke a trusted device

**Configuration**:
- Default trust duration: 30 days (sliding expiration based on last use)
- Max trusted devices per user: 10
```

---

### Step 8.2: Configuration Options (Optional)
**Location**: `Identity\Core\ID.GlobalSettings\Setup\Options\IdGlobalOptions.cs`

Add properties:
```csharp
public int TrustedDeviceMaxCount { get; set; } = 10;
public int TrustedDeviceDefaultDurationDays { get; set; } = 30;
```

Use in validators and handlers instead of hardcoded values.

---

## Checklist Summary

### Domain Layer
- [ ] Value objects (DeviceFingerprint, DeviceName, UserAgent, TrustedUntil)
- [ ] TrustedDevice aggregate with methods
- [ ] TrustedDevice validators (Addition, Revocation)
- [ ] Domain events (Added, Revoked, Used)
- [ ] Update AppUser with navigation property and methods

### Repository Layer
- [ ] ITrustedDeviceRepo interface
- [ ] TrustedDeviceRepo implementation
- [ ] EF Core configuration
- [ ] Register in DI
- [ ] Create and apply migration

### Application Layer
- [ ] DTOs (TrustedDeviceDto, TrustDeviceDto)
- [ ] Update Verify2FactorDto with TrustDevice flag
- [ ] Update PreSignInService to check trusted devices
- [ ] Update MFA verification handlers to trust devices
- [ ] Create ListTrustedDevices query/handler
- [ ] Create RevokeTrustedDevice command/handler

### Presentation Layer
- [ ] Controller endpoints (List, Revoke)

### Infrastructure
- [ ] CleanupExpiredTrustedDevicesJob
- [ ] Register job in Quartz

### Testing
- [ ] Domain tests (TrustedDevice + validators)
- [ ] Application tests (commands, queries)
- [ ] Update PreSignInService tests
- [ ] Update MFA verification tests

### Documentation
- [ ] Update README/NEXT_STEPS
- [ ] Add configuration options (optional)

---

## Estimated Effort
- **Domain Layer**: 4-6 hours
- **Repository + Persistence**: 2-3 hours
- **Application Layer**: 4-5 hours
- **Presentation + Jobs**: 2 hours
- **Testing**: 6-8 hours
- **Total**: ~18-24 hours

---

## Notes
- Follow existing patterns (validator tokens, domain events, GenResult)
- Use `//--------------------//` separator in tests
- All repo operations should be async with CancellationToken
- Domain methods are `internal`, exposed via validators
- Log warnings (not errors) if device trust fails - don't block login



-create  GLobalSettings for MaxTrustedDevices
- test AppUser.TrustDevice and AppUser.RevokeTrustedDevice
- test TrustedDeviceValidators
- test trustedDevice
- test trustedDevice Specs
- restrict ITrustedDeviceRepo ??? NO add delete etc?????  CanDeleteAsync??
- TrustedUntil Global setting
