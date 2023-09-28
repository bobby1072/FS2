namespace Common.Authentication
{
    public interface IBearerTokenAuthenticationStrategy : IAuthenticationStrategy
    {
        public long? TokenExpiryInSecond { get; }

    }
}
