# TeamManagerService Refactoring Strategy 🚀

## Overview
This document outlines a strategy to refactor the `TeamManagerService` using a **Service Factory Pattern** that maintains developer convenience while improving separation of concerns.

## Current State
- **TeamManagerService**: 26 methods, ~450 lines
- **Single service** handling all team-related operations
- **Good**: Easy to discover, one service to remember
- **Challenge**: Large interface, multiple responsibilities

## Proposed Architecture Pattern 🎯

### Core Concept: "Gateway Service + Specialized Services"
Instead of splitting into separate services that developers must remember, use the main service as a **gateway** that provides **context-aware specialized services**.

```csharp
// Single entry point (developers remember one service)
var teamManager = serviceProvider.GetService<IIdentityTeamManager<AppUser>>();

// Get specialized services when needed (with context)
var subscriptionService = await teamManager.GetSubscriptionServiceAsync(teamId);
var membershipService = await teamManager.GetMembershipServiceAsync(team);
var queryService = teamManager.GetQueryService();
```

## Proposed Service Boundaries 📦

### 1. TeamQueryService (Read Operations)
**Responsibility**: All read operations, searching, filtering

```csharp
public interface ITeamQueryService
{
    Task<IEnumerable<Team>> GetAllTeamsAsync(bool includeMntc = true, bool includeSuper = true);
    Task<IEnumerable<Team>> GetCustomerTeamsByNameAsync(string? name);
    Task<Page<Team>> GetPageAsync(int pageNumber, int pageSize, ...);
    Task<Team?> GetSuperTeamWithMembersAsync(int maxPosition = 1000);
    Task<Team?> GetMntcTeamWithMembersAsync(int maxPosition = 1000);
    // ... other query methods
}

// Usage:
var queries = teamManager.GetQueryService();
var allTeams = await queries.GetAllTeamsAsync(includeMaintenance: false);
```

### 2. TeamCommandService (Write Operations)
**Responsibility**: Team CRUD operations (create, update, delete)

```csharp
public interface ITeamCommandService
{
    Task<Team> CreateTeamAsync(Team newTeam, CancellationToken cancellationToken = default);
    Task<Team> UpdateTeamAsync(Team team, Name name, DescriptionNullable description);
    Task<BasicResult> DeleteTeamAsync(Team team);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

// Usage:
var commands = teamManager.GetCommandService();
var newTeam = await commands.CreateTeamAsync(teamData);
```

### 3. TeamMembershipService (Member Operations)
**Responsibility**: All member-related operations (context-aware)

```csharp
public interface ITeamMembershipService
{
    Team Team { get; } // Knows its context!
    
    Task<GenResult<TUser>> RegisterMemberAsync(TUser newMember);
    Task<GenResult<TUser>> RegisterMemberWithPasswordAsync(TUser newMember, string password);
    Task<GenResult<TUser>> UpdateMemberAsync(TUser updateUser);
    Task<BasicResult> DeleteMemberAsync(Guid memberId);
    Task<GenResult<Team>> SetLeaderAsync(TUser newLeader);
    Task<TUser?> GetMemberAsync(Guid userId);
    Task<IList<TUser>> GetAllMembersAsync(int maxPosition = 1000);
}

// Usage:
var memberService = await teamManager.GetMembershipServiceAsync(team);
await memberService.RegisterMemberAsync(newUser); // No need to pass team!
```

### 4. TeamSubscriptionService (Already Implemented) ✅
**Responsibility**: Subscription management

```csharp
// Already working example:
var subService = await teamManager.GetSubscriptionServiceAsync(teamId);
await subService.AddSubscriptionAsync(planId, discount);
```

### 5. TeamDeviceService (Already Implemented) ✅
**Responsibility**: Device management

```csharp
// Already working example:
var deviceService = await teamManager.GetDeviceServiceAsync(teamId, subId);
```

## Key Benefits ✅

### 1. **Developer Experience**
- ✅ **Single entry point**: Only need to remember `IIdentityTeamManager`
- ✅ **Discoverable**: IntelliSense shows `Get[Service]ServiceAsync()`
- ✅ **Context flows**: Team state passed to specialized services
- ✅ **No repetitive parameters**: Services know their context

### 2. **Architecture Benefits**
- ✅ **Single Responsibility**: Each service has focused concerns
- ✅ **Testability**: Smaller, focused services easier to test
- ✅ **Performance**: Context-aware services avoid repeated queries
- ✅ **Maintainability**: Changes isolated to specific service areas

### 3. **Flexibility**
- ✅ **Convenience methods**: Keep most common operations in main service
- ✅ **Specialized operations**: Use focused services for complex scenarios
- ✅ **Gradual migration**: Can implement incrementally

## Proposed Refactored TeamManagerService 🏗️

```csharp
public class TeamManagerService<TUser> : IIdentityTeamManager<TUser> 
{
    // CONVENIENCE METHODS (Keep most common operations)
    public async Task<Team?> GetByIdAsync(Guid? teamId)
    public async Task<Page<Team>> GetPageAsync(PagedRequest request)
    public async Task<Team> CreateTeamAsync(Team newTeam, CancellationToken cancellationToken)
    public async Task<GenResult<TUser>> RegisterMemberAsync(Team team, TUser newMember)
    
    // SERVICE FACTORIES (Gateway to specialized services)
    public ITeamQueryService GetQueryService()
    public ITeamCommandService GetCommandService()
    public async Task<GenResult<ITeamMembershipService>> GetMembershipServiceAsync(Guid? teamId)
    public async Task<GenResult<ITeamMembershipService>> GetMembershipServiceAsync(Team team)
    public async Task<GenResult<ITeamSubscriptionService>> GetSubscriptionServiceAsync(Guid? teamId)
    public async Task<GenResult<ITeamSubscriptionService>> GetSubscriptionServiceAsync(Team team)
    public async Task<GenResult<ITeamDeviceService>> GetDeviceServiceAsync(Guid? teamId, Guid? subId)
}
```

## Migration Strategy 🔄

### Phase 1: Add New Services (Non-Breaking)
```csharp
// Add new service methods alongside existing ones
public async Task<ITeamQueryService> GetQueryServiceAsync()
public async Task<GenResult<ITeamMembershipService>> GetMembershipServiceAsync(Team team)

// Keep existing methods (maybe mark as obsolete eventually)
public async Task<GenResult<TUser>> UpdateMemberAsync(Team team, TUser updateUser)
```

### Phase 2: Implement One Service at a Time
1. **Start with TeamMembershipService** (clear boundary)
2. **Test the pattern** with real usage
3. **Get feedback** from other developers
4. **Refine the approach** based on learnings

### Phase 3: Gradual Migration
1. **Update documentation** to show both approaches
2. **Let developers choose** their preferred style
3. **Eventually deprecate** duplicated methods (breaking change)

## Example Implementations 📝

### TeamMembershipService
```csharp
internal class TeamMembershipService(
    IIdUnitOfWork uow, 
    Team team, 
    UserManager<TUser> userManager) : ITeamMembershipService
{
    public Team Team { get; } = team;
    
    public async Task<GenResult<TUser>> RegisterMemberAsync(TUser newMember)
    {
        // Same validation token pattern
        var validationResult = TeamValidators.MemberAddition.Validate(Team, newMember);
        if (!validationResult.Succeeded)
            return validationResult.Convert<TUser>();

        var validationToken = validationResult.Value!;
        Team.AddMember(validationToken);

        var createResult = await userManager.CreateAsync(newMember);
        if (!createResult.Succeeded)
            return createResult.ToGenResult<TUser>(newMember);

        await UpdateAndSaveAsync();
        return GenResult<TUser>.Success(newMember);
    }
    
    // ... other member operations
}
```

### TeamQueryService
```csharp
internal class TeamQueryService(IIdUnitOfWork uow) : ITeamQueryService
{
    private readonly IIdentityTeamRepo _teamRepo = uow.TeamRepo;
    
    public async Task<IEnumerable<Team>> GetAllTeamsAsync(bool includeMntc = true, bool includeSuper = true)
    {
        var allTeams = await _teamRepo.ListAllAsync(new AllTeamsSpec());

        if (!includeMntc)
            allTeams = allTeams.Where(t => t.TeamType != TeamType.Maintenance).ToList();
        if (!includeSuper)
            allTeams = allTeams.Where(t => t.TeamType != TeamType.Super).ToList();

        return allTeams;
    }
    
    // ... other query operations
}
```

## Caching Strategy for Super and Maintenance Teams 🚀

### Why Cache These Teams?

Super and Maintenance teams have unique characteristics that make them ideal for caching:

#### **Access Patterns**
- ✅ **Frequently accessed** - Used for system operations, permissions, admin tasks
- ✅ **Rarely changed** - Team structure changes infrequently
- ✅ **System-critical** - Performance impacts entire application
- ✅ **Predictable queries** - Same queries repeated across requests

#### **Performance Impact**
```csharp
// These calls happen frequently across the application:
var superTeam = await teamManager.GetSuperTeamWithMembersAsync();
var maintenanceTeam = await teamManager.GetMntcTeamWithMembersAsync();

// Without caching: Database hit every time
// With caching: In-memory retrieval (99% faster)
```

### Caching Strategy Options 🎯

#### **Option 1: Application-Level Caching (Recommended)**

```csharp
public class CachedTeamQueryService : ITeamQueryService
{
    private readonly ITeamQueryService _innerService;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(30);
    
    private const string SUPER_TEAM_KEY = "SuperTeam_WithMembers";
    private const string MNTC_TEAM_KEY = "MntcTeam_WithMembers";

    public async Task<Team?> GetSuperTeamWithMembersAsync(int maxPosition = 1000)
    {
        var cacheKey = $"{SUPER_TEAM_KEY}_{maxPosition}";
        
        if (_cache.TryGetValue(cacheKey, out Team? cachedTeam))
        {
            return cachedTeam;
        }

        var team = await _innerService.GetSuperTeamWithMembersAsync(maxPosition);
        
        if (team != null)
        {
            _cache.Set(cacheKey, team, _cacheExpiry);
        }
        
        return team;
    }

    public async Task<Team?> GetMntcTeamWithMembersAsync(int maxPosition = 1000)
    {
        var cacheKey = $"{MNTC_TEAM_KEY}_{maxPosition}";
        
        if (_cache.TryGetValue(cacheKey, out Team? cachedTeam))
        {
            return cachedTeam;
        }

        var team = await _innerService.GetMntcTeamWithMembersAsync(maxPosition);
        
        if (team != null)
        {
            _cache.Set(cacheKey, team, _cacheExpiry);
        }
        
        return team;
    }
}
```

#### **Option 2: Repository-Level Caching**

```csharp
public class CachedTeamRepo : IIdentityTeamRepo
{
    private readonly IIdentityTeamRepo _innerRepo;
    private readonly IMemoryCache _cache;
    
    public async Task<Team?> FirstOrDefaultAsync<TSpec>(TSpec specification, CancellationToken cancellationToken = default) 
        where TSpec : ISpecification<Team>
    {
        // Cache specific specifications for system teams
        if (specification is SuperTeamWithMembersSpec superSpec)
        {
            var cacheKey = $"SuperTeam_Members_{superSpec.MaxPosition}";
            if (_cache.TryGetValue(cacheKey, out Team? cachedTeam))
                return cachedTeam;
                
            var team = await _innerRepo.FirstOrDefaultAsync(specification, cancellationToken);
            if (team != null)
            {
                _cache.Set(cacheKey, team, TimeSpan.FromMinutes(30));
            }
            return team;
        }
        
        if (specification is MntcTeamWithMembersSpec mntcSpec)
        {
            var cacheKey = $"MntcTeam_Members_{mntcSpec.MaxPosition}";
            if (_cache.TryGetValue(cacheKey, out Team? cachedTeam))
                return cachedTeam;
                
            var team = await _innerRepo.FirstOrDefaultAsync(specification, cancellationToken);
            if (team != null)
            {
                _cache.Set(cacheKey, team, TimeSpan.FromMinutes(30));
            }
            return team;
        }

        // No caching for other specifications
        return await _innerRepo.FirstOrDefaultAsync(specification, cancellationToken);
    }
}
```

#### **Option 3: Decorator Pattern (Most Flexible)**

```csharp
public class SystemTeamCacheDecorator : IIdentityTeamManager<TUser>
{
    private readonly IIdentityTeamManager<TUser> _inner;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SystemTeamCacheDecorator> _logger;

    public async Task<Team?> GetSuperTeamWithMembersAsync(int maxPosition = 1000)
    {
        var cacheKey = $"SystemTeams:Super:Members:{maxPosition}";
        
        if (_cache.TryGetValue(cacheKey, out Team? team))
        {
            _logger.LogDebug("Cache hit for Super team with {MaxPosition} members", maxPosition);
            return team;
        }

        _logger.LogDebug("Cache miss for Super team, fetching from database");
        team = await _inner.GetSuperTeamWithMembersAsync(maxPosition);
        
        if (team != null)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                Priority = CacheItemPriority.High
            };
            
            _cache.Set(cacheKey, team, cacheOptions);
            _logger.LogDebug("Cached Super team for {CacheKey}", cacheKey);
        }
        
        return team;
    }

    // Similar implementation for GetMntcTeamWithMembersAsync
    
    // Pass through all other methods without caching
    public async Task<Team> CreateTeamAsync(Team newTeam, CancellationToken cancellationToken) =>
        await _inner.CreateTeamAsync(newTeam, cancellationToken);
    
    // ... other methods
}
```

### Cache Invalidation Strategy 🔄

#### **When to Invalidate Cache**

```csharp
public class TeamCacheInvalidationService
{
    private readonly IMemoryCache _cache;
    
    public async Task InvalidateSystemTeamCache(TeamType teamType)
    {
        var patterns = teamType switch
        {
            TeamType.Super => new[] { "SuperTeam_", "SystemTeams:Super:" },
            TeamType.Maintenance => new[] { "MntcTeam_", "SystemTeams:Mntc:" },
            _ => Array.Empty<string>()
        };
        
        foreach (var pattern in patterns)
        {
            // Remove all cache entries matching pattern
            // (Implementation depends on caching library)
            RemoveCacheEntriesWithPattern(pattern);
        }
    }
    
    // Call this when:
    // - Super/Maintenance team members are added/removed
    // - Team properties are updated
    // - Team structure changes
}
```

#### **Domain Event Integration**

```csharp
public class TeamMembershipService : ITeamMembershipService
{
    private readonly TeamCacheInvalidationService _cacheInvalidation;
    
    public async Task<GenResult<TUser>> RegisterMemberAsync(TUser newMember)
    {
        var result = await base.RegisterMemberAsync(newMember);
        
        // Invalidate cache if this is a system team
        if (result.Succeeded && IsSystemTeam(Team.TeamType))
        {
            await _cacheInvalidation.InvalidateSystemTeamCache(Team.TeamType);
        }
        
        return result;
    }
    
    private static bool IsSystemTeam(TeamType teamType) =>
        teamType is TeamType.Super or TeamType.Maintenance;
}
```

### Configuration Options ⚙️

#### **Cache Settings**

```csharp
public class TeamCacheOptions
{
    public TimeSpan SuperTeamCacheDuration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan MaintenanceTeamCacheDuration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan MaxCacheDuration { get; set; } = TimeSpan.FromHours(2);
    public bool EnableCaching { get; set; } = true;
    public CacheItemPriority Priority { get; set; } = CacheItemPriority.High;
}
```

#### **DI Registration**

```csharp
// In Program.cs or Startup.cs
services.Configure<TeamCacheOptions>(configuration.GetSection("TeamCaching"));

services.AddMemoryCache();

// Option 1: Decorator pattern
services.AddScoped<IIdentityTeamManager<AppUser>, TeamManagerService<AppUser>>();
services.Decorate<IIdentityTeamManager<AppUser>, SystemTeamCacheDecorator<AppUser>>();

// Option 2: Service-specific caching
services.AddScoped<ITeamQueryService, TeamQueryService>();
services.Decorate<ITeamQueryService, CachedTeamQueryService>();
```

### Performance Benefits 📈

#### **Expected Performance Gains**

```csharp
// Without caching:
// - Database query: ~50-100ms
// - Entity materialization: ~10-20ms
// - Navigation property loading: ~20-50ms
// Total: ~80-170ms per request

// With caching:
// - Memory lookup: ~0.1-1ms
// - Object deserialization: ~1-5ms
// Total: ~1-6ms per request

// Performance improvement: 95-99% faster
```

#### **Memory Usage Considerations**

```csharp
// Typical memory usage for system teams:
// - Super team with 10 members: ~2-5KB
// - Maintenance team with 5 members: ~1-3KB
// - Total cache footprint: <10KB

// With 1000 concurrent requests:
// - Without cache: 1000 DB connections
// - With cache: 1-2 DB connections + 10KB memory
```

### Monitoring and Metrics 📊

#### **Cache Performance Tracking**

```csharp
public class CacheMetricsService
{
    private readonly IMetrics _metrics;
    
    public void RecordCacheHit(string teamType, string operation)
    {
        _metrics.Counter("team_cache_hits")
               .WithTag("team_type", teamType)
               .WithTag("operation", operation)
               .Increment();
    }
    
    public void RecordCacheMiss(string teamType, string operation)
    {
        _metrics.Counter("team_cache_misses")
               .WithTag("team_type", teamType)
               .WithTag("operation", operation)
               .Increment();
    }
}
```

#### **Health Checks**

```csharp
public class TeamCacheHealthCheck : IHealthCheck
{
    private readonly IMemoryCache _cache;
    
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if system teams are cached
            var superTeamCached = _cache.TryGetValue("SuperTeam_WithMembers", out _);
            var mntcTeamCached = _cache.TryGetValue("MntcTeam_WithMembers", out _);
            
            var data = new Dictionary<string, object>
            {
                ["SuperTeamCached"] = superTeamCached,
                ["MaintenanceTeamCached"] = mntcTeamCached
            };
            
            return Task.FromResult(HealthCheckResult.Healthy("Team cache is functioning", data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Team cache failed", ex));
        }
    }
}
```

### Implementation Recommendations 🎯

#### **Start Simple, Scale Up**

1. **Phase 1**: Add basic memory caching to most accessed methods
2. **Phase 2**: Implement proper cache invalidation
3. **Phase 3**: Add monitoring and metrics
4. **Phase 4**: Consider distributed caching if needed

#### **Best Practices**

- ✅ **Use sliding expiration** for frequently accessed data
- ✅ **Set absolute expiration** to prevent stale data
- ✅ **Monitor cache hit ratios** to validate effectiveness
- ✅ **Invalidate proactively** when data changes
- ✅ **Use cache-aside pattern** for consistency
- ✅ **Handle cache failures gracefully** (fallback to database)

#### **When NOT to Cache**

- ❌ **Customer teams** - Too many variations, change frequently
- ❌ **Real-time operations** - Where stale data could cause issues
- ❌ **Large datasets** - Memory constraints
- ❌ **Sensitive data** - Security/compliance concerns

---

## Questions to Consider 🤔

1. **Which methods are used most frequently?** (Keep in main service)
2. **What's the right balance** between convenience and separation?
3. **How should caching work** across services?
4. **Should services share common interfaces** or be completely independent?
5. **How to handle cross-cutting concerns** like logging, validation?

## Next Steps 🚀

1. **Review this document** and refine the approach
2. **Start with TeamMembershipService** as proof of concept
3. **Implement and test** one service completely
4. **Get feedback** from team/users
5. **Iterate and improve** the pattern
6. **Scale to other services** if successful

---

## Notes
- This pattern follows the **"Facade + Strategy"** design pattern
- Similar to how **Microsoft's UserManager** provides specialized functionality
- Maintains **backward compatibility** during migration
- Provides **clear upgrade path** for existing code

*Created: June 10, 2025*
*Status: Planning Phase*
