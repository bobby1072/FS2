using Common.Models;

namespace fsCore.Tests.ModelTests
{
    public class SpecificSpeciesLiveMatchCatchRuleTests : TestBase
    {
        [Fact]
        public void BuildRuleDescription_ShouldReturnSpeciesNames()
        {
            // Arrange
            var speciesNames = new List<string> { "species1", "species2" };
            var worldFish = new List<WorldFish>
            {
                new WorldFish("taxo1", "name1", "tst", "ddccsdsd", "ffvfcv", "dcdfcds"),
                new WorldFish("taxo2", "name2", "tdt", "ddcdd", "ffvfdvcdffcv", "gchecdbss")
            };
            var specificSpeciesLiveMatchCatchRule = new SpecificSpeciesLiveMatchCatchRule(speciesNames, worldFish);

            // Act
            var result = specificSpeciesLiveMatchCatchRule.BuildRuleDescription();

            // Assert
            Assert.Equal("SpecificSpeciesLiveMatchCatchRule: species1, species2", result);
        }
    }
}