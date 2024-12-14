using fsCore.Common.Misc.Abstract;
using fsCore.Common.Models;
using FluentValidation;
using Hangfire;
using Moq;
using Persistence.EntityFramework.Repository.Abstract;
using Services.Abstract;
using Services.Concrete;

namespace fsCore.Tests.ServiceTests
{
    public class LiveMatchServiceTests : LiveMatchTestBase
    {
        private readonly Mock<IBackgroundJobClient> _backgroundJobClient;
        private readonly Mock<ILiveMatchHubContextServiceProvider> _liveMatchHubContextServiceProvider;
        private readonly Mock<ILiveMatchPersistenceService> _liveMatchPersistenceService;
        private readonly Mock<IGroupService> _groupService;
        private readonly Mock<IActiveLiveMatchParticipantRepository> _liveMatchParticipantRepository;
        private readonly Mock<IActiveLiveMatchRepository>_liveMatchRepository;
        private readonly Mock<IValidator<LiveMatch>> _liveMatchValidator;
        private readonly Mock<IValidator<LiveMatchCatch>> _liveMatchCatchValidator;
        private readonly LiveMatchService _liveMatchService;
        public LiveMatchServiceTests()
        {
            _backgroundJobClient = new Mock<IBackgroundJobClient>();
            _liveMatchHubContextServiceProvider = new Mock<ILiveMatchHubContextServiceProvider>();
            _liveMatchPersistenceService = new Mock<ILiveMatchPersistenceService>();
            _groupService = new Mock<IGroupService>();
            _liveMatchRepository = new Mock<IActiveLiveMatchRepository>();
            _liveMatchParticipantRepository = new Mock<IActiveLiveMatchParticipantRepository>();
            _liveMatchValidator = new Mock<IValidator<LiveMatch>>();
            SetupValidator(_liveMatchValidator);

            _liveMatchCatchValidator = new Mock<IValidator<LiveMatchCatch>>();
            SetupValidator(_liveMatchCatchValidator);

            _liveMatchService = new LiveMatchService(_backgroundJobClient.Object, _liveMatchHubContextServiceProvider.Object, _liveMatchPersistenceService.Object, _liveMatchParticipantRepository.Object,_liveMatchRepository.Object,_liveMatchValidator.Object, _liveMatchCatchValidator.Object, _groupService.Object);
        }
    }
}