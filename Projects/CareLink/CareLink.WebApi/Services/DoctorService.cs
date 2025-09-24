using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services.Interfaces;

namespace CareLink.WebApi.Services;

public class DoctorService(IDoctorRepository repo) : IDoctorService
{
    public Task<Doctor> CreateAsync(Doctor doctor)
        => repo.AddAsync(doctor);

    public async Task<List<Doctor>> GetAllAsync()
        => await repo.GetAllAsync();

    public Task<Doctor?> GetByIdAsync(int id)
        => repo.GetByIdAsync(id);

    public Task<bool> DeleteAsync(int id) => repo.DeleteAsync(id);
}
