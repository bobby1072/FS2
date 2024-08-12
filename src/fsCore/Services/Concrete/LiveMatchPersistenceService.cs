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
        public async Task SetLiveMatch(LiveMatch liveMatch)
        {
            var liveMatchFromDb = await _activeLiveMatchRepository.GetFullOneById(liveMatch.Id);
            if (liveMatchFromDb is null)
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
                    throw new LiveMatchException(LiveMatchConstants.FailedToPersistLiveMatch);
                }
            }
            else if (!liveMatchFromDb.Equals(liveMatch))
            {
                if (!liveMatch.ValidateAgainstOriginal(liveMatchFromDb))
                {
                    throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
                }
                if (liveMatch.Catches.SequenceEqual(liveMatchFromDb.Catches))
                {
                    var savedResult = await _activeLiveMatchRepository.Update([liveMatch]);
                    if (savedResult is not null)
                    {

                        if (liveMatch.MatchStatus == LiveMatchStatus.InProgress)
                        {
                            await _cachingService.SetObject($"{_liveMatchKey}{liveMatch.Id}", liveMatch.ToJsonType());
                        }
                    }
                    else
                    {
                        throw new LiveMatchException(LiveMatchConstants.FailedToPersistLiveMatch);
                    }
                }
                else
                {
                    var liveMatchCreateJob = _activeLiveMatchRepository.Update([liveMatch]);
                    var liveMatchCatchCreateJob = _activeLiveMatchCatchRepository.Update(liveMatch.Catches);
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
                        throw new LiveMatchException(LiveMatchConstants.FailedToPersistLiveMatch);
                    }
                }
            }
            else
            {
                if (liveMatch.MatchStatus == LiveMatchStatus.InProgress)
                {
                    await _cachingService.SetObject($"{_liveMatchKey}{liveMatch.Id}", liveMatch.ToJsonType());
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
    }
}