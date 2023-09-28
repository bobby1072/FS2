namespace Common.Authentication;

public interface IAuthenticationStrategy
{
    public AuthenticationStrategy Type { get; }
    public Tuple<string, string> AuthenticationHeader { get; }

}

