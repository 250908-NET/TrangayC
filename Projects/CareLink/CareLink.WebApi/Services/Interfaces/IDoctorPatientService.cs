using CareLink.WebApi.Models;

namespace CareLink.WebApi.Services.Interfaces;

public interface IDoctorPatientService
{
    Task<(bool found, bool alreadyLinked, Doctor? doctor, Patient? patient)> LinkPatientAsync(int doctorId, int patientId);
    Task<(bool found, Doctor? doctor, Patient? patient)> UnlinkPatientAsync(int doctorId, int patientId);
}
