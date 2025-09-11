using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace firstAPI.Endpoints;

public static class ValidateEndpoints
{
    public static IEndpointRouteBuilder MapValidateEndpoints(this IEndpointRouteBuilder app)
    {
        var validatePath = app.MapGroup("/validate");

        validatePath.MapGet("email/{email}", (string email) =>
        {
            if (email is null) return Results.BadRequest(new { error = "please enter an email" });
            bool hasAt = email.Contains("@");
            bool hasDot = email.Contains(".");
            bool hasSpace = email.Contains(" ");
            bool hasSpecial = email.Any(ch => !char.IsLetterOrDigit(ch));
            if (hasAt && hasDot && !hasSpace && hasSpecial) return Results.Ok(new { email, valid = true });
            return Results.Ok(new { email, valid = false });
        });

        validatePath.MapGet("phone/{phone}", (string phone) =>
        {
            if (string.IsNullOrWhiteSpace(phone)) return Results.BadRequest(new { error = "please enter a phone number" });
            var trimmed = phone.Trim();
            var cleaned = Regex.Replace(trimmed, @"[\s\-()]+", "");
            bool valid = Regex.IsMatch(cleaned, @"^[0-9]{10}$");
            return Results.Ok(new { phone, valid });
        });

        validatePath.MapGet("creditcard/{number}", (string number) =>
        {
            if (string.IsNullOrWhiteSpace(number)) return Results.BadRequest(new { error = "please enter a card number" });
            var digitsOnly = Regex.Replace(number, @"[\s\-]", "");
            if (!digitsOnly.All(char.IsDigit) || digitsOnly.Length < 15 || digitsOnly.Length > 19) return Results.Ok(new { number, valid = false });
            bool LuhnCheck(string s)
            {
                int sum = 0; bool doubleIt = false;
                for (int i = s.Length - 1; i >= 0; i--)
                {
                    int d = s[i] - '0';
                    if (doubleIt) { d *= 2; if (d > 9) d -= 9; }
                    sum += d; doubleIt = !doubleIt;
                }
                return sum % 10 == 0;
            }
            bool valid = LuhnCheck(digitsOnly);
            return Results.Ok(new { number, valid });
        });

        validatePath.MapGet("strongpassword/{password}", (string password) =>
        {
            if (password is null) return Results.BadRequest(new { error = "please enter a password" });
            bool hasLower = password.Any(char.IsLower);
            bool hasUpper = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));
            bool minLength = password.Length >= 15;
            bool isStrong = minLength && hasLower && hasUpper && hasDigit && hasSpecial;
            return Results.Ok(new { password, isStrong, rules = new { minLength, hasLower, hasUpper, hasDigit, hasSpecial } });
        });

        return app;
    }
}
