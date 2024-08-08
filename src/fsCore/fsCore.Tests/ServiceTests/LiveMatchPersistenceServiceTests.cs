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
            //Arrange
            var liveMatch = _fixture.Build<LiveMatch>().With(x => x.MatchRules.Rules, [new SpecificSpeciesLiveMatchCatchRule(["dd"], [])]).Create();
            _mockCachingService.Setup(x => x.TryGetObject<LiveMatch>(liveMatch.Id.ToString())).ReturnsAsync(liveMatch);

            //Act
            var result = await _liveMatchPersistenceService.TryGetLiveMatch(liveMatch.Id);

            //Assert
            Assert.Equal(liveMatch, result);
            _mockCachingService.Verify(x => x.TryGetObject<LiveMatch>(liveMatch.Id.ToString()), Times.Once);

        }
    }
}