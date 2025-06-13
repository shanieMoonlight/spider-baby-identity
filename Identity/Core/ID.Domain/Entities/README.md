# Domain Entities - Architectural Pattern Documentation

## Value Object Input / Primitive Property Storage Pattern

This folder contains domain entities that follow a specific architectural pattern designed to balance domain modeling best practices with Entity Framework Core compatibility and query performance.

## Pattern Overview

Our domain entities use **value objects for input parameters** but store data as **primitive properties** internally. This hybrid approach provides the benefits of both approaches while avoiding their respective drawbacks.

### Example Pattern

```csharp
public class Team : AggregateRoot<TeamId>
{
    // Primitive properties for storage (EF Core compatible)
    public string TeamName { get; private set; }
    public string TeamDescription { get; private set; }
    
    // Value objects used for input validation and domain operations
    public void UpdateTeamName(Name teamName, IUpdateTeamNameValidator validator)
    {
        // Value object provides validation and domain semantics
        ArgumentNullException.ThrowIfNull(teamName);
        validator.ValidateCanUpdateTeamName(this);
        
        // Store as primitive for EF Core compatibility
        TeamName = teamName.Value;
        
        // Domain events, etc.
        AddDomainEvent(new TeamNameUpdatedEvent(Id, teamName));
    }
}
```

## Why This Pattern?

### 1. EF Core Compatibility
- **Problem**: EF Core has limited support for value object mapping and LINQ translation
- **Solution**: Primitive properties map directly to database columns without configuration complexity
- **Benefit**: Simple, reliable database queries without translation issues

### 2. Query Simplicity
- **Problem**: Value objects in LINQ queries can cause translation failures
- **Solution**: Primitive properties work seamlessly with LINQ to SQL
- **Benefit**: Developers can write natural queries without worrying about EF limitations

```csharp
// This works reliably with primitives
var teams = await context.Teams
    .Where(t => t.TeamName.Contains("Engineering"))
    .ToListAsync();

// This might fail with value objects due to translation issues
```

### 3. Domain Validation at Construction
- **Problem**: Need to ensure data integrity and business rules
- **Solution**: Value objects validate input at method boundaries
- **Benefit**: Rich domain validation while maintaining storage simplicity

### 4. Performance Optimization
- **Problem**: Value object mapping can add overhead to queries
- **Solution**: Direct primitive mapping is fastest and most efficient
- **Benefit**: Optimal query performance for read-heavy scenarios

## Implementation Guidelines

### Input Parameters
- Always use value objects for method parameters that represent domain concepts
- Leverage value object validation and business rules
- Provide clear domain semantics through typed parameters

### Storage Properties
- Use primitive types (string, int, DateTime, etc.) for entity properties
- Keep setters private to maintain encapsulation
- Map directly to database columns without complex configuration

### Domain Events
- Include value objects in domain events for rich event data
- Consumers can benefit from typed, validated event properties
- Maintains domain semantics in the event stream

## Benefits Summary

1. **Domain Clarity**: Value objects provide clear domain semantics and validation
2. **EF Compatibility**: Primitive storage ensures reliable ORM mapping
3. **Query Performance**: Direct primitive queries are fast and predictable
4. **Maintenance**: Simple storage pattern reduces configuration complexity
5. **Testing**: Easy to test both domain logic and data access separately

## Alternative Approaches Considered

### Full Value Object Storage
- **Pros**: Pure domain modeling, rich type safety
- **Cons**: EF Core complexity, LINQ translation issues, performance overhead
- **Decision**: Rejected due to practical ORM limitations

### Full Primitive Approach
- **Pros**: Simple storage, fast queries
- **Cons**: Loss of domain semantics, reduced validation, primitive obsession
- **Decision**: Rejected due to domain modeling concerns

### Current Hybrid Approach
- **Pros**: Domain semantics + practical storage + query performance
- **Cons**: Slight complexity in having both input and storage representations
- **Decision**: Adopted as optimal balance for our requirements

## Future Considerations

- Monitor EF Core improvements in value object support
- Consider migration to full value objects if/when EF limitations are resolved
- Maintain consistency across all domain entities using this pattern

---

This pattern ensures our domain layer provides rich modeling capabilities while maintaining practical performance and compatibility with our chosen technology stack.
