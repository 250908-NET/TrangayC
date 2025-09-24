using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services.Interfaces;
using System.Linq;

namespace CareLink.WebApi.Services;

public class DoctorService(IDoctorRepository repo) : IDoctorService
{
    public Task<Doctor> CreateAsync(Doctor doctor)
        => repo.AddAsync(doctor);

    public async Task<List<object>> GetAllAsync()
    {
        var doctors = await repo.GetAllAsync();
        var shaped = doctors
            .Select(d => new
            {
                d.Id,
                d.FirstName,
                d.LastName,
                d.Specialty,
                patients = d.DoctorPatients
                    .Where(dp => dp.Patient != null)
                    .Select(dp => new
                    {
                        dp.Patient!.Id,
                        dp.Patient!.FirstName,
                        dp.Patient!.LastName
                    }).ToList()
            } as object)
            .ToList();

        return shaped;
    }

    public Task<Doctor?> GetByIdAsync(int id)
        => repo.GetByIdAsync(id);

    public Task<bool> DeleteAsync(int id) => repo.DeleteAsync(id);
}
