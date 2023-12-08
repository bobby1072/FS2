namespace Common.Authentication;
public class ClientConfigurationResponse
{
    public ClientConfigurationResponse(string apiHost, string authorityHost, string authorityClientId)
    {
        ApiHost = apiHost;
        AuthorityHost = authorityHost;
        AuthorityClientId = authorityClientId;
    }

    public string ApiHost { get; }


    public string AuthorityHost { get; }


    public string AuthorityClientId { get; }
}