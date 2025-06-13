# Pagination, Filtering, and Sorting Library - Backend Guide

This library provides extension methods to add dynamic filtering, sorting, and pagination to your `IQueryable<T>` and `IEnumerable<T>` collections. It automatically builds LINQ expressions from filter and sort requests sent by the frontend.

---

## Quick Start

### Basic Usage

```csharp
using Pagination.Extensions;

// For database queries (EF Core)
public Task<Page<Team>> GetTeamsAsync(PagedRequest request)
{
    var data = _context.Teams
        .AddFiltering(request.FilterList)  // Apply filters
        .AddEfSorting(request.SortList)    // Apply sorting (EF Core)
        .AsNoTracking();

    return Task.FromResult(new Page<Team>(data, request.PageNumber, request.PageSize));
}

// For in-memory collections
public Page<Team> GetTeamsInMemory(PagedRequest request, List<Team> teams)
{
    var data = teams
        .AddFiltering(request.FilterList)  // Apply filters
        .AddSorting(request.SortList);     // Apply sorting (in-memory)

    return new Page<Team>(data.AsQueryable(), request.PageNumber, request.PageSize);
}
```

---

## Extension Methods

### `AddFiltering()`

Applies filters to your queryable/enumerable collection based on `FilterRequest` objects.

#### For IQueryable (EF Core)
```csharp
IQueryable<T> AddFiltering<T>(
    this IQueryable<T> list, 
    IEnumerable<FilterRequest>? filterRequestList, 
    Func<string, string>? getPropertySelectorLambda = null, 
    ILogger? logger = null)
```

#### For IEnumerable (In-Memory)
```csharp
IEnumerable<T> AddFiltering<T>(
    this IEnumerable<T> list, 
    IEnumerable<FilterRequest> filterRequestList, 
    Func<string, string>? getPropertySelectorLambda = null, 
    ILogger? logger = null)
```

### `AddEfSorting()`

Applies sorting to your queryable collection based on `SortRequest` objects.

```csharp
IQueryable<T> AddEfSorting<T>(
    this IQueryable<T> list, 
    IEnumerable<SortRequest> sortRequestList, 
    CustomSortExpressionBuilder<T>? sortExpBuilder = null)
```

### `AddSorting()`

Applies sorting to your in-memory enumerable collection based on `SortRequest` objects.

```csharp
IEnumerable<T> AddSorting<T>(
    this IEnumerable<T> list, 
    IEnumerable<SortRequest> sortRequestList, 
    CustomSortFuncBuilder<T>? sortFuncBuilder = null)
```

---

## Handling Frontend Property Names

Often, frontend developers use different property names than your backend entities. Use the optional parameters to map between them.

### Property Filtering Mapping

```csharp
// Override in your repository base class
protected virtual Func<string, string>? GetFilteringPropertySelectorLambda() => 
    (frontendFieldName) => frontendFieldName switch
    {
        "teamname" => nameof(Team.Name),
        "leadername" => nameof(Team.Leader.UserName),
        _ => frontendFieldName // Default: use as-is
    };

// Usage
var data = _context.Teams
    .AddFiltering(filterList, GetFilteringPropertySelectorLambda())
    .AsNoTracking();
```

### Custom Sort Expression Builder

For complex sorting scenarios (nested properties, calculated fields), use `CustomSortExpressionBuilder<T>`:

```csharp
protected virtual CustomSortExpressionBuilder<Team>? GetSortBuilder() => 
    CustomSortExpressionBuilder<Team>
        .Create("teamname", team => team.Name)  // Map "teamname" to Team.Name
        .AddCustomSort("leadername", team => team.Leader.UserName)  // Nested property
        .AddCustomSort("membercount", team => team.Members.Count()); // Calculated field

// Usage with EF Core
var data = _context.Teams
    .AddEfSorting(sortList, GetSortBuilder())
    .AsNoTracking();
```

### Custom Sort Function Builder (In-Memory)

For in-memory collections, use `CustomSortFuncBuilder<T>`:

```csharp
protected virtual CustomSortFuncBuilder<Team>? GetSortFuncBuilder() => 
    CustomSortFuncBuilder<Team>
        .Create("teamname", team => team.Name)  // Map "teamname" to Team.Name
        .AddCustomSort("leadername", team => team.Leader?.UserName ?? "")  // Nested property
        .AddCustomSort("membercount", team => team.Members.Count); // Calculated field

// Usage with in-memory collections
var data = teamsList
    .AddSorting(sortList, GetSortFuncBuilder());
```

---

## Complete Repository Example

```csharp
public class TeamRepository : ITeamRepository
{
    private readonly DbContext _context;
    
    public TeamRepository(DbContext context)
    {
        _context = context;
    }

    public Task<Page<Team>> PageAsync(
        int pageNumber, 
        int pageSize, 
        IEnumerable<SortRequest> sortList, 
        IEnumerable<FilterRequest>? filterList = null)
    {
        var data = _context.Teams
            .Where(t => t.TeamType != TeamType.Super)  // Your business logic
            .AddFiltering(filterList, GetFilteringPropertySelectorLambda())
            .AddEfSorting(sortList, GetSortBuilder())
            .AsNoTracking();

        return Task.FromResult(new Page<Team>(data, pageNumber, pageSize));
    }

    protected virtual Func<string, string>? GetFilteringPropertySelectorLambda() => 
        (field) => field switch
        {
            "teamname" => nameof(Team.Name),
            "leadername" => "Leader.UserName",  // For nested properties
            _ => field
        };

    protected virtual CustomSortExpressionBuilder<Team>? GetSortBuilder() => 
        CustomSortExpressionBuilder<Team>
            .Create("teamname", team => team.Name)
            .AddCustomSort("leadername", team => team.Leader.UserName);
}
```

---

## Filter Types Support

The library automatically handles different data types and filter operations:

### String Filters
- `equals`, `not_equal_to`, `contains`, `starts_with`, `ends_with`, `in`

### Number/Date Filters  
- `equals`, `not_equal_to`, `greater_than`, `less_than`, `greater_than_or_equal_to`, `less_than_or_equal_to`, `between`, `in`

### Boolean Filters
- `equals`, `not_equal_to`

---

## Best Practices

### 1. Use Property Name Mapping
Always provide property mapping for frontend-to-backend field name translation:

```csharp
// Good
.AddFiltering(filterList, GetFilteringPropertySelectorLambda())

// Avoid - frontend field names must match backend exactly
.AddFiltering(filterList)
```

### 2. Handle Nested Properties
Use `CustomSortExpressionBuilder` for nested or calculated properties:

```csharp
// Good - handles Team.Leader.UserName properly
.AddCustomSort("leadername", team => team.Leader.UserName)

// Avoid - may cause EF translation issues
GetFilteringPropertySelectorLambda = field => field == "leadername" ? "Leader.UserName" : field
```

### 3. Add Logging
Pass a logger to catch and debug filter/sort issues:

```csharp
.AddFiltering(filterList, GetFilteringPropertySelectorLambda(), logger)
```

### 4. Apply Business Logic First
Apply your business filters before pagination filters:

```csharp
var data = _context.Teams
    .Where(t => t.IsActive)  // Business logic first
    .AddFiltering(filterList, GetFilteringPropertySelectorLambda())  // Then user filters
    .AddEfSorting(sortList, GetSortBuilder())
    .AsNoTracking();
```

---

## Error Handling

- Invalid filters are automatically skipped and logged (if logger provided)
- Invalid sort fields fall back to default property sorting
- Empty filter/sort lists are ignored (no performance impact)

---

## Performance Notes

- Use `AsNoTracking()` for read-only scenarios
- The library builds efficient LINQ expressions that translate well to SQL
- Filtering and sorting happen at the database level (not in memory)
- Always apply business logic filters before user filters

---

For frontend API documentation, see `README.frontend.md`.
