using Xunit.Abstractions;

namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.OutboxMsgs;

public class OutboxMsgsRemoveOlderThanSpecTests(ITestOutputHelper _output)
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var daysAgo = 1;
        var date = DateTime.UtcNow.AddDays(-(daysAgo + 5));
        var msg = OutboxMessageDataFactory.Create(
            createdOn: date,
            processedOn: DateTime.Now.AddHours(-1),
            error: string.Empty);

        // Act
        var spec = new OutboxMsgsRemoveOlderThanSpec(daysAgo);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        var what1 = msg.CreatedOnUtc < DateTime.Now.AddDays(-daysAgo);
        var what2 = msg.ProcessedOnUtc != null;
        var what3 = string.IsNullOrWhiteSpace(msg.Error);
        var what = msg.CreatedOnUtc < DateTime.Now.AddDays(-daysAgo)
            && msg.ProcessedOnUtc != null
            && string.IsNullOrWhiteSpace(msg.Error);
        _output.WriteLine($"msg.CreatedOnUtc < DateTime.Now.AddDays(-daysAgo): {what1}");
        _output.WriteLine($"msg.ProcessedOnUtc != null:  {what2}");
        _output.WriteLine($"string.IsNullOrWhiteSpace(msg.Error): {what3}");
        _output.WriteLine($"Criteria: {criteria(msg)}");
        _output.WriteLine($"Criteria(msg): {what}");
        _output.WriteLine($"msg.ProcessedOnUtc:  {msg.ProcessedOnUtc}");

        criteria(msg).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfNewerDate()
    {
        // Arrange
        var daysAgo = 10;
        var date = DateTime.UtcNow.AddDays(-5);
        var msg = OutboxMessageDataFactory.Create(createdOn: date, error: string.Empty);

        // Act
        var spec = new OutboxMsgsRemoveOlderThanSpec(daysAgo);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(msg).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void ShouldShortCircuit_ReturnsTrue_WhenDaysIsNegative()
    {
        // Arrange
        var spec = new OutboxMsgsRemoveOlderThanSpec(-1);

        // Act
        var result = spec.ShouldShortCircuit();

        // Assert
        result.ShouldBeTrue();
    }

    //------------------------------------//
}
