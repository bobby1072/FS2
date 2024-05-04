using Common.DbInterfaces.Repository;
using DataImporter.ModelImporters;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace DataImporter
{
    internal class MockDataImporter : IDataImporter
    {
        private readonly IUserImporter _userImporter;
        private readonly ILogger<MockDataImporter> _logger;
        private readonly IGroupImporter _groupImporter;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IGroupPositionRepository _positionRepository;
        private readonly IGroupCatchRepository _groupCatchRepository;
        public MockDataImporter(IUserImporter userImporter, ILogger<MockDataImporter> logger, IUserRepository userRepository, IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository, IGroupPositionRepository positionRepository, IGroupCatchRepository groupCatchRepository, IGroupImporter groupImporter)
        {
            _userImporter = userImporter;
            _logger = logger;
            _groupImporter = groupImporter;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _positionRepository = positionRepository;
            _groupCatchRepository = groupCatchRepository;
        }
        [AutomaticRetry(Attempts = 3, LogEvents = true)]
        public async Task Import()
        {
            try
            {
                var allCountJobsList = new[] { _groupCatchRepository.GetCount(), _groupMemberRepository.GetCount(), _groupRepository.GetCount(), _positionRepository.GetCount(), _userRepository.GetCount() };
                var allCountJobs = await Task.WhenAll(allCountJobsList);
                var completedJobs = allCountJobsList.Select(x => x.Result).ToArray();
                if (completedJobs.Any(x => x > 0))
                {
                    return;
                }
                await _userImporter.Import();
                await _groupImporter.Import();
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to import mock data, the error was: {0}", e);
            }
        }
    }
}