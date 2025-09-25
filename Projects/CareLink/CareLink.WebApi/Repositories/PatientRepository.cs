using CareLink.WebApi.Data;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareLink.WebApi.Repositories;

/// <summary>
/// Repository for CRUD operations against the <see cref="Patient"/> aggregate.
/// Encapsulates EF Core access for patients and related navigation loading.
/// </summary>
public class PatientRepository(CareLinkDbContext db) : IPatientRepository
{
    /// <summary>
    /// Adds a new patient and saves changes.
    /// </summary>
    /// <param name="patient">The patient entity to persist.</param>
    /// <returns>The persisted patient with generated values.</returns>
    public async Task<Patient> AddAsync(Patient patient)
    {
        db.Patients.Add(patient);
        await db.SaveChangesAsync();
        return patient;
    }

    /// <summary>
    /// Retrieves all patients, including their linked doctors via <see cref="DoctorPatient"/>.
    /// </summary>
    /// <returns>A list of patients with related doctors eager-loaded.</returns>
    public Task<List<Patient>> GetAllAsync()
        => db.Patients
            .Include(p => p.DoctorPatients)
                .ThenInclude(dp => dp.Doctor)
            .ToListAsync();

    /// <summary>
    /// Retrieves a patient by identifier.
    /// </summary>
    /// <param name="id">The patient identifier.</param>
    /// <returns>The patient if found; otherwise null.</returns>
    public Task<Patient?> GetByIdAsync(int id)
        => db.Patients.FirstOrDefaultAsync(d => d.Id == id);

    /// <summary>
    /// Deletes a patient by identifier, if present.
    /// </summary>
    /// <param name="id">The patient identifier.</param>
    /// <returns>True if a record was deleted; otherwise false.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var patientEntity = await db.Patients.FindAsync([id]);
        if (patientEntity is null) return false;
        db.Patients.Remove(patientEntity);
        await db.SaveChangesAsync();
        return true;
    }
}
