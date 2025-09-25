using CareLink.WebApi.Data;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareLink.WebApi.Repositories;

/// <summary>
/// Repository for CRUD operations against the <see cref="Doctor"/> aggregate.
/// Encapsulates EF Core access for doctors and related navigation loading.
/// </summary>
public class DoctorRepository(CareLinkDbContext db) : IDoctorRepository
{
    /// <summary>
    /// Adds a new doctor and saves changes.
    /// </summary>
    /// <param name="doctor">The doctor entity to persist.</param>
    /// <returns>The persisted doctor with generated values.</returns>
    public async Task<Doctor> AddAsync(Doctor doctor)
    {
        db.Doctors.Add(doctor);
        await db.SaveChangesAsync();
        return doctor;
    }

    /// <summary>
    /// Retrieves all doctors, including their linked patients via <see cref="DoctorPatient"/>.
    /// </summary>
    /// <returns>A list of doctors with related patients eager-loaded.</returns>
    public Task<List<Doctor>> GetAllAsync()
        => db.Doctors
            .Include(d => d.DoctorPatients)
                .ThenInclude(dp => dp.Patient)
            .ToListAsync();

    /// <summary>
    /// Retrieves a doctor by identifier.
    /// </summary>
    /// <param name="id">The doctor identifier.</param>
    /// <returns>The doctor if found; otherwise null.</returns>
    public Task<Doctor?> GetByIdAsync(int id)
        => db.Doctors.FirstOrDefaultAsync(d => d.Id == id);

    /// <summary>
    /// Deletes a doctor by identifier, if present.
    /// </summary>
    /// <param name="id">The doctor identifier.</param>
    /// <returns>True if a record was deleted; otherwise false.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var doctorEntity = await db.Doctors.FindAsync([id]);
        if (doctorEntity is null) return false;
        db.Doctors.Remove(doctorEntity);
        await db.SaveChangesAsync();
        return true;
    }
}
