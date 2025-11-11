using ID.Domain.Repos.Specs.NewFolder.OutboxMsgs;
using Xunit.Abstractions;

namespace ID.Domain.Repos.Tests.Specs.OutboxMsgs;

public class OutboxMsgsUnprocessedSpecTests(ITestOutputHelper _output)
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

        _output.WriteLine($"msg: {msg}");
        _output.WriteLine($"ProcessedOn: {msg.ProcessedOnUtc}");

        // Assert
        criteria(msg).ShouldBeFalse();
    }


}//Cls