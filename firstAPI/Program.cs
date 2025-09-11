using System.Security.Cryptography;
using System.Text.RegularExpressions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//#Challenge 1: Basic Calculator
var calculatorPath = app.MapGroup("/calculator");
calculatorPath.MapGet("/add/{a}/{b}", (int a, int b) =>
{
    var result = new { operation = "add", input1 = a, input2 = b, result = a + b };
    return Results.Ok(result); 
});
 
calculatorPath.MapGet("/subtract/{a}/{b}", (int a, int b) => 
{
    var result = new { operation = "subtract", input1 = a, input2 = b, result = a - b };
    return Results.Ok(result); 
});
 
calculatorPath.MapGet("/multiply/{a}/{b}", (int a, int b) => 
{
    var result = new { operation = "multiply", input1 = a, input2 = b, result = a * b };
    return Results.Ok(result); 
});
 
calculatorPath.MapGet("/divide/{a}/{b}", (double a, double b) =>
{
    if (b == 0)
    {
        var error = new { error = "Cannot divide by zero" };
        return Results.BadRequest(error);
    }
    var result = new { operation = "divide", input1 = a, input2 = b, result = a / b };
    return Results.Ok(result); 
});

//#Challenge 2: String Manipulator
var textPath = app.MapGroup("/text");
textPath.MapGet("reverse/{text}", (string text) =>
{
    char[] charArray = text.ToCharArray();
    Array.Reverse(charArray);
    string result = new string(charArray);
    return Results.Ok(new { result = result });
});
 
textPath.MapGet("uppercase/{text}", (string text) =>
{
    string result = text.ToUpper();
    return Results.Ok(new { result = result });
});
 
textPath.MapGet("lowercase/{text}", (string text) =>
{
    string result = text.ToLower();
    return Results.Ok(new { result = result });
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

    var result = new { characterCount = characterCount, wordCount = wordCount, vowelCount = vowelCount };
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
        while (l < r && !char.IsLetter(text[l]))
        {
            l++;
        }
        while (r > l && !char.IsLetter(text[r]))
        {
            r--;
        }
        if (char.ToLower(text[l]) != char.ToLower(text[r]))
        {
            return Results.Ok(new { isPalindrome = false });
        }
        l++;
        r--;
    }

    return Results.Ok(new { isPalindrome = true });
});

//#Challenge 3: Number Games
var numberPath = app.MapGroup("/numbers");
numberPath.MapGet("fizzbuzz/{count}", (int count) =>
{
    var result = new List<string>();
    for (int i = 1; i <= count; i++)
    {
        bool divisibleBy3 = (i % 3 == 0);
        bool divisibleBy5 = (i % 5 == 0);

        if (divisibleBy3 && divisibleBy5)
        {
            result.Add("FizzBuzz");
        }
        else if (divisibleBy3)
        {
            result.Add("Fizz");
        }
        else if (divisibleBy5)
        {
            result.Add("Buzz");
        }
        else
        {
            result.Add(i.ToString());
        }
    }
    return Results.Ok(result);
});
numberPath.MapGet("prime/{number}", (int number) =>
{   
    if (number <= 1)
    {
        return Results.Ok(new { isPrime = false });
    }
    for (int i = 2; i <= Math.Sqrt(number); i++)
    {
        if (number % i == 0)
        {
            return Results.Ok(new { isPrime = false });
        }
    }
    return Results.Ok(new { isPrime = true });
});

numberPath.MapGet("fibonacci/{count}", (int count) =>
{
    var result = new List<int>();
    int a = 0, b = 1;
    for (int i = 0; i < count; i++)
    {
        result.Add(a);
        int temp = a;
        a = b;
        b = temp + b;
    }
    return Results.Ok(result);
});

numberPath.MapGet("factors/{number}", (int number) =>
{
    var result = new List<int>();
    for (int i = 1; i <= number; i++)
    {
        if (number % i == 0)
        {
            result.Add(i);
        }
    }
    return Results.Ok(result);
});

//#Challenge 4: Date and Time Fun 
var datePath = app.MapGroup("/date");
datePath.MapGet("today", () =>
{
    var today = DateTime.Now;
    return Results.Ok(new { today = today.ToString("yyyy-MM-dd") });
});

datePath.MapGet("age/{birthYear}", (int birthYear) =>
{
    var today = DateTime.Now;
    var age = today.Year - birthYear;
    return Results.Ok(new { age = age });
});

datePath.MapGet("daysbetween/{date1}/{date2}", (DateTime date1, DateTime date2) =>
{
    var daysBetween = date2 - date1;
    return Results.Ok(new { daysBetween = daysBetween.Days });
});

datePath.MapGet("weekday/{date}", (DateTime date) =>
{
    var weekday = date.DayOfWeek;
    return Results.Ok(new { weekday = weekday });
});

//#Challenge 5: Simple Collections
var colorsStore = new List<string> { "red", "blue", "green", "yellow", "purple" };
var colorsPath = app.MapGroup("/colors");
colorsPath.MapGet("", () =>
{
    return Results.Ok(colorsStore);
});

colorsPath.MapGet("random", () =>
{
    var random = new Random();
    var color = colorsStore[random.Next(colorsStore.Count)];
    return Results.Ok(color);
});

colorsPath.MapGet("search/{letter}", (string letter) =>
{
    if (string.IsNullOrEmpty(letter)) return Results.Ok(new List<string>());
    var result = colorsStore
        .Where(c => c.StartsWith(letter, StringComparison.OrdinalIgnoreCase))
        .ToList();
    return Results.Ok(result);
});

colorsPath.MapPost("add/{color}", (string color) =>
{
    if (string.IsNullOrWhiteSpace(color)) return Results.BadRequest(new { error = "color must not be empty" });
    
    if (!colorsStore.Any(c => string.Equals(c, color, StringComparison.OrdinalIgnoreCase)))
    {
        colorsStore.Add(color);
    }
    return Results.Ok(colorsStore);
});

//#Challenge 6: Temperature Converter
var tempPath = app.MapGroup("/temp");
tempPath.MapGet("celsius-to-fahrenheit/{temp}", (double temp) =>
{
    var result = new { celsius = temp, fahrenheit = (temp * 9 / 5) + 32 };
    return Results.Ok(result);
});

tempPath.MapGet("fahrenheit-to-celsius/{temp}", (double temp) =>
{
    var result = new { fahrenheit = temp, celsius = (temp - 32) * 5 / 9 };
    return Results.Ok(result);
});

tempPath.MapGet("kelvin-to-celsius/{temp}", (double temp) =>
{
    var result = new { kelvin = temp, celsius = temp - 273.15 };
    return Results.Ok(result);
});

tempPath.MapGet("compare/{temp1}/{unit1}/{temp2}/{unit2}", (double temp1, string unit1, double temp2, string unit2) =>
{
    var result = new { temp1 = temp1, unit1 = unit1, temp2 = temp2, unit2 = unit2, difference = Math.Abs(temp1 - temp2) };
    return Results.Ok(result);
});

//#Challenge 7: Password Generator
var passwordPath = app.MapGroup("/password");
passwordPath.MapGet("simple/{length}", (int length) =>
{
    bool passwordLengthIsZero = (length <= 0);
    if (passwordLengthIsZero) return Results.BadRequest(new { error = "length must be a positive integer" });

    const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string digits = "0123456789";
    string lettersAndDigits = letters + digits;

    var buffer = new char[length];
    for (int i = 0; i < length; i++)
    {
        int randomNumberIndex = RandomNumberGenerator.GetInt32(lettersAndDigits.Length);
        buffer[i] = lettersAndDigits[randomNumberIndex];
    }

    var password = new string(buffer);
    return Results.Ok(new { password });
});

passwordPath.MapGet("complex/{length}", (int length) =>
{
    bool passwordLengthIsZero = (length <= 0);
    if (passwordLengthIsZero) return Results.BadRequest(new { error = "length must be a positive integer" });

    const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string digits = "0123456789";
    const string specials = "!@#$%^&*()-_=+[]{};:,.<>?/";
    string lettersAndDigitsAndSpecials = letters + digits + specials;

    var buffer = new char[length];
    for (int i = 0; i < length; i++)
    {
        int randomNumberIndex = RandomNumberGenerator.GetInt32(lettersAndDigitsAndSpecials.Length);
        buffer[i] = lettersAndDigitsAndSpecials[randomNumberIndex];
    }

    var password = new string(buffer);
    return Results.Ok(new { password });
});

passwordPath.MapGet("memorable/{words}", (int words) =>
{
    bool wordsIsZero = (words <= 0);
    if (wordsIsZero) return Results.BadRequest(new { error = "please enter a at least one word" });

    string[] wordList = new[]
    {
        "blue","river","sun","cloud","tree","mountain","ocean","stone","bright","quiet",
        "happy","swift","forest","silver","night","gentle","rapid","gold","wind","star"
    };
    var parts = new List<string>(words);
    for (int i = 0; i < words; i++)
    {
        int randomNumberIndex = RandomNumberGenerator.GetInt32(wordList.Length);
        parts.Add(wordList[randomNumberIndex]);
    }
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
    int lengthScore = password.Length >= 15 ? 2 : password.Length >= 10 ? 1 : 0;

    int varietyScore = 0;
    if (hasLower) varietyScore++;
    if (hasUpper) varietyScore++;
    if (hasDigit) varietyScore++;
    if (hasSpecial) varietyScore++;

    int score = lengthScore + varietyScore;
    string strength = score >= 5 ? "Strong" : score >= 3 ? "Medium" : "Weak";
    return Results.Ok(new { password, strength });
});

//Challenge 8: Simple Validator
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

    // Trims white spaces, dashes, and parentheses
    var trimmed = phone.Trim();
    var cleaned = Regex.Replace(trimmed, @"[\s\-()]+", "");

    // Check for 10 digits
    bool valid = Regex.IsMatch(cleaned, @"^[0-9]{10}$");
    return Results.Ok(new { phone, valid });
});

validatePath.MapGet("creditcard/{number}", (string number) =>
{
    if (string.IsNullOrWhiteSpace(number)) return Results.BadRequest(new { error = "please enter a card number" });

    // Trims white spaces and dashes
    var digitsOnly = Regex.Replace(number, @"[\s\-]", "");
    if (!digitsOnly.All(char.IsDigit) || digitsOnly.Length < 15 || digitsOnly.Length > 19) return Results.Ok(new { number, valid = false });
    
    // Luhn algorithm is used to check if credit card number structure is correct
    bool LuhnCheck(string s)
    {
        int sum = 0;
        bool doubleIt = false;
        for (int i = s.Length - 1; i >= 0; i--)
        {
            int d = s[i] - '0';
            if (doubleIt)
            {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            doubleIt = !doubleIt;
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
    return Results.Ok(new
    {
        password,
        isStrong,
        rules = new { minLength, hasLower, hasUpper, hasDigit, hasSpecial }
    });
});

app.Run();