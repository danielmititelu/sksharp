namespace SkSharp;

public class LoggedInSkypeApi
{
    private SkypeService _skypeService;
    private LoginTokens _loginTokens;

    internal LoggedInSkypeApi(LoginTokens loginTokens)
    {
        _skypeService = new SkypeService();
        _loginTokens = loginTokens;
    }

    public string GetUserId()
    {
        return _loginTokens.UserId;
    }
}