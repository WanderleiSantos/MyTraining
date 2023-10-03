using System.Text.RegularExpressions;

namespace Application.Shared.Extensions;

public static class PasswordExtension
{
    public static bool IsPassWordValid(this string input)
    {
        var regex = new Regex(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$");

        return regex.IsMatch(input);
    }
}