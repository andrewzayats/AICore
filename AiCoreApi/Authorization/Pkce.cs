using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AiCoreApi.Authorization;

public class Pkce
{
    public string CodeVerifier;
    public string CodeChallenge;

    public Pkce(uint size = 128)
    {
        CodeVerifier = GenerateCodeVerifier(size);
        CodeChallenge = GenerateCodeChallenge(CodeVerifier);
    }

    public static string GenerateCodeVerifier(uint size = 128)
    {
        if (size < 43 || size > 128)
            size = 128;
        const string unreservedCharacters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
        var random = new Random();
        var highEntropyCryptograph = new char[size];
        for (var i = 0; i < highEntropyCryptograph.Length; i++)
        {
            highEntropyCryptograph[i] = unreservedCharacters[random.Next(unreservedCharacters.Length)];
        }

        return new string(highEntropyCryptograph);
    }

    public static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(codeVerifier));
        return Base64UrlEncoder.Encode(challengeBytes);
    }
}