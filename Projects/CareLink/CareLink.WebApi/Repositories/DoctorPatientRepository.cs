using CareLink.WebApi.Data;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareLink.WebApi.Repositories;

public class DoctorPatientRepository(CareLinkDbContext db) : IDoctorPatientRepository
{
    public Task<bool> ExistsAsync(int doctorId, int patientId)
        => db.Set<DoctorPatient>().AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);

    public async Task AddAsync(int doctorId, int patientId)
    {
        db.Set<DoctorPatient>().Add(new DoctorPatient { DoctorId = doctorId, PatientId = patientId });
        await db.SaveChangesAsync();
    }

    public async Task<bool> RemoveAsync(int doctorId, int patientId)
    {
        var link = await db.Set<DoctorPatient>()
            .FirstOrDefaultAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
        if (link is null) return false;
        db.Remove(link);
        await db.SaveChangesAsync();
        return true;
    }
}
