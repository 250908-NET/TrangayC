using System.ComponentModel.DataAnnotations;

public static class ModelValidationHelper
{
    public static (bool IsValid, List<ValidationResult> Results) Validate(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
        bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
        return (isValid, validationResults);
    }

    public static List<string> GetErrorMessages(IEnumerable<ValidationResult> validationResults)
    {
        return validationResults.Select(r => r.ErrorMessage ?? "Validation error").ToList();
    }
}
