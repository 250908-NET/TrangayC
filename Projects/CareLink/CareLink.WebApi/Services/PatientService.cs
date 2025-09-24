using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services.Interfaces;

namespace CareLink.WebApi.Services;

public class PatientService(IPatientRepository repo) : IPatientService
{
    public Task<Patient> CreateAsync(Patient patient)
        => repo.AddAsync(patient);

    public async Task<List<Patient>> GetAllAsync()
        => await repo.GetAllAsync();

    public Task<Patient?> GetByIdAsync(int id)
        => repo.GetByIdAsync(id);

    public Task<bool> DeleteAsync(int id) => repo.DeleteAsync(id);
}
