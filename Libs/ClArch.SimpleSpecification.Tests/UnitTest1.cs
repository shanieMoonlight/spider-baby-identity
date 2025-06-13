using Shouldly;

namespace ClArch.SimpleSpecification.Tests;

// Test entity for testing purposes
public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<TestChildEntity> Children { get; set; } = [];
}

public class TestChildEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int ParentId { get; set; }
    public TestEntity Parent { get; set; } = null!;
}