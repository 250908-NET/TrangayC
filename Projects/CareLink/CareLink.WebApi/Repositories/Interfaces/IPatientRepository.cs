using CareLink.WebApi.Models;

namespace CareLink.WebApi.Repositories.Interfaces;

public interface IPatientRepository
{
    Task<Patient> AddAsync(Patient patient);
    Task<Patient?> GetByIdAsync(int id);
    Task<List<Patient>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}
