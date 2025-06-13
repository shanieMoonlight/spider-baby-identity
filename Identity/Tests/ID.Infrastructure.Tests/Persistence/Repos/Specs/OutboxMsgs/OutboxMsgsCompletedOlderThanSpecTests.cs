using Xunit.Abstractions;

namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.OutboxMsgs;

public class OutboxMsgsCompletedOlderThanSpecTests(ITestOutputHelper output)
{
    [Fact]
    public void Constructor_SetsCriteriaCorrectly()
    {
        // Arrange
        var daysAgo = 1;
        var date = DateTime.UtcNow.AddDays(-daysAgo - 5);
        //processedOn => nonnull, Created => before daysago
        var msg = OutboxMessageDataFactory.Create(createdOn: date, processedOn: date.AddHours(-1), error: string.Empty);

        // Act
        var spec = new OutboxMsgsCompletedOlderThanSpec(daysAgo);
        var criteria = spec.TESTING_GetCriteria().Compile();
        var result = criteria(msg);


        // Assert
        var what1 = msg.CreatedOnUtc < DateTime.Now.AddDays(-daysAgo);
        var what2 = msg.ProcessedOnUtc != null;
        var what3 = string.IsNullOrWhiteSpace(msg.Error);
        var what = msg.CreatedOnUtc < DateTime.Now.AddDays(-daysAgo)
            && msg.ProcessedOnUtc != null
            && string.IsNullOrWhiteSpace(msg.Error);

        output.WriteLine($"msg.CreatedOnUtc < DateTime.Now.AddDays(-daysAgo): {what1}");
        output.WriteLine($"msg.ProcessedOnUtc != null: {what2}");
        output.WriteLine($"string.IsNullOrWhiteSpace(msg.Error): {what3}");
        output.WriteLine($"Criteria: {what}");
        output.WriteLine($"msg.ProcessedOnUtc:  {msg.ProcessedOnUtc}");

        criteria(msg).ShouldBeTrue();
    }

    //------------------------------------//

    [Fact]
    public void Constructor_SetsCriteriaCorrectly_FALSE_IfNewerDate()
    {
        // Arrange
        var daysAgo = 1;
        var date = DateTime.UtcNow.AddDays(1);
        //processedOn => nonnull, Created => before daysago
        var om = OutboxMessageDataFactory.Create(createdOn: DateTime.Now, processedOn: date, error: string.Empty);

        // Act
        var spec = new OutboxMsgsCompletedOlderThanSpec(daysAgo);
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert

        var what1 = om.CreatedOnUtc < DateTime.Now.AddDays(-daysAgo);
        var what2 = om.ProcessedOnUtc != null;
        var what3 = string.IsNullOrWhiteSpace(om.Error);
        var what = om.CreatedOnUtc < DateTime.Now.AddDays(-daysAgo)
            && om.ProcessedOnUtc != null
            && string.IsNullOrWhiteSpace(om.Error);
        output.WriteLine($"msg.CreatedOnUtc < DateTime.Now.AddDays(-daysAgo): {what1}");
        output.WriteLine($"msg.ProcessedOnUtc != null: {what2}");
        output.WriteLine($"string.IsNullOrWhiteSpace(msg.Error): {what3}");
        output.WriteLine($"Criteria: {what}");
        output.WriteLine($"msg.ProcessedOnUtc:  {om.ProcessedOnUtc}");

        criteria(om).ShouldBeFalse();
    }

    //------------------------------------//

    [Fact]
    public void ShouldShortCircuit_ReturnsTrue_WhenDaysIsNegative()
    {
        // Arrange
        var spec = new OutboxMsgsCompletedOlderThanSpec(-1);

        // Act
        var result = spec.ShouldShortCircuit();

        // Assert
        result.ShouldBeTrue();
    }

    //------------------------------------//
}
