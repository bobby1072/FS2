using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class MockGroupMemberBuilder
    {
        public static GroupMember Build(Guid groupId, Guid userId, int positionId)
        {
            return new GroupMember(
                groupId,
                positionId,
                userId
            );
        }
    }
}