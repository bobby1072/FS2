using System.Net;
using Common;
using Common.Models;
using Persistence.EntityFramework.Repository.Abstract;
using Services.Abstract;
namespace Services.Concrete
{
    public class LiveMatchPersistenceService : ILiveMatchPersistenceService
    {
        private const string _liveMatchKey = "match-";
        private readonly ICachingService _cachingService;
        private readonly IActiveLiveMatchCatchRepository _activeLiveMatchCatchRepository;
        private readonly IActiveLiveMatchRepository _activeLiveMatchRepository;
        public LiveMatchPersistenceService(ICachingService cachingService, IActiveLiveMatchCatchRepository activeLiveMatchCatchRepository, IActiveLiveMatchRepository activeLiveMatchRepository)
        {
            _cachingService = cachingService;
            _activeLiveMatchCatchRepository = activeLiveMatchCatchRepository;
            _activeLiveMatchRepository = activeLiveMatchRepository;
        }
        public async Task SetLiveMatchCatches(Guid liveMatchId, ICollection<LiveMatchCatch> liveMatchCatches)
        {
            var foundLiveMatch = await TryGetLiveMatch(liveMatchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (liveMatchCatches.Count == 0 || liveMatchCatches.SequenceEqual(foundLiveMatch.Catches))
            {
                return;
            }
            var catchesToCreateAndUpdate = await CreateAndUpdateCatchJobs(foundLiveMatch.Catches, liveMatchCatches) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (foundLiveMatch.MatchStatus == LiveMatchStatus.InProgress)
            {
                foundLiveMatch.Catches = liveMatchCatches.ToList();
                await _cachingService.SetObject($"{_liveMatchKey}{foundLiveMatch.Id}", foundLiveMatch.ToJsonType());
            }
        }
        public async Task SetLiveMatch(LiveMatch liveMatch)
        {
            var foundLiveMatch = await TryGetLiveMatch(liveMatch.Id);
            if (liveMatch.Equals(foundLiveMatch))
            {
                return;
            }
            else if (foundLiveMatch is null)
            {
                var liveMatchCreateJob = _activeLiveMatchRepository.Create([liveMatch]);
                var liveMatchCatchCreateJob = _activeLiveMatchCatchRepository.Create(liveMatch.Catches);
                await Task.WhenAll(liveMatchCreateJob, liveMatchCatchCreateJob);
                if (await liveMatchCreateJob is not null && await liveMatchCatchCreateJob is not null)
                {
                    if (liveMatch.MatchStatus == LiveMatchStatus.InProgress)
                    {
                        await _cachingService.SetObject($"{_liveMatchKey}{liveMatch.Id}", liveMatch.ToJsonType());
                    }
                }
                else
                {
                    throw new LiveMatchException(LiveMatchConstants.FailedToPersistLiveMatch, HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                var liveMatchCreateJob = _activeLiveMatchRepository.Update([liveMatch]);
                var liveMatchCatchCreateJob = CreateAndUpdateCatchJobs(foundLiveMatch.Catches, liveMatch.Catches);
                await Task.WhenAll(liveMatchCreateJob, liveMatchCatchCreateJob);
                if (await liveMatchCreateJob is not null && await liveMatchCatchCreateJob is not null)
                {
                    if (liveMatch.MatchStatus == LiveMatchStatus.InProgress)
                    {
                        await _cachingService.SetObject($"{_liveMatchKey}{liveMatch.Id}", liveMatch.ToJsonType());
                    }
                }
                else
                {
                    throw new LiveMatchException(LiveMatchConstants.FailedToPersistLiveMatch, HttpStatusCode.InternalServerError);
                }
            }
        }
        public async Task<LiveMatch?> TryGetLiveMatch(Guid matchId)
        {
            try
            {
                var cacheType = await _cachingService.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{matchId}");
                if (cacheType is not null)
                {
                    return cacheType.ToRuntimeType();
                }
                var liveMatch = await _activeLiveMatchRepository.GetFullOneById(matchId);
                if (liveMatch is not null)
                {
                    if (liveMatch.MatchStatus == LiveMatchStatus.InProgress)
                    {
                        await _cachingService.SetObject($"{_liveMatchKey}{matchId}", liveMatch.ToJsonType());
                        return (await _cachingService.TryGetObject<LiveMatchJsonType>($"{_liveMatchKey}{matchId}"))?.ToRuntimeType();
                    }
                    else
                    {
                        return liveMatch;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        private async Task<ICollection<LiveMatchCatch>?> CreateAndUpdateCatchJobs(ICollection<LiveMatchCatch> existingCatches, ICollection<LiveMatchCatch> catchesToInsert)
        {
            var catchesToUpdate = new List<LiveMatchCatch>();
            var catchesToCreate = new List<LiveMatchCatch>();

            foreach (var catchItem in catchesToInsert)
            {
                if (existingCatches.Any(existingCatch => existingCatch.Id == catchItem.Id))
                {
                    catchesToUpdate.Add(catchItem);
                }
                else
                {
                    catchesToCreate.Add(catchItem);
                }
            }

            var catchesToUpdateJob = catchesToUpdate.Any()
                ? _activeLiveMatchCatchRepository.Update(catchesToUpdate)
                : Task.FromResult<ICollection<LiveMatchCatch>?>(null);

            var catchesToCreateJob = catchesToCreate.Any()
                ? _activeLiveMatchCatchRepository.Create(catchesToCreate)
                : Task.FromResult<ICollection<LiveMatchCatch>?>(null);

            await Task.WhenAll(catchesToCreateJob, catchesToUpdateJob);

            var updatedCatches = await catchesToUpdateJob;
            var createdCatches = await catchesToCreateJob;

            return createdCatches is null || updatedCatches is null ? null : updatedCatches?.Union(createdCatches).ToArray();
        }
    }
}