using System.Security.Cryptography;
using System.Text;

namespace MyTraining.Application.Shared.Extensions;

public static class EncryptExtension
{
    public static string CreateSHA256Hash(this string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash).ToLower();
    }
}