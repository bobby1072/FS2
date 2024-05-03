using Common.Models;

namespace DataImporter.ModelBuilders
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