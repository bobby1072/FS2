using Common.Misc;
using Common.Models;
using Common.Utils;
using DataImporter.MockModelBuilders;
using FluentAssertions;

namespace fsCore.Tests.UtilTests
{
    public class LiveMatchUtilsTests
    {
        private class Should_Calculate_LiveMatch_Winner_Correctly_Class_Data : TheoryData<LiveMatch, User>
        {
            public Should_Calculate_LiveMatch_Winner_Correctly_Class_Data()
            {
                var participants = new[] {
                    LiveMatchParticipant.FromUser(MockUserBuilder.Build()),
                    LiveMatchParticipant.FromUser(MockUserBuilder.Build()),
                    LiveMatchParticipant.FromUser(MockUserBuilder.Build())
                };
                var liveMatch = new LiveMatch(Guid.NewGuid(), "test match", new LiveMatchRules([]), LiveMatchStatus.InProgress, LiveMatchWinStrategy.HighestSingleWeight, [], participants, participants.FirstOrDefault()!.Id!, DateTime.UtcNow);

                var firstSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 20,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 21,true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 23,true),
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 24,true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 25,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 243, true),
                };
                liveMatch.Catches = firstSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.HighestSingleWeight;
                Add(liveMatch.JsonClone()!, participants[0]);

                var secondSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 20,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 21,true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 25,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 23,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 24,true),
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 243, true),
                };
                liveMatch.Catches = secondSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.MostCatches;
                Add(liveMatch.JsonClone()!, participants[0]);

                var thirdSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 20, true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 21, true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 23, true),
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 24, true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 25, true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 243, true),
                };
                liveMatch.Catches = thirdSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.HighestTotalWeight;
                Add(liveMatch.JsonClone()!, participants[0]);

                var fourthSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 20),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "Pike", 2, 21, true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 23, true),
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 24,true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 25,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 243, true),
                };

                liveMatch.Catches = fourthSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.MostSpecies;
                Add(liveMatch.JsonClone()!, participants[0]);

                var fifthSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 20,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 10, 21,true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 23,true),
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 24,true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 25,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 243,true),
                };

                liveMatch.Catches = fifthSetOfCatches;
                liveMatch.MatchWinStrategy = LiveMatchWinStrategy.LongestInTotal;
                Add(liveMatch.JsonClone()!, participants[0]);

                var sixthSetOfCatches = new[]{
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 200, 20,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 21,true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 23,true),
                    MockLiveMatchCatchBuilder.Build(participants[1].Id!, liveMatch.Id, "salmon", 2, 24,true),
                    MockLiveMatchCatchBuilder.Build(participants[2].Id!, liveMatch.Id, "salmon", 2, 25,true),
                    MockLiveMatchCatchBuilder.Build(participants[0].Id!, liveMatch.Id, "salmon", 2, 243,true),
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
            winner.Should().BeEquivalentTo(expectedWinner);
        }

    }
}