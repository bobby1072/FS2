using Common;
using Common.Models;
using Common.Utils;
using DataImporter.MockModelBuilders;

namespace fsCore.Tests.UtilTests
{
    public class LiveMatchUtilsTests
    {
        internal class Should_Calculate_LiveMatch_Winner_Correctly_Class_Data : TheoryData<LiveMatch, User>
        {
            public Should_Calculate_LiveMatch_Winner_Correctly_Class_Data()
            {
                var participants = new[] {
                    MockUserBuilder.Build(),
                    MockUserBuilder.Build(),
                    MockUserBuilder.Build()
                };
                var liveMatch = new LiveMatch(Guid.NewGuid(), "test match", new LiveMatchRules([]), LiveMatchStatus.InProgress, LiveMatchWinStrategy.HighestSingleWeight, [], participants, (Guid)participants.FirstOrDefault()!.Id!, DateTime.UtcNow);

                var firstSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 20),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 21),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 23),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 24),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 25),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 243),
                };
                liveMatch.Catches = firstSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.HighestSingleWeight;
                Add(liveMatch.JsonClone()!, participants[0]);

                var secondSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 20),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 21),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 25),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 23),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 24),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 243),
                };
                liveMatch.Catches = secondSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.MostCatches;
                Add(liveMatch.JsonClone()!, participants[0]);

                var thirdSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 20),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 21),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 23),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 24),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 25),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 243),
                };
                liveMatch.Catches = thirdSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.HighestTotalWeight;
                Add(liveMatch.JsonClone()!, participants[0]);

                var fourthSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 20),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "Pike", 2, 21),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 23),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 24),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 25),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 243),
                };

                liveMatch.Catches = fourthSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.MostSpecies;
                Add(liveMatch.JsonClone()!, participants[0]);

                var fifthSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 20),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 10, 21),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 23),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 24),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 25),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 243),
                };

                liveMatch.Catches = fifthSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.LongestInTotal;
                Add(liveMatch.JsonClone()!, participants[0]);

                var sixthSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 200, 20),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 21),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 23),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[1].Id!, liveMatch.Id, "salmon", 2, 24),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[2].Id!, liveMatch.Id, "salmon", 2, 25),
                    MockLiveMatchCatchBuilder.Build((Guid)participants[0].Id!, liveMatch.Id, "salmon", 2, 243),
                };

                liveMatch.Catches = sixthSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.LongestSingleCatch;
                Add(liveMatch.JsonClone()!, participants[1]);


            }
        }
        [Theory]
        [ClassData(typeof(Should_Calculate_LiveMatch_Winner_Correctly_Class_Data))]
        public void Should_Calculate_LiveMatch_Winner_Correctly(LiveMatch liveMatch, User expectedWinner)
        {
            //Act
            var winner = LiveMatchUtils.CalculateWinner(liveMatch);

            //Assert
            Assert.Equal(expectedWinner, winner);
        }

    }
}