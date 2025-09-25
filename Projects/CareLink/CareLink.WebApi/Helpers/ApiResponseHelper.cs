namespace CareLink.WebApi.Helpers;

/// <summary>
/// Provides small helpers to build consistent API response bodies.
/// These helpers return plain objects that callers wrap with Results.Ok/BadRequest/etc.
/// </summary>
public static class ApiResponseHelper
{
    /// <summary>
    /// Creates a standardized success response body.
    /// </summary>
    /// <typeparam name="T">Type of the payload being returned.</typeparam>
    /// <param name="data">The payload to include in the response body.</param>
    /// <param name="message">A human-friendly message describing the successful operation.</param>
    /// <returns>An anonymous object with <c>success</c>, <c>data</c>, and <c>message</c> properties.</returns>
    public static object Success<T>(T data, string message = "Operation completed successfully")
        => new { success = true, data, message };

    /// <summary>
    /// Creates a standardized error response body from a collection of error messages.
    /// </summary>
    /// <param name="errors">A list of error messages to include.</param>
    /// <param name="message">A human-friendly message describing the failure.</param>
    /// <returns>An anonymous object with <c>success</c>, <c>errors</c>, and <c>message</c> properties.</returns>
    public static object Error(IEnumerable<string> errors, string message = "Operation failed")
        => new { success = false, errors = errors.ToArray(), message };

    /// <summary>
    /// Creates a standardized error response body from a single error message.
    /// </summary>
    /// <param name="error">The error message to include.</param>
    /// <param name="message">A human-friendly message describing the failure.</param>
    /// <returns>An anonymous object with <c>success</c>, <c>errors</c>, and <c>message</c> properties.</returns>
    public static object Error(string error, string message = "Operation failed")
        => new { success = false, errors = new[] { error }, message };
}
