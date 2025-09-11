using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace firstAPI.Endpoints;

public static class TextEndpoints
{
    public static IEndpointRouteBuilder MapTextEndpoints(this IEndpointRouteBuilder app)
    {
        var textPath = app.MapGroup("/text");

        textPath.MapGet("reverse/{text}", (string text) =>
        {
            char[] charArray = text.ToCharArray();
            Array.Reverse(charArray);
            string result = new string(charArray);
            return Results.Ok(new { result });
        });

        textPath.MapGet("uppercase/{text}", (string text) =>
        {
            string result = text.ToUpper();
            return Results.Ok(new { result });
        });

        textPath.MapGet("lowercase/{text}", (string text) =>
        {
            string result = text.ToLower();
            return Results.Ok(new { result });
        });

        textPath.MapGet("count/{text}", (string text) =>
        {
            int textLength = text.Length;
            int whiteSpace = text.Count(element => char.IsWhiteSpace(element));
            int characterCount = textLength - whiteSpace;
            string[] words = text.Split();
            int wordCount = words.Length;
            string vowels = "aeiouAEIOU";
            var vowelCount = text.Count(element => vowels.Contains(element));
            var result = new { characterCount, wordCount, vowelCount };
            return Results.Ok(result);
        });

        textPath.MapGet("palindrome/{text}", (string text) =>
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Results.Ok(new { isPalindrome = true });
            }
            int l = 0, r = text.Length - 1;
            while (l < r)
            {
                while (l < r && !char.IsLetter(text[l])) l++;
                while (r > l && !char.IsLetter(text[r])) r--;
                if (char.ToLower(text[l]) != char.ToLower(text[r]))
                {
                    return Results.Ok(new { isPalindrome = false });
                }
                l++; r--;
            }
            return Results.Ok(new { isPalindrome = true });
        });

        return app;
    }
}
