namespace Common.Authentication;
public class ClientConfigurationResponse
{
    public ClientConfigurationResponse(string apiHost, string authorityHost, string authorityScope, string authorityClientId)
    {
        ApiHost = apiHost;
        AuthorityHost = authorityHost;
        AuthorityScope = authorityScope;
        AuthorityClientId = authorityClientId;
    }

    public string ApiHost { get; }


    public string AuthorityHost { get; }

    public string AuthorityScope { get; }

    public string AuthorityClientId { get; }
}