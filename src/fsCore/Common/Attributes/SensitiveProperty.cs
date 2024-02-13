namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveProperty : Attribute
    {
        public SensitiveProperty()
        {
        }
    }
}
