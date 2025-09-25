using CareLink.WebApi.Models;

namespace CareLink.WebApi.Repositories.Interfaces;

public interface IDoctorPatientRepository
{
    Task<bool> ExistsAsync(int doctorId, int patientId);
    Task AddAsync(int doctorId, int patientId);
    Task<bool> RemoveAsync(int doctorId, int patientId);
}
