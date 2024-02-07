namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LockedProperty : Attribute
    {
        public LockedProperty()
        {
        }
    }
}
