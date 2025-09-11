using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace firstAPI.Endpoints;

public static class PasswordEndpoints
{
    public static IEndpointRouteBuilder MapPasswordEndpoints(this IEndpointRouteBuilder app)
    {
        var passwordPath = app.MapGroup("/password");

        passwordPath.MapGet("simple/{length}", (int length) =>
        {
            if (length <= 0) return Results.BadRequest(new { error = "length must be a positive integer" });
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            string charset = letters + digits;
            var buffer = new char[length];
            for (int i = 0; i < length; i++) buffer[i] = charset[RandomNumberGenerator.GetInt32(charset.Length)];
            var password = new string(buffer);
            return Results.Ok(new { password });
        });

        passwordPath.MapGet("complex/{length}", (int length) =>
        {
            if (length <= 0) return Results.BadRequest(new { error = "length must be a positive integer" });
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specials = "!@#$%^&*()-_=+[]{};:,.<>?/";
            string charset = letters + digits + specials;
            var buffer = new char[length];
            for (int i = 0; i < length; i++) buffer[i] = charset[RandomNumberGenerator.GetInt32(charset.Length)];
            var password = new string(buffer);
            return Results.Ok(new { password });
        });

        passwordPath.MapGet("memorable/{words}", (int words) =>
        {
            if (words <= 0) return Results.BadRequest(new { error = "please enter a at least one word" });
            string[] wordList = new[] { "blue","river","sun","cloud","tree","mountain","ocean","stone","bright","quiet","happy","swift","forest","silver","night","gentle","rapid","gold","wind","star" };
            var parts = new List<string>(words);
            for (int i = 0; i < words; i++) parts.Add(wordList[RandomNumberGenerator.GetInt32(wordList.Length)]);
            var password = string.Join('-', parts);
            return Results.Ok(new { password });
        });

        passwordPath.MapGet("strength/{password}", (string password) =>
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
