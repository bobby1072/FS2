using AutoFixture;
using Common.Models;
using Moq;
using Persistence.EntityFramework.Repository.Abstract;
using Services.Abstract;
using Services.Concrete;

namespace fsCore.Tests.ServiceTests
{
    public class LiveMatchPersistenceServiceTests : TestBase
    {
        private const string _liveMatchKey = "match-";
        private Mock<ICachingService> _mockCachingService;
        private readonly Mock<IActiveLiveMatchCatchRepository> _mockActiveLiveMatchCatchRepository;
        private readonly Mock<IActiveLiveMatchRepository> _mockActiveLiveMatchRepository;
        private readonly LiveMatchPersistenceService _liveMatchPersistenceService;
        public LiveMatchPersistenceServiceTests()
        {
            _mockActiveLiveMatchCatchRepository = new Mock<IActiveLiveMatchCatchRepository>();
            _mockActiveLiveMatchRepository = new Mock<IActiveLiveMatchRepository>();
            _mockCachingService = new Mock<ICachingService>();
            _liveMatchPersistenceService = new LiveMatchPersistenceService(_mockCachingService.Object, _mockActiveLiveMatchCatchRepository.Object, _mockActiveLiveMatchRepository.Object);
        }
        [Fact]
        public async Task Should_Return_Match_If_Cached()
        {
            // Arrange
            var liveMatch = CreateLiveMatch();
            _mockCachingService.Setup(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}")).ReturnsAsync(liveMatch.ToJsonType());

            // Act
            var result = await _liveMatchPersistenceService.TryGetLiveMatch(liveMatch.Id);

            // Assert
            _mockCachingService.Verify(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}"), Times.Once);
            _mockActiveLiveMatchRepository.Verify(x => x.GetFullOneById(It.IsAny<Guid>()), Times.Never);
            Assert.Equal(liveMatch, result);

        }
        private LiveMatch CreateLiveMatch()
        {
            var rules = new LiveMatchRules();
            var liveMatch = _fixture.Build<LiveMatch>().With(x => x.MatchRules, rules).Create();
            return liveMatch;
        }
    }
}