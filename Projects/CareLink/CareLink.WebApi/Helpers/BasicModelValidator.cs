using System.ComponentModel.DataAnnotations;

namespace CareLink.WebApi.Helpers;

public static class BasicModelValidator
{
    public static (bool isValid, List<string> errors) Validate<T>(T model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model!);
        var isValid = Validator.TryValidateObject(model!, ctx, validationResults, validateAllProperties: true);

        var errors = validationResults
            .Select(vr => vr.ErrorMessage ?? "Invalid value.")
            .ToList();

        return (isValid, errors);
    }
}
