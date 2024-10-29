using Common.Misc;
using Common.Models;
using Microsoft.Extensions.Caching.Distributed;
using Persistence.EntityFramework.Repository.Abstract;
using Services.Abstract;
using System.Net;
namespace Services.Concrete
{
    public class LiveMatchPersistenceService : ILiveMatchPersistenceService
    {
        private const string _liveMatchKey = "match-";
        private readonly ICachingService _cachingService;
        private readonly IActiveLiveMatchCatchRepository _activeLiveMatchCatchRepository;
        private readonly IActiveLiveMatchRepository _activeLiveMatchRepository;
        private readonly IActiveLiveMatchParticipantRepository _activeLiveMatchParticipantRepository;
        public LiveMatchPersistenceService(ICachingService cachingService, IActiveLiveMatchCatchRepository activeLiveMatchCatchRepository, IActiveLiveMatchRepository activeLiveMatchRepository, IActiveLiveMatchParticipantRepository activeLiveMatchParticipantRepository)
        {
            _cachingService = cachingService;
            _activeLiveMatchCatchRepository = activeLiveMatchCatchRepository;
            _activeLiveMatchRepository = activeLiveMatchRepository;
            _activeLiveMatchParticipantRepository = activeLiveMatchParticipantRepository;
        }
        public async Task SaveParticipant(ICollection<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)> participants)
        {
            var matches = await _activeLiveMatchRepository.GetFullOneById(participants.Select(x => x.MatchId).ToArray()) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            var userToUpdateList = new List<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)>();
            var userToCreateList = new List<(LiveMatchParticipant LiveMatchParticipant, Guid MatchId)>();
            var cacheJobList = new List<Task<string>>();
            foreach (var participant in participants)
            {
                var foundLiveMatch = matches.FirstOrDefault(m => m.Id == participant.MatchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
                if (foundLiveMatch.Participants.FirstOrDefault(p => p.Id == participant.LiveMatchParticipant.Id) is null)
                {
                    userToCreateList.Add(participant);
                    if (foundLiveMatch.MatchStatus == LiveMatchStatus.InProgress || foundLiveMatch.MatchStatus == LiveMatchStatus.Finished)
                    {
                        foundLiveMatch.Participants = foundLiveMatch.Participants.Append(participant.LiveMatchParticipant).ToList();
                        cacheJobList.Add(_cachingService.SetObject($"{_liveMatchKey}{foundLiveMatch.Id}", foundLiveMatch.ToJsonType(), GetTimeToCache(foundLiveMatch)));
                    }
                }
                else
                {
                    userToUpdateList.Add(participant);
                    if (foundLiveMatch.MatchStatus == LiveMatchStatus.InProgress || foundLiveMatch.MatchStatus == LiveMatchStatus.Finished)
                    {
                        foundLiveMatch.Participants = foundLiveMatch.Participants.Select(p => p.Id == participant.LiveMatchParticipant.Id ? participant.LiveMatchParticipant : p).ToList();
                        cacheJobList.Add(_cachingService.SetObject($"{_liveMatchKey}{foundLiveMatch.Id}", foundLiveMatch.ToJsonType(), GetTimeToCache(foundLiveMatch)));
                    }
                }
            }

            var finalJobList = new List<Task>
            {
                userToUpdateList.Count > 0 ? _activeLiveMatchParticipantRepository.Update(userToUpdateList): Task.CompletedTask,
                userToCreateList.Count > 0 ? _activeLiveMatchParticipantRepository.Create(userToCreateList): Task.CompletedTask,
                cacheJobList.Count > 0 ? Task.WhenAll(cacheJobList): Task.CompletedTask
            };

            await Task.WhenAll(finalJobList);

        }
        public async Task DeleteParticipant(Guid liveMatchId, LiveMatchParticipant user)
        {
            var foundLiveMatch = await TryGetLiveMatch(liveMatchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (foundLiveMatch.Participants.FirstOrDefault(p => p.Id == user.Id) is User foundUser)
            {
                await _activeLiveMatchParticipantRepository.Delete(user, liveMatchId);
                if (foundLiveMatch.MatchStatus == LiveMatchStatus.InProgress || foundLiveMatch.MatchStatus == LiveMatchStatus.Finished)
                {
                    foundLiveMatch.Participants = foundLiveMatch.Participants.Where(p => p.Id != user.Id).ToList();
                    await _cachingService.SetObject($"{_liveMatchKey}{foundLiveMatch.Id}", foundLiveMatch.ToJsonType(), GetTimeToCache(foundLiveMatch));
                }
            }
        }
        public async Task DeleteParticipant(Guid liveMatchId, ICollection<LiveMatchParticipant> user)
        {
            var foundLiveMatch = await TryGetLiveMatch(liveMatchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            var idList = user.Select(u => (Guid)u.Id!).ToList();
            if (foundLiveMatch.Participants.FirstOrDefault(p => user.FirstOrDefault(x => p.Id == x.Id) is not null) is not null)
            {
                await _activeLiveMatchParticipantRepository.Delete(idList, liveMatchId);
                if (foundLiveMatch.MatchStatus == LiveMatchStatus.InProgress || foundLiveMatch.MatchStatus == LiveMatchStatus.Finished)
                {
                    var participants = await _activeLiveMatchParticipantRepository.GetForMatch(liveMatchId);
                    foundLiveMatch.Participants = participants?.ToList() ?? [];
                    await _cachingService.SetObject($"{_liveMatchKey}{foundLiveMatch.Id}", foundLiveMatch.ToJsonType(), GetTimeToCache(foundLiveMatch));
                }
            }
        }
        public async Task SaveCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch)
        {
            var foundLiveMatch = await TryGetLiveMatch(liveMatchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (foundLiveMatch.Catches.FirstOrDefault(c => c.Id == liveMatchCatch.Id) is LiveMatchCatch foundCatch)
            {
                if (liveMatchCatch.Equals(foundCatch))
                {
                    return;
                }
                await _activeLiveMatchCatchRepository.Update([liveMatchCatch]);
                if (foundLiveMatch.MatchStatus == LiveMatchStatus.InProgress || foundLiveMatch.MatchStatus == LiveMatchStatus.Finished)
                {
                    foundLiveMatch.Catches = foundLiveMatch.Catches.Select(c => c.Id == liveMatchCatch.Id ? liveMatchCatch : c).ToList();
                    await _cachingService.SetObject($"{_liveMatchKey}{foundLiveMatch.Id}", foundLiveMatch.ToJsonType(), GetTimeToCache(foundLiveMatch));
                }
            }
            else
            {
                await _activeLiveMatchCatchRepository.Create([liveMatchCatch]);
                if (foundLiveMatch.MatchStatus == LiveMatchStatus.InProgress || foundLiveMatch.MatchStatus == LiveMatchStatus.Finished)
                {
                    foundLiveMatch.Catches = foundLiveMatch.Catches.Append(liveMatchCatch).ToList();
                    await _cachingService.SetObject($"{_liveMatchKey}{foundLiveMatch.Id}", foundLiveMatch.ToJsonType(), GetTimeToCache(foundLiveMatch));
                }
            }
        }
        public async Task DeleteCatch(Guid liveMatchId, LiveMatchCatch liveMatchCatch)
        {
            var foundLiveMatch = await TryGetLiveMatch(liveMatchId) ?? throw new LiveMatchException(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails, HttpStatusCode.BadRequest);
            if (foundLiveMatch.Catches.FirstOrDefault(c => c.Id == liveMatchCatch.Id) is LiveMatchCatch foundCatch)
            {
                await _activeLiveMatchCatchRepository.Delete([liveMatchCatch]);
                if (foundLiveMatch.MatchStatus == LiveMatchStatus.InProgress || foundLiveMatch.MatchStatus == LiveMatchStatus.Finished)
                {
                    foundLiveMatch.Catches = foundLiveMatch.Catches.Where(c => c.Id != liveMatchCatch.Id).ToList();
                    await _cachingService.SetObject($"{_liveMatchKey}{foundLiveMatch.Id}", foundLiveMatch.ToJsonType(), GetTimeToCache(foundLiveMatch));
                }
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
                var liveMatchCreateJob = await _activeLiveMatchRepository.Create([liveMatch]);
                if (liveMatchCreateJob is not null)
                {
                    if (liveMatch.MatchStatus == LiveMatchStatus.InProgress || liveMatch.MatchStatus == LiveMatchStatus.Finished)
                    {
                        await _cachingService.SetObject($"{_liveMatchKey}{liveMatch.Id}", liveMatch.ToJsonType(), GetTimeToCache(liveMatch));
                    }
                }
                else
                {
                    throw new LiveMatchException(LiveMatchConstants.FailedToPersistLiveMatch, HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                var liveMatchCreate = await _activeLiveMatchRepository.Update([liveMatch]);
                if (liveMatchCreate is not null)
                {
                    if (liveMatch.MatchStatus == LiveMatchStatus.InProgress || liveMatch.MatchStatus == LiveMatchStatus.Finished)
                    {
                        await _cachingService.SetObject($"{_liveMatchKey}{liveMatch.Id}", liveMatch.ToJsonType(), GetTimeToCache(liveMatch));
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
                    return liveMatch;
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
        private static DistributedCacheEntryOptions GetTimeToCache(LiveMatch liveMatch)
        {
            if (liveMatch.TimeUntilEnd is null)
            {
                return new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) };
            }
            return new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = liveMatch.TimeUntilEnd };
        }
    }
}