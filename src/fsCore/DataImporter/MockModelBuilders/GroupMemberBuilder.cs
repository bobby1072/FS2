using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class GroupMemberBuilder
    {
        public static GroupMember Build(Guid groupId, Guid userId, Guid positionId)
        {
            return new GroupMember(
                groupId,
                positionId,
                userId
            );
        }
    }
}