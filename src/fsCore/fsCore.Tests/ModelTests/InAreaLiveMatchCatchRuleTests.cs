using Common.Models;
using FluentAssertions;

namespace fsCore.Tests.ModelTests
{
    public class InAreaLiveMatchCatchRuleTests : TestBase
    {
        [Fact]
        public void BuildRuleDescription_ShouldReturnAreas()
        {
            // Arrange
            var fourPointGeoAreas = new List<FourPointGeoArea>
            {
                new(new(1, 1), new(1, 2), new(2, 1), new(2, 2))
            };
            var inAreaLiveMatchCatchRule = new InAreaLiveMatchCatchRule(fourPointGeoAreas);

            // Act
            var result = inAreaLiveMatchCatchRule.BuildRuleDescription();

            // Assert
            result.Should().Be("InAreaLiveMatchCatchRule: [{\"topLeft\":{\"latitude\":1,\"longitude\":1},\"bottomLeft\":{\"latitude\":1,\"longitude\":2},\"topRight\":{\"latitude\":2,\"longitude\":1},\"bottomRight\":{\"latitude\":2,\"longitude\":2}}]");
        }
    }
}