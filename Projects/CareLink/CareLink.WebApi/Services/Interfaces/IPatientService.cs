using CareLink.WebApi.Models;

namespace CareLink.WebApi.Services.Interfaces;

public interface IPatientService
{
    Task<Patient> CreateAsync(Patient patient);
    Task<List<object>> GetAllAsync();
    Task<object?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}
