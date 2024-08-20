using AutoFixture;
using Common.Models;
using DataImporter.MockModelBuilders;
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
        private readonly LiveMatchPersistenceService _liveMatchPersistenceService;
        public LiveMatchPersistenceServiceTests()
        {
            _mockActiveLiveMatchCatchRepository = new Mock<IActiveLiveMatchCatchRepository>();
            _mockActiveLiveMatchRepository = new Mock<IActiveLiveMatchRepository>();
            _mockCachingService = new Mock<ICachingService>();
            _liveMatchPersistenceService = new LiveMatchPersistenceService(_mockCachingService.Object, _mockActiveLiveMatchCatchRepository.Object, _mockActiveLiveMatchRepository.Object);
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
            Assert.Equal(liveMatch, result);

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
                _mockCachingService.Verify(x => x.SetObject($"{_liveMatchKey}{liveMatch.Id.ToString()}", It.Is<LiveMatchJsonType>(x => x.Id == liveMatch.Id), It.IsAny<DistributedCacheEntryOptions?>()), Times.Once);
            }
            else
            {
                _mockCachingService.Verify(x => x.SetObject(It.IsAny<string>(), It.IsAny<LiveMatch>(), It.IsAny<DistributedCacheEntryOptions?>()), Times.Never);
            }
            _mockCachingService.Verify(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id.ToString()}"), Times.Once);
            _mockActiveLiveMatchRepository.Verify(x => x.GetFullOneById(liveMatch.Id), Times.Once);
            Assert.Equal(liveMatch, result);
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
            Assert.Null(result);
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
            Assert.Null(result);
        }
        internal class SetLiveMatchCatches_Should_Return_If_Catch_Lists_Are_The_Same_Class_Data : TheoryData<List<LiveMatchCatch>, List<LiveMatchCatch>, bool>
        {
            public SetLiveMatchCatches_Should_Return_If_Catch_Lists_Are_The_Same_Class_Data()
            {
                var liveMatchId = Guid.NewGuid();
                var tenCatches = Enumerable.Range(0, 10).Select(x => MockLiveMatchCatchBuilder.Build(Guid.NewGuid(), liveMatchId)).ToList();
                Add(tenCatches, tenCatches, true);
                Add(tenCatches, tenCatches.Take(5).ToList(), false);

            }
        }
        [Theory]
        [ClassData(typeof(SetLiveMatchCatches_Should_Return_If_Catch_Lists_Are_The_Same_Class_Data))]
        public async Task SetLiveMatchCatches_Should_Return_If_Catch_Lists_Are_The_Same(List<LiveMatchCatch> catches1, List<LiveMatchCatch> catches2, bool isTheSame)
        {
            //Arrange
            var liveMatch = CreateLiveMatch();
            liveMatch.Catches = catches1;
            _mockCachingService.Setup(x => x.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{liveMatch.Id}")).ReturnsAsync(liveMatch.ToJsonType());
            _mockActiveLiveMatchRepository.Setup(x => x.GetFullOneById(liveMatch.Id)).ReturnsAsync(liveMatch);
            _mockActiveLiveMatchCatchRepository.Setup(x => x.Create(It.IsAny<ICollection<LiveMatchCatch>>())).ReturnsAsync([]);
            _mockActiveLiveMatchCatchRepository.Setup(x => x.Update(It.IsAny<ICollection<LiveMatchCatch>>())).ReturnsAsync([]);


            //Act
            await _liveMatchPersistenceService.SetLiveMatchCatches(liveMatch.Id, catches2);

            //Assert
            if (isTheSame is false)
            {
                _mockActiveLiveMatchCatchRepository.Verify(x => x.Create(It.IsAny<ICollection<LiveMatchCatch>>()), Times.AtLeastOnce);
            }
            else
            {
                _mockActiveLiveMatchCatchRepository.Verify(x => x.Create(It.IsAny<ICollection<LiveMatchCatch>>()), Times.Never);
                _mockActiveLiveMatchCatchRepository.Verify(x => x.Update(It.IsAny<ICollection<LiveMatchCatch>>()), Times.Never);
            }
        }
        private static LiveMatch CreateLiveMatch()
        {
            var rules = new LiveMatchRules();
            var liveMatch = _fixture.Build<LiveMatch>().With(x => x.MatchRules, rules).Create();
            return liveMatch;
        }
    }
}