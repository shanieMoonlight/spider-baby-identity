using System.Linq.Expressions;
using ClArch.SimpleSpecification;
using Microsoft.EntityFrameworkCore;

namespace ClArch.SimpleSpecification.Tests;

// Test specifications for testing purposes
public class TestEntityByIdSpec : ASimpleSpecification<TestEntity>
{
    public TestEntityByIdSpec(int id) : base(e => e.Id == id)
    {
    }
}

public class TestEntityByNameSpec : ASimpleSpecification<TestEntity>
{
    public TestEntityByNameSpec(string name) : base(e => e.Name == name)
    {
        SetShortCircuit(() => string.IsNullOrWhiteSpace(name));
    }
}

public class TestEntityByAgeRangeSpec : ASimpleSpecification<TestEntity>
{
    public TestEntityByAgeRangeSpec(int minAge, int maxAge) : base(e => e.Age >= minAge && e.Age <= maxAge)
    {
        SetOrderBy(q => q.OrderBy(e => e.Age));
    }
}

public class TestEntityWithChildrenSpec : ASimpleSpecification<TestEntity>
{
    public TestEntityWithChildrenSpec() : base(e => e.IsActive)
    {
        SetInclude<List<TestChildEntity>>(q => q.Include(e => e.Children));
    }
}

public class TestEntityPaginatedSpec : ASimpleSpecification<TestEntity>
{
    public TestEntityPaginatedSpec(int skip, int take) : base(e => e.IsActive)
    {
        SetSkip(skip);
        SetTake(take);
        SetOrderBy(q => q.OrderBy(e => e.Name));
    }
}

public class TestEntityWithShortCircuitSpec : ASimpleSpecification<TestEntity>
{
    public TestEntityWithShortCircuitSpec(bool shouldShortCircuit) : base(e => e.IsActive)
    {
        SetShortCircuit(() => shouldShortCircuit);
    }
}
