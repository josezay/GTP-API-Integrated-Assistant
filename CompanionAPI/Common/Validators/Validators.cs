using System.Text.RegularExpressions;

namespace CompanionAPI.Common.Validators;

public static class Validators
{
    public static bool BeAValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        return emailRegex.IsMatch(email);
    }

    public static bool BeAValidGender(string gender)
    {
        if (string.IsNullOrWhiteSpace(gender))
            return false;

        return gender == "F" || gender == "M";
    }
}
