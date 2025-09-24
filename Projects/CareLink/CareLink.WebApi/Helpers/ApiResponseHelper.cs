namespace CareLink.WebApi.Helpers;

public static class ApiResponseHelper
{
    // Return plain JSON bodies (objects) so callers can wrap them with Results.Ok/BadRequest/etc.

    // Success body
    public static object Success<T>(T data, string message = "Operation completed successfully")
        => new { success = true, data, message };

    // Error bodies
    public static object Error(IEnumerable<string> errors, string message = "Operation failed")
        => new { success = false, errors = errors.ToArray(), message };

    public static object Error(string error, string message = "Operation failed")
        => new { success = false, errors = new[] { error }, message };
}
