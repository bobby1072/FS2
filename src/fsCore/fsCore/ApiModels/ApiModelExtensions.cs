using System.Net;
using Common.Misc;
using Common.Models;
using Common.Permissions;
using Common.Utils;

namespace fsCore.ApiModels
{
    public static class ApiModelExtensions
    {
        public static GroupCatchComment ToGroupCatchComment(this GroupCatchCommentInput input)
        {
            return new GroupCatchComment(input.Id, input.GroupCatchId, input.UserId, input.Comment, input.CreatedAt?.ToUniversalTime() ?? DateTime.UtcNow);
        }

        public static RawUserPermission ToRawUserPermission(this UserWithGroupPermissionSet user)
        {
            var rawUser = new RawUserPermission(user);
            rawUser.GroupPermissions.Abilities = user.GroupPermissions.Abilities.Select(x => new Permission<Guid> { Action = x.Action, Fields = x.Fields, Subject = x.Subject.Id ?? throw new Exception() }).ToArray();
            return rawUser;
        }
        public static async Task<GroupCatch> ToGroupCatchAsync(this SaveGroupCatchFormInput saveCatchFormInput, Guid userId)
        {
            return new GroupCatch(
                userId,
                saveCatchFormInput.GroupId,
                saveCatchFormInput.Species,
                saveCatchFormInput.Weight,
                saveCatchFormInput.CaughtAt is not null ? DateTime.Parse(saveCatchFormInput.CaughtAt).ToUniversalTime() : DateTime.UtcNow,
                saveCatchFormInput.Length,
                saveCatchFormInput.Latitude,
                saveCatchFormInput.Longitude,
                saveCatchFormInput.Description,
                saveCatchFormInput.Id,
                saveCatchFormInput.CreatedAt is not null ? DateTime.Parse(saveCatchFormInput.CreatedAt).ToUniversalTime() : DateTime.UtcNow,
                saveCatchFormInput.CatchPhoto is not null ? await saveCatchFormInput.CatchPhoto.ToByteArrayAsync(1) : null,
                null,
                null,
                saveCatchFormInput.WorldFishTaxocode,
                null
            );
        }
        public static LiveMatchCatch ToLiveMatchCatch(this SaveMatchCatchLiveMatchHubInput saveCatchFormInput, Guid userId)
        {
            return new LiveMatchCatch(
                userId,
                saveCatchFormInput.MatchId,
                saveCatchFormInput.Species,
                saveCatchFormInput.Weight,
                saveCatchFormInput.CaughtAt.ToUniversalTime(),
                saveCatchFormInput.Length,
                saveCatchFormInput.Latitude,
                saveCatchFormInput.Longitude,
                saveCatchFormInput.Description,
                saveCatchFormInput.CountsInMatch,
                saveCatchFormInput.Id,
                saveCatchFormInput.CreatedAt is not null ? saveCatchFormInput.CreatedAt?.ToUniversalTime() : DateTime.UtcNow,
                saveCatchFormInput.WorldFishTaxocode
            );
        }
        public static async Task<Group> ToGroupAsync(this SaveGroupFormInput groupInput)
        {
            return new Group(
                groupInput.Name,
                groupInput.Emblem is not null ? await groupInput.Emblem.ToByteArrayAsync(720, 576, (decimal)0.5) : null,
                groupInput.Description,
                groupInput.Id is not null ? groupInput.Id : null,
                groupInput.CreatedAt is not null ? DateTime.Parse(groupInput.CreatedAt).ToUniversalTime() : DateTime.UtcNow,
                bool.Parse(groupInput.IsPublic),
                bool.Parse(groupInput.IsListed),
                bool.Parse(groupInput.CatchesPublic),
                groupInput.LeaderId
            );
        }
        public static LiveMatch ToLiveMatch(this SaveMatchLiveHubInput saveMatchLiveHubInput, Guid matchLeaderId)
        {
            if (!Enum.TryParse<LiveMatchStatus>(saveMatchLiveHubInput.MatchStatus.ToString(), out var matchStatus) || !Enum.TryParse<LiveMatchWinStrategy>(saveMatchLiveHubInput.MatchWinStrategy.ToString(), out var matchWinStrategy))
            {
                throw new ApiException(ErrorConstants.BadRequest, HttpStatusCode.BadRequest);
            }
            return new LiveMatch(saveMatchLiveHubInput.GroupId, saveMatchLiveHubInput.MatchName, saveMatchLiveHubInput.MatchRules.ToRuntimeType(), matchStatus, matchWinStrategy, new List<LiveMatchCatch>(), new List<LiveMatchParticipant>(), matchLeaderId, DateTime.UtcNow, saveMatchLiveHubInput.CommencesAt, saveMatchLiveHubInput.EndsAt, saveMatchLiveHubInput.Description, saveMatchLiveHubInput.Id);
        }
    }
}