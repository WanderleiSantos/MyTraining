using System.Security.Cryptography;
using System.Text;

namespace Application.Shared.Extensions;

public static class EncryptExtension
{
    public static string CreateSha256Hash(this string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash).ToLower();
    }

    public static string HashPassword(this string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }
    
    public static bool VerifyPassword(this string input, string verify)
    {
        return BCrypt.Net.BCrypt.Verify(input, verify);
    }
}