using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services.Interfaces;
using CareLink.WebApi.Models;
using CareLink.WebApi.Dtos;

namespace CareLink.WebApi.Services;

/// <summary>
/// Provides application-level operations for the doctor-patient relationship (link/unlink).
/// Relies on repositories for data access and returns slim DTOs for API responses.
/// </summary>
public class DoctorPatientService(
    IDoctorRepository doctorRepo,
    IPatientRepository patientRepo,
    IDoctorPatientRepository linkRepo) : IDoctorPatientService
{
    /// <summary>
    /// Links an existing patient to an existing doctor.
    /// </summary>
    /// <param name="doctorId">The doctor identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <returns>
    /// A tuple indicating whether both entities were found (<c>found</c>), whether the link already existed
    /// (<c>alreadyLinked</c>), and the projected doctor and patient DTOs.
    /// </returns>
    public async Task<(bool found, bool alreadyLinked, DoctorDto? doctor, PatientDto? patient)> LinkPatientAsync(int doctorId, int patientId)
    {
        var doctorEntity = await doctorRepo.GetByIdAsync(doctorId);
        var patientEntity = await patientRepo.GetByIdAsync(patientId);
        if (doctorEntity is null || patientEntity is null)
            return (false, false, MapDoctor(doctorEntity), MapPatient(patientEntity));

        var linkExists = await linkRepo.ExistsAsync(doctorId, patientId);
        if (linkExists)
            return (true, true, MapDoctor(doctorEntity), MapPatient(patientEntity));

        await linkRepo.AddAsync(doctorId, patientId);
        return (true, false, MapDoctor(doctorEntity), MapPatient(patientEntity));
    }

    /// <summary>
    /// Unlinks a patient from a doctor.
    /// </summary>
    /// <param name="doctorId">The doctor identifier.</param>
    /// <param name="patientId">The patient identifier.</param>
    /// <returns>
    /// A tuple indicating whether an existing link was removed (<c>found</c>) and the projected doctor and patient DTOs.
    /// </returns>
    public async Task<(bool found, DoctorDto? doctor, PatientDto? patient)> UnlinkPatientAsync(int doctorId, int patientId)
    {
        // Fetch the entities up front to return them even after unlink
        var doctorEntity = await doctorRepo.GetByIdAsync(doctorId);
        var patientEntity = await patientRepo.GetByIdAsync(patientId);
        var removed = await linkRepo.RemoveAsync(doctorId, patientId);
        return removed
            ? (true, MapDoctor(doctorEntity), MapPatient(patientEntity))
            : (false, MapDoctor(doctorEntity), MapPatient(patientEntity));
    }

    private static DoctorDto? MapDoctor(Doctor? d)
        => d is null ? null : new DoctorDto(d.Id, d.FirstName, d.LastName, d.Specialty);

    private static PatientDto? MapPatient(Patient? p)
        => p is null ? null : new PatientDto(p.Id, p.FirstName, p.LastName);
}
