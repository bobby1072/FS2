using AutoFixture;
using Common.Models;
using Common.Utils;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Persistence.EntityFramework.Repository.Abstract;
using Services.Abstract;
using Services.Concrete;

namespace fsCore.Tests.ServiceTests
{
    public class LiveMatchPersistenceServiceTests : TestBase
    {
        private const string _liveMatchKey = "match-";
        private readonly Mock<ICachingService> _mockCachingService;
        private readonly Mock<IActiveLiveMatchCatchRepository> _mockActiveLiveMatchCatchRepository;
        private readonly Mock<IActiveLiveMatchRepository> _mockActiveLiveMatchRepository;
        private readonly Mock<IActiveLiveMatchParticipantRepository> _mockActiveLiveMatchParticipantRepository;
        private readonly LiveMatchPersistenceService _liveMatchPersistenceService;
        public LiveMatchPersistenceServiceTests()
        {
            _mockActiveLiveMatchCatchRepository = new Mock<IActiveLiveMatchCatchRepository>();
            _mockActiveLiveMatchRepository = new Mock<IActiveLiveMatchRepository>();
            _mockCachingService = new Mock<ICachingService>();
            _mockActiveLiveMatchParticipantRepository = new Mock<IActiveLiveMatchParticipantRepository>();
            _liveMatchPersistenceService = new LiveMatchPersistenceService(_mockCachingService.Object, _mockActiveLiveMatchCatchRepository.Object, _mockActiveLiveMatchRepository.Object, _mockActiveLiveMatchParticipantRepository.Object);
        }
        [Fact]
        public async Task TryGetMatch_Should_Return_Match_If_Cached()
        {
            // Arrange
            var liveMatch = CreateLiveMatch();
            _mockCachingService.Setup(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}")).ReturnsAsync(liveMatch.ToJsonType());

            // Act
            var result = await _liveMatchPersistenceService.TryGetLiveMatch(liveMatch.Id);

            // Assert
            _mockCachingService.Verify(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}"), Times.Once);
            _mockActiveLiveMatchRepository.Verify(x => x.GetFullOneById(It.IsAny<Guid>()), Times.Never);
            result.Should().BeEquivalentTo(liveMatch);

        }
        [Theory]
        [InlineData(LiveMatchStatus.NotStarted)]
        [InlineData(LiveMatchStatus.InProgress)]
        public async Task TryGetMatch_Should_Return_MatchFrom_Db_If_Cached_Returns_Null(LiveMatchStatus status)
        {
            //Arrange
            var liveMatch = CreateLiveMatch();
            liveMatch.MatchStatus = status;
            _mockCachingService.Setup(x => x.TryGetObject<LiveMatchJsonType>(It.IsAny<string>())).ReturnsAsync((LiveMatchJsonType?)null);
            _mockActiveLiveMatchRepository.Setup(x => x.GetFullOneById(liveMatch.Id)).ReturnsAsync(liveMatch);

            //Act
            var result = await _liveMatchPersistenceService.TryGetLiveMatch(liveMatch.Id);

            //Assert
            if (status == LiveMatchStatus.InProgress)
            {
                _mockCachingService.Verify(x => x.SetObject($"{_liveMatchKey}{liveMatch.Id}", It.Is<LiveMatchJsonType>(x => x.Id == liveMatch.Id), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
            }
            else
            {
                _mockCachingService.Verify(x => x.SetObject(It.IsAny<string>(), It.IsAny<LiveMatch>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
            }
            _mockCachingService.Verify(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}"), Times.Once);
            _mockActiveLiveMatchRepository.Verify(x => x.GetFullOneById(liveMatch.Id), Times.Once);
            result.Should().BeEquivalentTo(liveMatch);
        }
        [Fact]
        public async Task TryGetMatch_Should_Return_Null_If_TryGetObject_Throws_Exception()
        {
            //Arrange
            var liveMatch = CreateLiveMatch();
            _mockCachingService.Setup(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}")).ThrowsAsync(new Exception());

            //Act
            var result = await _liveMatchPersistenceService.TryGetLiveMatch(liveMatch.Id);

            //Assert
            _mockCachingService.Verify(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}"), Times.Once);
            _mockActiveLiveMatchRepository.Verify(x => x.GetFullOneById(It.IsAny<Guid>()), Times.Never);
            result.Should().BeNull();
        }
        [Fact]
        public async Task TryGetMatch_Should_Return_Null_If_GetFullOneById_Throws_Exception()
        {
            //Arrange
            var liveMatch = CreateLiveMatch();
            _mockCachingService.Setup(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}")).ReturnsAsync((LiveMatchJsonType?)null);
            _mockActiveLiveMatchRepository.Setup(x => x.GetFullOneById(liveMatch.Id)).ThrowsAsync(new Exception());
            //Act
            var result = await _liveMatchPersistenceService.TryGetLiveMatch(liveMatch.Id);

            //Assert
            _mockCachingService.Verify(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}"), Times.Once);
            _mockActiveLiveMatchRepository.Verify(x => x.GetFullOneById(It.IsAny<Guid>()), Times.Once);
            result.Should().BeNull();
        }
        [Theory]
        [InlineData(LiveMatchStatus.NotStarted)]
        [InlineData(LiveMatchStatus.InProgress)]
        public async Task SetLiveMatch_Should_Create_The_Match_If_Doesnt_Exist(LiveMatchStatus status) 
        {
            //Arrange
            var liveMatch = CreateLiveMatch();
            liveMatch.MatchStatus = status;
            _mockActiveLiveMatchRepository.Setup(x => x.GetFullOneById(It.IsAny<Guid>())).ReturnsAsync((LiveMatch?)null);
            _mockCachingService.Setup(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}")).ReturnsAsync((LiveMatchJsonType?)null);
            _mockActiveLiveMatchRepository.Setup(x => x.Create(It.Is<ICollection<LiveMatch>>(y => y.FirstOrDefault()!.Id == liveMatch.Id))).ReturnsAsync([liveMatch]);
            _mockCachingService.Setup(x => x.SetObject($"{_liveMatchKey}{liveMatch.Id.ToString()}", It.Is<LiveMatchJsonType>(y => y.Id == liveMatch.Id), It.IsAny<DistributedCacheEntryOptions>())).ReturnsAsync("fdgfdsgyfdsghfdsghghsdj");

            //Act
            await _liveMatchPersistenceService.SetLiveMatch(liveMatch);

            //Assert
            _mockActiveLiveMatchRepository.Verify(x => x.Create(It.Is<ICollection<LiveMatch>>(y => y.FirstOrDefault()!.Id == liveMatch.Id)), Times.Once);
            if(status == LiveMatchStatus.InProgress)
            {
                _mockCachingService.Verify(x => x.SetObject($"{_liveMatchKey}{liveMatch.Id.ToString()}", It.Is<LiveMatchJsonType>(y => y.Id == liveMatch.Id), It.IsAny<DistributedCacheEntryOptions>()), Times.Once);
            }
            else
            {
                _mockCachingService.Verify(x => x.SetObject($"{_liveMatchKey}{liveMatch.Id.ToString()}", It.Is<LiveMatchJsonType>(y => y.Id == liveMatch.Id), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
            }
        }
        private static LiveMatch CreateLiveMatch()
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