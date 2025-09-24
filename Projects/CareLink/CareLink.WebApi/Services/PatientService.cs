using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services.Interfaces;
using System.Linq;

namespace CareLink.WebApi.Services;

public class PatientService(IPatientRepository repo) : IPatientService
{
    public Task<Patient> CreateAsync(Patient patient)
        => repo.AddAsync(patient);

    public async Task<List<object>> GetAllAsync()
    {
        var patients = await repo.GetAllAsync();
        var shaped = patients
            .Select(p => new
            {
                p.Id,
                p.FirstName,
                p.LastName,
                doctors = p.DoctorPatients
                    .Where(dp => dp.Doctor != null)
                    .Select(dp => new
                    {
                        dp.Doctor!.Id,
                        dp.Doctor!.FirstName,
                        dp.Doctor!.LastName,
                        dp.Doctor!.Specialty
                    }).ToList()
            } as object)
            .ToList();

        return shaped;
    }

    public Task<Patient?> GetByIdAsync(int id)
        => repo.GetByIdAsync(id);

    public Task<bool> DeleteAsync(int id) => repo.DeleteAsync(id);
}
