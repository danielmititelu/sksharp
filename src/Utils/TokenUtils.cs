using System.Text.RegularExpressions;

namespace SkSharp;

internal partial class TokenUtils
{
    [GeneratedRegex("expires=([0-9]+)")]
    private static partial Regex ExpiresInRegex();

    [GeneratedRegex("(registrationToken=[a-z0-9\\+/=]+)")]
    private static partial Regex RegistrationTokenRegex();

    internal static (string registrationToken, int expires) ParseRegistrationTokenHeader(string registrationTokenHeader)
    {
        var registrationToken = RegistrationTokenRegex().Match(registrationTokenHeader).Value;
        var expires = ExpiresInRegex().Match(registrationTokenHeader).Groups[1].Value;
        if (int.TryParse(expires, out var expiresInt))
        {
            return (registrationToken, expiresInt);
        }
        return (registrationToken, 0);
    }


}