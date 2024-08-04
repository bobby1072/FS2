using Common.Models;
using Common.Models.MiscModels;
using DataImporter.MockModelBuilders;

namespace fsCore.Tests.ModelTests
{
    public class LiveMatchTests
    {
        [Fact]
        public void LiveMatch_Rules_Should_Validate_Catches_Correctly_With_Specific_Catch_Rule()
        {
            //Arrange

            var liveMatchId = Guid.NewGuid();
            var specificSpeciesRules = new SpecificSpeciesLiveMatchCatchRule(new List<string> { "salmon", "pike" }, new List<WorldFish>(), Guid.NewGuid());
            var validCatch = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            validCatch.Species = "salmon";
            var validCatch2 = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            validCatch2.Species = "pike";
            var invalidCatch = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            invalidCatch.Species = "catfish";
            var catches = new List<LiveMatchCatch> { validCatch, validCatch2, invalidCatch };
            var liveMatchRules = new LiveMatchRules(new List<LiveMatchSingleRule> { specificSpeciesRules });
            var LiveMatch = new LiveMatch(Guid.NewGuid(), "test match", liveMatchRules, LiveMatchStatus.InProgress, LiveMatchWinStrategy.HighestSingleWeight, catches, new[] { MockUserBuilder.Build() }, liveMatchId);
            var catchValidator = LiveMatch.MatchRules.BuildMatchRulesValidator();

            //Act && Assert
            foreach (var catchItem in LiveMatch.Catches)
            {
                var result = catchValidator.Validate(catchItem);
                if (catchItem.Species == "salmon" || catchItem.Species == "pike")
                {
                    Assert.True(result.IsValid);
                }
                else
                {
                    Assert.False(result.IsValid);
                }
            }
        }
        [Fact]
        public void LiveMatch_Rules_Should_Validate_Catches_Correctly_With_In_Area_Rule()
        {
            //Arrange
            var liveMatchId = Guid.NewGuid();

            var topLeft = new LatLng(34.0522, -118.2437);
            var bottomLeft = new LatLng(33.0522, -118.2437);
            var topRight = new LatLng(34.0522, -117.2437);
            var bottomRight = new LatLng(33.0522, -117.2437);

            var geoArea = new FourPointGeoArea(topLeft, bottomLeft, topRight, bottomRight);
            var withinAreasRules = new InAreaLiveMatchCatchRule(new List<FourPointGeoArea> { geoArea });
            var validCatch = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            validCatch.Latitude = 33.5522;
            validCatch.Longitude = -117.7437;
            var validCatch2 = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            validCatch2.Latitude = 33.8022;
            validCatch2.Longitude = -118.0437;
            var invalidCatch = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            invalidCatch.Latitude = 35.0522;
            invalidCatch.Longitude = -119.2437;
            var catches = new List<LiveMatchCatch> { validCatch, validCatch2, invalidCatch };
            var liveMatchRules = new LiveMatchRules(new List<LiveMatchSingleRule> { withinAreasRules });
            var LiveMatch = new LiveMatch(Guid.NewGuid(), "test match", liveMatchRules, LiveMatchStatus.InProgress, LiveMatchWinStrategy.HighestSingleWeight, catches, new[] { MockUserBuilder.Build() }, liveMatchId);
            var catchValidator = LiveMatch.MatchRules.BuildMatchRulesValidator();

            //Act && Assert
            foreach (var catchItem in LiveMatch.Catches)
            {
                var result = catchValidator.Validate(catchItem);
                if (catchItem.Latitude == 33.5522 && catchItem.Longitude == -117.7437 || catchItem.Latitude == 33.8022 && catchItem.Longitude == -118.0437)
                {
                    Assert.True(result.IsValid);
                }
                else
                {
                    Assert.False(result.IsValid);
                }
            }
        }

    }
}