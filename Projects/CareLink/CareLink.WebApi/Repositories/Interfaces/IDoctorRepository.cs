using CareLink.WebApi.Models;

namespace CareLink.WebApi.Repositories.Interfaces;

public interface IDoctorRepository
{
    Task<Doctor> AddAsync(Doctor doctor);
    Task<Doctor?> GetByIdAsync(int id);
    Task<List<Doctor>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}
