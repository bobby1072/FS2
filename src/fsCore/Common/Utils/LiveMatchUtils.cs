using Common.Models;

namespace Common.Utils
{
    public static class LiveMatchUtils
    {
        public static string GetWinStrategyName(LiveMatchWinStrategy winStrategy)
        {
            return winStrategy switch
            {
                LiveMatchWinStrategy.LongestSingleCatch => "Longest Single Catch",
                LiveMatchWinStrategy.LongestInTotal => "Longest In Total",
                LiveMatchWinStrategy.MostCatches => "Most Catches",
                LiveMatchWinStrategy.HighestTotalWeight => "Highest Total Weight",
                LiveMatchWinStrategy.HighestSingleWeight => "Highest Single Weight",
                LiveMatchWinStrategy.MostSpecies => "Most Species",
                _ => "Unknown",
            };
        }
        public static User CalculateWinner(LiveMatch liveMatch)
        {
            if (liveMatch.MatchWinStrategy == LiveMatchWinStrategy.HighestSingleWeight)
            {
                var test = liveMatch.Participants.FirstOrDefault(u => u.Id == FindHighestSingleWeightWinner(liveMatch.Catches));
                return test;
            }
            return liveMatch.MatchWinStrategy switch
            {
                LiveMatchWinStrategy.LongestSingleCatch => liveMatch.Catches.Any() ? liveMatch.Participants.FirstOrDefault(u => u.Id == FindLongestSingleCatchWinner(liveMatch.Catches)) : throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches),
                LiveMatchWinStrategy.LongestInTotal => liveMatch.Catches.Any() ? liveMatch.Participants.FirstOrDefault(u => u.Id == FindLongestInTotalWinner(liveMatch.Catches)) : throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches),
                LiveMatchWinStrategy.MostCatches => liveMatch.Catches.Any() ? liveMatch.Participants.FirstOrDefault(u => u.Id == FindMostCatchesWinner(liveMatch.Catches)) : throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches),
                LiveMatchWinStrategy.HighestTotalWeight => liveMatch.Catches.Any() ? liveMatch.Participants.FirstOrDefault(u => u.Id == FindHighestTotalWeightWinner(liveMatch.Catches)) : throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches),
                LiveMatchWinStrategy.HighestSingleWeight => liveMatch.Catches.Any() ? liveMatch.Participants.FirstOrDefault(u => u.Id == FindHighestSingleWeightWinner(liveMatch.Catches)) : throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches),
                LiveMatchWinStrategy.MostSpecies => liveMatch.Catches.Any() ? liveMatch.Participants.FirstOrDefault(u => u.Id == FindMostSpeciesWinner(liveMatch.Catches)) : throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches),
                _ => throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails),
            } ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches);

        }
        private static Guid FindLongestSingleCatchWinner(IList<LiveMatchCatch> catches)
        {
            return catches.OrderByDescending(c => c.Length).FirstOrDefault()?.UserId ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches);
        }
        private static Guid FindLongestInTotalWinner(IList<LiveMatchCatch> catches)
        {
            return catches.GroupBy(c => c.UserId).OrderByDescending(g => g.Sum(c => c.Length)).FirstOrDefault()?.Key ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches);
        }
        private static Guid FindMostCatchesWinner(IList<LiveMatchCatch> catches)
        {
            return catches.GroupBy(c => c.UserId).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches);
        }
        private static Guid FindHighestTotalWeightWinner(IList<LiveMatchCatch> catches)
        {
            return catches.GroupBy(c => c.UserId).OrderByDescending(g => g.Sum(c => c.Weight)).FirstOrDefault()?.Key ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches);
        }
        private static Guid FindHighestSingleWeightWinner(IList<LiveMatchCatch> catches)
        {
            var winner = catches.OrderByDescending(c => c.Weight).FirstOrDefault()?.UserId ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches);
            return winner;
        }
        private static Guid FindMostSpeciesWinner(IList<LiveMatchCatch> catches)
        {
            return catches.GroupBy(c => c.UserId).OrderByDescending(g => g.SelectMany(c => c.Species).Distinct().Count()).FirstOrDefault()?.Key ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasNoCatches);
        }
    }
}