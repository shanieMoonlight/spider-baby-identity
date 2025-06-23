namespace ID.Infrastructure.Tests.Utility;

public class RandomTokenGeneratorTests
{
    [Fact]
    public void Generate_ShouldReturnUniqueTokens_OnMultipleCalls()
    {
        // Arrange
        const int iterations = 20;
        var tokens = new HashSet<string>();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var token = RandomTokenGenerator.Generate();
            token.ShouldNotBeNullOrWhiteSpace();
            tokens.Add(token);
        }

        // Assert
        tokens.Count.ShouldBe(iterations, "Tokens should all be unique");
    }

    [Theory]
    [InlineData(10, 20)]
    [InlineData(50, 60)]
    [InlineData(100, 120)]
    public void Generate_ShouldReturnTokenOfExpectedDecodedLength(int min, int max)
    {
        // Act
        var token = RandomTokenGenerator.Generate(min, max);
        var bytes = Convert.FromBase64String(token);
        bytes.Length.ShouldBeGreaterThanOrEqualTo(min);
        bytes.Length.ShouldBeLessThan(max);
    }
}
