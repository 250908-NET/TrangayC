namespace CareLink.WebApi.Dtos;

public sealed record PatientDto(
    int Id,
    string FirstName,
    string LastName
);
