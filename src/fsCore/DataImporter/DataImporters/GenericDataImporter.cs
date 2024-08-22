using Common;
using DataImporter.DataImporters.ModelImporters.Abstract;
using Hangfire;
using Microsoft.Extensions.Logging;
using Persistence.EntityFramework.Repository.Abstract;
namespace DataImporter.DataImporters
{
    internal class GenericDataImporter : IDataImporter
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GenericDataImporter> _logger;
        private readonly IUserImporter _userImporter;
        private readonly IGroupImporter _groupImporter;
        private readonly IGroupPositionImporter _groupPositionImporter;
        private readonly IGroupMemberImporter _groupMemberImporter;
        private readonly IGroupCatchImporter _groupCatchImporter;
        public GenericDataImporter(IUserImporter userImporter, ILogger<GenericDataImporter> logger, IUserRepository userRepository, IGroupImporter groupImporter, IGroupPositionImporter groupPositionImporter, IGroupMemberImporter groupMemberImporter, IGroupCatchImporter groupCatchImporter)
        {
            _groupCatchImporter = groupCatchImporter;
            _groupMemberImporter = groupMemberImporter;
            _userImporter = userImporter;
            _groupPositionImporter = groupPositionImporter;
            _logger = logger;
            _groupImporter = groupImporter;
            _userRepository = userRepository;
        }
        [Queue(HangfireConstants.Queues.StartUpJobs)]
        [AutomaticRetry(Attempts = 5, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail, DelaysInSeconds = [1])]
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
                await Task.Delay(1500);
                await _groupImporter.Import();
                await Task.Delay(1500);
                await _groupPositionImporter.Import();
                await Task.Delay(1500);
                await _groupMemberImporter.Import();
                await Task.Delay(1500);
                await _groupCatchImporter.Import();
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to import mock data, the error was: {E}", e);
                await _userRepository.DeleteAll();
                throw new ApiException(e);
            }
        }
    }
}