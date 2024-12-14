using AutoFixture;
using fsCore.Common.Models;
using fsCore.Common.Utils;

namespace fsCore.Tests.ServiceTests
{
    public abstract class LiveMatchTestBase : TestBase
    {
        protected static LiveMatch CreateLiveMatch()
        {
            var rules = new LiveMatchRules();
            var liveMatch = _fixture
                .Build<LiveMatch>()
                .With(x => x.MatchRules, rules)
                .With(x => x.CreatedAt, DateTimeUtils.RandomPastDate().Invoke())
                .With(x => x.CommencesAt, DateTime.UtcNow)
                .With(x => x.EndsAt, DateTimeUtils.RandomFutureDate().Invoke())
                .Create();
            return liveMatch;
        }
    }
}