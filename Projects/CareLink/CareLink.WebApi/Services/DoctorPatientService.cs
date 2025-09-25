using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services.Interfaces;
using CareLink.WebApi.Models;
using CareLink.WebApi.Dtos;

namespace CareLink.WebApi.Services;

public class DoctorPatientService(
    IDoctorRepository doctorRepo,
    IPatientRepository patientRepo,
    IDoctorPatientRepository linkRepo) : IDoctorPatientService
{
    public async Task<(bool found, bool alreadyLinked, DoctorDto? doctor, PatientDto? patient)> LinkPatientAsync(int doctorId, int patientId)
    {
        var doctor = await doctorRepo.GetByIdAsync(doctorId);
        var patient = await patientRepo.GetByIdAsync(patientId);
        if (doctor is null || patient is null)
            return (false, false, MapDoctor(doctor), MapPatient(patient));

        var exists = await linkRepo.ExistsAsync(doctorId, patientId);
        if (exists)
            return (true, true, MapDoctor(doctor), MapPatient(patient));

        await linkRepo.AddAsync(doctorId, patientId);
        return (true, false, MapDoctor(doctor), MapPatient(patient));
    }

    public async Task<(bool found, DoctorDto? doctor, PatientDto? patient)> UnlinkPatientAsync(int doctorId, int patientId)
    {
        // Fetch the entities up front to return them even after unlink
        var doctor = await doctorRepo.GetByIdAsync(doctorId);
        var patient = await patientRepo.GetByIdAsync(patientId);
        var removed = await linkRepo.RemoveAsync(doctorId, patientId);
        return removed
            ? (true, MapDoctor(doctor), MapPatient(patient))
            : (false, MapDoctor(doctor), MapPatient(patient));
    }

    private static DoctorDto? MapDoctor(Doctor? d)
        => d is null ? null : new DoctorDto(d.Id, d.FirstName, d.LastName, d.Specialty);

    private static PatientDto? MapPatient(Patient? p)
        => p is null ? null : new PatientDto(p.Id, p.FirstName, p.LastName);
}
