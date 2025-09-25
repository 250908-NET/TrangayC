using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services.Interfaces;

namespace CareLink.WebApi.Services;

/// <summary>
/// Provides application-level operations for patients.
/// Uses repositories for data access and projects results for API consumption.
/// </summary>
public class PatientService(IPatientRepository repo) : IPatientService
{
    /// <summary>
    /// Creates a new patient entity.
    /// </summary>
    /// <param name="patient">The patient to create.</param>
    /// <returns>The created patient.</returns>
    public Task<Patient> CreateAsync(Patient patient)
        => repo.AddAsync(patient);

    /// <summary>
    /// Retrieves all patients with a lightweight projection of related doctors for display.
    /// </summary>
    /// <returns>A list of anonymous objects representing patients and their doctors.</returns>
    public async Task<List<object>> GetAllAsync()
    {
        var patientEntities = await repo.GetAllAsync();
        var projectedList = patientEntities
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

        return projectedList;
    }

    /// <summary>
    /// Retrieves a patient by identifier, projecting related doctors for response shaping.
    /// </summary>
    /// <param name="id">The patient identifier.</param>
    /// <returns>An anonymous object with patient details and doctors if found; otherwise null.</returns>
    public async Task<object?> GetByIdAsync(int id)
    {
        var patientEntity = await repo.GetByIdAsync(id);
        if (patientEntity is null) return null;

        var projected = new
        {
            patientEntity.Id,
            patientEntity.FirstName,
            patientEntity.LastName,
            doctors = patientEntity.DoctorPatients
                .Where(dp => dp.Doctor != null)
                .Select(dp => new
                {
                    dp.Doctor!.Id,
                    dp.Doctor!.FirstName,
                    dp.Doctor!.LastName,
                    dp.Doctor!.Specialty
                }).ToList()
        } as object;

        return projected;
    }

    /// <summary>
    /// Deletes a patient by identifier.
    /// </summary>
    /// <param name="id">The patient identifier.</param>
    /// <returns>True if a record was deleted; otherwise false.</returns>
    public Task<bool> DeleteAsync(int id) => repo.DeleteAsync(id);
}

