using CareLink.WebApi.Data;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareLink.WebApi.Repositories;

/// <summary>
/// Repository for managing the doctor-patient link (junction) records.
/// Encapsulates all EF Core operations for the linking table.
/// </summary>
public class DoctorPatientRepository(CareLinkDbContext db) : IDoctorPatientRepository
{
    /// <summary>
    /// Checks whether a link exists for the given doctor and patient identifiers.
    /// </summary>
    /// <param name="doctorId">The doctor identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <returns>True if the link exists; otherwise false.</returns>
    public Task<bool> ExistsAsync(int doctorId, int patientId)
        => db.Set<DoctorPatient>().AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);

    /// <summary>
    /// Creates a link between the given doctor and patient identifiers.
    /// </summary>
    /// <param name="doctorId">The doctor identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    public async Task AddAsync(int doctorId, int patientId)
    {
        var doctorPatientLink = new DoctorPatient { DoctorId = doctorId, PatientId = patientId };
        db.Set<DoctorPatient>().Add(doctorPatientLink);
        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Removes an existing link between the given doctor and patient identifiers.
    /// </summary>
    /// <param name="doctorId">The doctor identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <returns>True if a link was found and removed; otherwise false.</returns>
    public async Task<bool> RemoveAsync(int doctorId, int patientId)
    {
        var doctorPatientLink = await db.Set<DoctorPatient>()
            .FirstOrDefaultAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
        if (doctorPatientLink is null) return false;
        db.Remove(doctorPatientLink);
        await db.SaveChangesAsync();
        return true;
    }
}
