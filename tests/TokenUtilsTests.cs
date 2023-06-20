namespace SkSharp.Tests;

public class TokenUtilsTests
{
    [Test]
    public void RegistrationTokenHeader()
    {
        var expectedRegistrationToken = "registrationToken=ultrasecrettoken";
        var expectedExpires = 1687192941;
        var registrationtokenHeader = $"{expectedRegistrationToken}; expires={expectedExpires}";
        var (registrationToken, expires) = TokenUtils.ParseRegistrationTokenHeader(registrationtokenHeader);
        Assert.Multiple(() =>
        {
            Assert.That(registrationToken, Is.EqualTo(expectedRegistrationToken));
            Assert.That(expires, Is.EqualTo(expectedExpires));
        });
    }
}