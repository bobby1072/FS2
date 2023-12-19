namespace Common.Authentication;

public class ClientConfigSettings
{
    public const string Key = "ClientConfig";
    public string Scope => "openid profile email";
    public string ApiHost { get; init; }
    public string UserInfoEndpoint { get; init; }
    public string ClientHost { get; init; }
    public string AuthorityHost { get; init; }
    public string AuthorityClientId { get; init; }
}