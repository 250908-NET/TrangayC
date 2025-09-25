using CareLink.WebApi.Dtos;

namespace CareLink.WebApi.Services.Interfaces;

public interface IDoctorPatientService
{
    Task<(bool found, bool alreadyLinked, DoctorDto? doctor, PatientDto? patient)> LinkPatientAsync(int doctorId, int patientId);
    Task<(bool found, DoctorDto? doctor, PatientDto? patient)> UnlinkPatientAsync(int doctorId, int patientId);
}
