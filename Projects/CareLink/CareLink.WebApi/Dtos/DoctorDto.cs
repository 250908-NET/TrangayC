namespace CareLink.WebApi.Dtos;

public sealed record DoctorDto(
    int Id,
    string FirstName,
    string LastName,
    string Specialty
);
