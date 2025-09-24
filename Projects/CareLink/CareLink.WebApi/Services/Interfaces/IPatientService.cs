using CareLink.WebApi.Models;

namespace CareLink.WebApi.Services.Interfaces;

public interface IPatientService
{
    Task<Patient> CreateAsync(Patient patient);
    Task<List<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}
