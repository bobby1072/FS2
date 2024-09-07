
namespace fsCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiredSignalRUserConnectionId : Attribute
    {
        public RequiredSignalRUserConnectionId()
        {
        }
    }
}