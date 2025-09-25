using System.ComponentModel.DataAnnotations;

namespace CareLink.WebApi.Helpers;

/// <summary>
/// Provides a minimal, reusable DataAnnotations-based validator for simple models.
/// Returns a tuple indicating whether the model is valid and a list of validation error messages.
/// </summary>
public static class BasicModelValidator
{
    /// <summary>
    /// Validates the provided model using DataAnnotations attributes.
    /// </summary>
    /// <typeparam name="T">Type of the model to validate.</typeparam>
    /// <param name="model">The instance to validate.</param>
    /// <returns>
    /// A tuple where <c>isValid</c> indicates overall validation result and <c>errors</c>
    /// contains all collected validation error messages (empty if valid).
    /// </returns>
    public static (bool isValid, List<string> errors) Validate<T>(T model)
    {
        // Collect detailed validation results while validating the object.
        var validationResults = new List<ValidationResult>();

        // Build a validation context describing the object under validation.
        var validationContext = new ValidationContext(model!);

        // Validate the object against DataAnnotations on its properties (and object-level attributes).
        var isValid = Validator.TryValidateObject(
            model!,
            validationContext,
            validationResults,
            validateAllProperties: true);

        // Extract readable error messages for API responses or logs.
        var errors = validationResults
            .Select(result => result.ErrorMessage ?? "Invalid value.")
            .ToList();

        return (isValid, errors);
    }
}
