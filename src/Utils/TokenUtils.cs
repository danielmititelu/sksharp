using System.Text.RegularExpressions;

namespace SkSharp;

internal partial class TokenUtils
{
    [GeneratedRegex("expires=([0-9]+)")]
    private static partial Regex ExpiresInRegex();

    [GeneratedRegex(@"(registrationToken=[a-z0-9\+/=]+)", RegexOptions.IgnoreCase)]
    private static partial Regex RegistrationTokenRegex();

    [GeneratedRegex(@"endpointId=({[a-z0-9\-]+})", RegexOptions.IgnoreCase)]
    private static partial Regex EndpointRegex();

    internal static (string registrationToken, int expires, string endpoint) ParseRegistrationTokenHeader(string registrationTokenHeader)
    {
        var registrationToken = RegistrationTokenRegex().Match(registrationTokenHeader).Groups[1].Value;
        var expires = ExpiresInRegex().Match(registrationTokenHeader).Groups[1].Value;
        var endpoint = EndpointRegex().Match(registrationTokenHeader).Groups[1].Value;
        if (int.TryParse(expires, out var expiresInt))
        {
            return (registrationToken, expiresInt, endpoint);
        }
        return (registrationToken, 0, endpoint);
    }
}