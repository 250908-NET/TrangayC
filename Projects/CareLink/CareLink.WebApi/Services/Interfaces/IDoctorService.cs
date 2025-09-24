using CareLink.WebApi.Models;

namespace CareLink.WebApi.Services.Interfaces;

public interface IDoctorService
{
    Task<Doctor> CreateAsync(Doctor doctor);
    Task<List<Doctor>> GetAllAsync();
    Task<Doctor?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}
