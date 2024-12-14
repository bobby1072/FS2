using fsCore.Common.Models;

namespace fsCore.DataImporter.MockModelBuilders
{
    public static class MockGroupMemberBuilder
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