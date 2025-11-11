using ID.Domain.Repos.Specs.NewFolder.OutboxMsgs;

namespace ID.Infrastructure.Tests.Persistence.Repos.Specs.OutboxMsgs;

public class OutboxMsgsUnprocessedSpecTests
{
    [Fact]
    public void Criteria_ReturnsTrue_If_NotProcessed()
    {
        // Arrange
        var msg = IdOutboxMessageDataFactory.Create(
            processedOn: null
        );

        var spec = UnprocessedOutboxMsgsSpec.Create();
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        criteria(msg).ShouldBeTrue();
    }

    //--------------------------// 

    [Fact]
    public void Criteria_ReturnsFalse_If_Processed()
    {
        // Arrange
        var msg = IdOutboxMessageDataFactory.Create(
            processedOn: DateTime.UtcNow.AddDays(-1)
        );

        var spec = UnprocessedOutboxMsgsSpec.Create();
        var criteria = spec.TESTING_GetCriteria().Compile();

        // Assert
        Console.WriteLine($" criteria(msg): {criteria(msg)}");
        Console.WriteLine($" msg: {msg}");

        criteria(msg).ShouldBeFalse();
    }


}//Cls