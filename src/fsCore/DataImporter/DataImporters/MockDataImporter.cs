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
        private readonly IGroupPositionImporter _groupPositionImporter;
        private readonly IGroupPositionRepository _positionRepository;
        private readonly IGroupCatchRepository _groupCatchRepository;
        public MockDataImporter(IUserImporter userImporter, ILogger<MockDataImporter> logger, IUserRepository userRepository, IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository, IGroupPositionRepository positionRepository, IGroupCatchRepository groupCatchRepository, IGroupImporter groupImporter, IGroupPositionImporter groupPositionImporter)
        {
            _userImporter = userImporter;
            _groupPositionImporter = groupPositionImporter;
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
                var userCountJob = _userRepository.GetCount();
                var groupCountJob = _groupRepository.GetCount();
                var groupMemberCountJob = _groupMemberRepository.GetCount();
                var groupPositionCountJob = _positionRepository.GetCount();
                var groupCatchCountJob = _groupCatchRepository.GetCount();
                await Task.WhenAll(userCountJob, groupCountJob, groupMemberCountJob, groupPositionCountJob, groupCatchCountJob);
                if (userCountJob.Result == 0)
                {
                    await _userImporter.Import();
                }
                if (groupCatchCountJob.Result == 0)
                {
                    await _groupImporter.Import();
                }
                if (groupPositionCountJob.Result == 0)
                {
                    await _groupPositionImporter.Import();
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to import mock data, the error was: {0}", e);
            }
        }
    }
}