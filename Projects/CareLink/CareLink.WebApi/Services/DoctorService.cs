using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services.Interfaces;

namespace CareLink.WebApi.Services;

/// <summary>
/// Provides application-level operations for doctors.
/// Uses repositories for data access and projects results for API consumption.
/// </summary>
public class DoctorService(IDoctorRepository repo) : IDoctorService
{
    /// <summary>
    /// Creates a new doctor entity.
    /// </summary>
    /// <param name="doctor">The doctor to create.</param>
    /// <returns>The created doctor.</returns>
    public Task<Doctor> CreateAsync(Doctor doctor)
        => repo.AddAsync(doctor);

    /// <summary>
    /// Retrieves all doctors with a lightweight projection of related patients for display.
    /// </summary>
    /// <returns>A list of anonymous objects representing doctors and their patients.</returns>
    public async Task<List<object>> GetAllAsync()
    {
        var doctorEntities = await repo.GetAllAsync();
        var projectedList = doctorEntities
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

        return projectedList;
    }

    /// <summary>
    /// Retrieves a doctor by identifier.
    /// </summary>
    /// <param name="id">The doctor identifier.</param>
    /// <returns>The doctor if found; otherwise null.</returns>
    public Task<Doctor?> GetByIdAsync(int id)
        => repo.GetByIdAsync(id);

    /// <summary>
    /// Deletes a doctor by identifier.
    /// </summary>
    /// <param name="id">The doctor identifier.</param>
    /// <returns>True if a record was deleted; otherwise false.</returns>
    public Task<bool> DeleteAsync(int id) => repo.DeleteAsync(id);
}
