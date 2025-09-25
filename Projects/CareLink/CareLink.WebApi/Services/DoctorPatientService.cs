using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services.Interfaces;
using CareLink.WebApi.Models;

namespace CareLink.WebApi.Services;

public class DoctorPatientService(
    IDoctorRepository doctorRepo,
    IPatientRepository patientRepo,
    IDoctorPatientRepository linkRepo) : IDoctorPatientService
{
    public async Task<(bool found, bool alreadyLinked, Doctor? doctor, Patient? patient)> LinkPatientAsync(int doctorId, int patientId)
    {
        var doctor = await doctorRepo.GetByIdAsync(doctorId);
        var patient = await patientRepo.GetByIdAsync(patientId);
        if (doctor is null || patient is null)
            return (false, false, doctor, patient);

        var exists = await linkRepo.ExistsAsync(doctorId, patientId);
        if (exists)
            return (true, true, doctor, patient);

        await linkRepo.AddAsync(doctorId, patientId);
        return (true, false, doctor, patient);
    }

    public async Task<(bool found, Doctor? doctor, Patient? patient)> UnlinkPatientAsync(int doctorId, int patientId)
    {
        // Fetch the entities up front to return them even after unlink
        var doctor = await doctorRepo.GetByIdAsync(doctorId);
        var patient = await patientRepo.GetByIdAsync(patientId);
        var removed = await linkRepo.RemoveAsync(doctorId, patientId);
        return removed
            ? (true, doctor, patient)
            : (false, doctor, patient);
    }
}
