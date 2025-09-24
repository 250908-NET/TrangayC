using CareLink.WebApi.Data;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareLink.WebApi.Repositories;

public class PatientRepository(CareLinkDbContext db) : IPatientRepository
{
    public async Task<Patient> AddAsync(Patient patient)
    {
        db.Patients.Add(patient);
        await db.SaveChangesAsync();
        return patient;
    }

    public Task<List<Patient>> GetAllAsync()
        => db.Patients.ToListAsync();

    public Task<Patient?> GetByIdAsync(int id)
        => db.Patients.FirstOrDefaultAsync(d => d.Id == id);

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await db.Patients.FindAsync([id]);
        if (entity is null) return false;
        db.Patients.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}
