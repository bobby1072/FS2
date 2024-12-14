namespace fsCore.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiredSignalRUserConnectionId : Attribute
    {
        public const string ConnectionIdUserIdCacheKeyPrefix = "connectionIdUserId-";
        public RequiredSignalRUserConnectionId() { }
    }
}