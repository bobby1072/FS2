using Common;
using Common.DbInterfaces.Repository;
using DataImporter.ModelImporters;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace DataImporter
{
    internal class GenericDataImporter : IDataImporter
    {
        private readonly IUserImporter _userImporter;
        private readonly ILogger<GenericDataImporter> _logger;
        private readonly IGroupImporter _groupImporter;
        private readonly IUserRepository _userRepository;
        private readonly IGroupMemberImporter _groupMemberImporter;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IGroupPositionImporter _groupPositionImporter;
        private readonly IGroupPositionRepository _groupPositionRepository;
        private readonly IGroupCatchImporter _groupCatchImporter;
        private readonly IGroupCatchRepository _groupCatchRepository;
        public GenericDataImporter(IUserImporter userImporter, ILogger<GenericDataImporter> logger, IUserRepository userRepository, IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository, IGroupPositionRepository positionRepository, IGroupCatchRepository groupCatchRepository, IGroupImporter groupImporter, IGroupPositionImporter groupPositionImporter, IGroupMemberImporter groupMemberImporter, IGroupCatchImporter groupCatchImporter)
        {
            _groupCatchImporter = groupCatchImporter;
            _groupMemberImporter = groupMemberImporter;
            _userImporter = userImporter;
            _groupPositionImporter = groupPositionImporter;
            _logger = logger;
            _groupImporter = groupImporter;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _groupPositionRepository = positionRepository;
            _groupCatchRepository = groupCatchRepository;
        }
        [Queue(HangfireConstants.Queues.StartUpJobs)]
        [AutomaticRetry(Attempts = 2, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail, DelaysInSeconds = new[] { 10 })]
        public virtual async Task Import()
        {
            try
            {
                var userAmount = await _userRepository.GetCount();
                if (userAmount > 0)
                {
                    return;
                }

                await _userImporter.Import();

                await _groupImporter.Import();

                await _groupPositionImporter.Import();

                await _groupMemberImporter.Import();

                await _groupCatchImporter.Import();
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to import mock data, the error was: {0}", e);
                await Task.WhenAll(_userRepository.DeleteAll(), _groupRepository.DeleteAll(), _groupMemberRepository.DeleteAll(), _groupPositionRepository.DeleteAll(), _groupCatchRepository.DeleteAll());
            }
        }
    }
}