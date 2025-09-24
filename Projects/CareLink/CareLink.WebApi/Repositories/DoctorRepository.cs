using CareLink.WebApi.Data;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareLink.WebApi.Repositories;

public class DoctorRepository(CareLinkDbContext db) : IDoctorRepository
{
    public async Task<Doctor> AddAsync(Doctor doctor)
    {
        db.Doctors.Add(doctor);
        await db.SaveChangesAsync();
        return doctor;
    }

    public Task<List<Doctor>> GetAllAsync()
        => db.Doctors.ToListAsync();

    public Task<Doctor?> GetByIdAsync(int id)
        => db.Doctors.FirstOrDefaultAsync(d => d.Id == id);

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await db.Doctors.FindAsync([id]);
        if (entity is null) return false;
        db.Doctors.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}
