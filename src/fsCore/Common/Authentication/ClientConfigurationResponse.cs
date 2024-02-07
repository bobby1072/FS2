namespace Common.Authentication;
public class ClientConfigurationResponse
{
    public ClientConfigurationResponse(string apiHost, string authorityHost, string authorityClientId, string scope)
    {
        ApiHost = apiHost;
        Scope = scope;
        AuthorityHost = authorityHost;
        AuthorityClientId = authorityClientId;
    }

    public string ApiHost { get; }

    public string Scope { get; }

    public string AuthorityHost { get; }


    public string AuthorityClientId { get; }
}