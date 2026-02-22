using System.Text.Json;
using fsCore.Common.Models;
using fsCore.DataImporter.MockModelBuilders;

namespace fsCore.Tests.ModelTests
{
    public class LiveMatchTests : TestBase
    {
        [Fact]
        public void LiveMatch_Rules_Should_Validate_Catches_Correctly_With_Specific_Catch_Rule()
        {
            //Arrange

            var liveMatchId = Guid.NewGuid();
            var specificSpeciesRules = new SpecificSpeciesLiveMatchCatchRule(
                ["salmon", "pike"],
                []
            );
            var validCatch = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            validCatch.Species = "salmon";
            var validCatch2 = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            validCatch2.Species = "pike";
            var invalidCatch = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            invalidCatch.Species = "catfish";
            var catches = new List<LiveMatchCatch> { validCatch, validCatch2, invalidCatch };
            var liveMatchRules = new LiveMatchRules([specificSpeciesRules]);
            var LiveMatch = new LiveMatch(
                Guid.NewGuid(),
                "test match",
                liveMatchRules,
                LiveMatchStatus.InProgress,
                LiveMatchWinStrategy.HighestSingleWeight,
                catches,
                [LiveMatchParticipant.FromUser(MockUserBuilder.Build())!],
                Guid.NewGuid(),
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                "",
                liveMatchId
            );
            var catchValidator = LiveMatch.MatchRules.BuildMatchCatchValidator();

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

            var topLeft = new LatLng((decimal)34.0522, (decimal)-118.2437);
            var bottomLeft = new LatLng((decimal)33.0522, (decimal)-118.2437);
            var topRight = new LatLng((decimal)34.0522, (decimal)-117.2437);
            var bottomRight = new LatLng((decimal)33.0522, (decimal)-117.2437);

            var geoArea = new FourPointGeoArea(topLeft, bottomLeft, topRight, bottomRight);
            var withinAreasRules = new InAreaLiveMatchCatchRule([geoArea]);
            var validCatch = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            validCatch.Latitude = (decimal)33.5522;
            validCatch.Longitude = (decimal)-117.7437;
            var validCatch2 = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            validCatch2.Latitude = (decimal)33.8022;
            validCatch2.Longitude = (decimal)-118.0437;
            var invalidCatch = MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId);
            invalidCatch.Latitude = (decimal)35.0522;
            invalidCatch.Longitude = (decimal)-119.2437;
            var catches = new List<LiveMatchCatch> { validCatch, validCatch2, invalidCatch };
            var liveMatchRules = new LiveMatchRules([withinAreasRules]);
            var LiveMatch = new LiveMatch(
                Guid.NewGuid(),
                "test match",
                liveMatchRules,
                LiveMatchStatus.InProgress,
                LiveMatchWinStrategy.HighestSingleWeight,
                catches,
                [LiveMatchParticipant.FromUser(MockUserBuilder.Build())!],
                Guid.NewGuid(),
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                "",
                liveMatchId
            );
            var catchValidator = LiveMatch.MatchRules.BuildMatchCatchValidator();

            //Act && Assert
            foreach (var catchItem in LiveMatch.Catches)
            {
                var result = catchValidator.Validate(catchItem);
                if (
                    catchItem.Latitude == (decimal)33.5522
                        && catchItem.Longitude == (decimal)-117.7437
                    || catchItem.Latitude == (decimal)33.8022
                        && catchItem.Longitude == (decimal)-118.0437
                )
                {
                    Assert.True(result.IsValid);
                }
                else
                {
                    Assert.False(result.IsValid);
                }
            }
        }

        private class Json_Serialisation_Should_Work_Through_Caching_Types_Class_Data
            : TheoryData<LiveMatch>
        {
            public Json_Serialisation_Should_Work_Through_Caching_Types_Class_Data()
            {
                var withinAreaRuleLiveMatchId = Guid.NewGuid();

                var topLeft = new LatLng((decimal)34.0522, (decimal)-118.2437);
                var bottomLeft = new LatLng((decimal)33.0522, (decimal)-118.2437);
                var topRight = new LatLng((decimal)34.0522, (decimal)-117.2437);
                var bottomRight = new LatLng((decimal)33.0522, (decimal)-117.2437);

                var geoArea = new FourPointGeoArea(topLeft, bottomLeft, topRight, bottomRight);
                var withinAreasRules = new InAreaLiveMatchCatchRule([geoArea]);
                var withinAreaRuleValidCatch = MockLiveMatchCatchBuilder.Build(
                    Guid.NewGuid(),
                    withinAreaRuleLiveMatchId
                );
                withinAreaRuleValidCatch.Latitude = (decimal)33.5522;
                withinAreaRuleValidCatch.Longitude = (decimal)-117.7437;
                var withinAreaRuleValidCatch2 = MockLiveMatchCatchBuilder.Build(
                    Guid.NewGuid(),
                    withinAreaRuleLiveMatchId
                );
                withinAreaRuleValidCatch2.Latitude = (decimal)33.8022;
                withinAreaRuleValidCatch2.Longitude = (decimal)-118.0437;
                var withinAreaRuleInvalidCatch = MockLiveMatchCatchBuilder.Build(
                    Guid.NewGuid(),
                    withinAreaRuleLiveMatchId
                );
                withinAreaRuleInvalidCatch.Latitude = (decimal)35.0522;
                withinAreaRuleInvalidCatch.Longitude = (decimal)-119.2437;
                var withinAreaRuleCatches = new List<LiveMatchCatch>
                {
                    withinAreaRuleValidCatch,
                    withinAreaRuleValidCatch2,
                    withinAreaRuleInvalidCatch,
                };
                var withinAreaRuleLiveMatchRules = new LiveMatchRules([withinAreasRules]);
                var withinAreaMatchUsers = new List<LiveMatchParticipant>()
                {
                    LiveMatchParticipant.FromUser(MockUserBuilder.Build())!,
                };

                var LiveMatchWithInAreaRule = new LiveMatch(
                    Guid.NewGuid(),
                    "test match",
                    withinAreaRuleLiveMatchRules,
                    LiveMatchStatus.InProgress,
                    LiveMatchWinStrategy.HighestSingleWeight,
                    withinAreaRuleCatches,
                    withinAreaMatchUsers,
                    withinAreaMatchUsers[0].Id,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    "",
                    withinAreaRuleLiveMatchId
                );
                Add(LiveMatchWithInAreaRule);

                var specificSpeciesRuleLiveMatchId = Guid.NewGuid();
                var specificSpeciesRules = new SpecificSpeciesLiveMatchCatchRule(
                    ["salmon", "pike"],
                    []
                );
                var specificSpeciesRuleValidCatch = MockLiveMatchCatchBuilder.Build(
                    Guid.NewGuid(),
                    specificSpeciesRuleLiveMatchId
                );
                withinAreaRuleValidCatch.Species = "salmon";
                var specificSpeciesRuleValidCatch2 = MockLiveMatchCatchBuilder.Build(
                    Guid.NewGuid(),
                    specificSpeciesRuleLiveMatchId
                );
                withinAreaRuleValidCatch2.Species = "pike";
                var specificSpeciesRuleInvalidCatch = MockLiveMatchCatchBuilder.Build(
                    Guid.NewGuid(),
                    specificSpeciesRuleLiveMatchId
                );
                withinAreaRuleInvalidCatch.Species = "catfish";
                var specificSpeciesRuleCatches = new List<LiveMatchCatch>
                {
                    specificSpeciesRuleValidCatch,
                    specificSpeciesRuleValidCatch2,
                    specificSpeciesRuleInvalidCatch,
                };
                var specificSpeciesRuleLiveMatchRules = new LiveMatchRules([specificSpeciesRules]);
                var specificRuleMatchUsers = new List<LiveMatchParticipant>()
                {
                    LiveMatchParticipant.FromUser(MockUserBuilder.Build())!,
                };
                var LiveMatchWithSpecificSpeciesRule = new LiveMatch(
                    Guid.NewGuid(),
                    "test match",
                    specificSpeciesRuleLiveMatchRules,
                    LiveMatchStatus.InProgress,
                    LiveMatchWinStrategy.HighestSingleWeight,
                    specificSpeciesRuleCatches,
                    specificRuleMatchUsers,
                    specificRuleMatchUsers[0].Id,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    "",
                    specificSpeciesRuleLiveMatchId
                );
                Add(LiveMatchWithSpecificSpeciesRule);

                var liveMatchWithBothRules = new LiveMatch(LiveMatchWithSpecificSpeciesRule)
                {
                    MatchRules = new LiveMatchRules([withinAreasRules, specificSpeciesRules]),
                };
                Add(liveMatchWithBothRules);
            }
        }

        [Theory]
        [ClassData(typeof(Json_Serialisation_Should_Work_Through_Caching_Types_Class_Data))]
        public void Json_Serialisation_Should_Work_Through_Caching_Types(LiveMatch liveMatch)
        {
            //Arrange
            var liveMatchCacheType = liveMatch.ToJsonType();
            var json = JsonSerializer.Serialize(liveMatchCacheType);

            //Act
            var jsonResult = JsonSerializer.Deserialize<LiveMatchJsonType>(json);
            var actual = jsonResult?.ToRuntimeType();

            //Assert
            Assert.Equal(liveMatch, actual);
        }
    }
}
