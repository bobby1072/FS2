namespace fsCore.Common.Authentication;

public class ClientConfigSettings
{
    public const string Key = "ClientConfig";
    public string Scope { get; init; }
    public string ApiHost { get; init; }
    public string UserInfoEndpoint { get; init; }
    public string AuthorityHost { get; init; }
    public string AuthorityClientId { get; init; }
}