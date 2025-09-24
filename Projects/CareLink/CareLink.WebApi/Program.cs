using CareLink.WebApi.Data;
using CareLink.WebApi.Repositories;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services;
using CareLink.WebApi.Services.Interfaces;
using CareLink.WebApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// DbContext
//Read connection_string.env file
builder.Services.AddDbContext<CareLinkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Repositories
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

// Services
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var doctors = app.MapGroup("/api/doctors");

doctors.MapGet("", async (CareLinkDbContext db) =>
{
    var list = await db.Doctors
        .Include(d => d.DoctorPatients)
            .ThenInclude(dp => dp.Patient)
        .Select(d => new
        {
            d.Id,
            d.FirstName,
            d.LastName,
            d.Specialty,
            patients = d.DoctorPatients
                .Where(dp => dp.Patient != null)
                .Select(dp => new
                {
                    dp.Patient!.Id,
                    dp.Patient!.FirstName,
                    dp.Patient!.LastName
                }).ToList()
        })
        .ToListAsync();
    return Results.Ok(list);
});

doctors.MapGet("/{id:int}", async (int id, IDoctorService service) =>
{
    var item = await service.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

doctors.MapPost("", async (Doctor doctor, IDoctorService service) =>
{
    if (string.IsNullOrWhiteSpace(doctor.FirstName) || string.IsNullOrWhiteSpace(doctor.LastName) || string.IsNullOrWhiteSpace(doctor.Specialty))
        return Results.BadRequest("FirstName, LastName, and Specialty are required.");
    var created = await service.CreateAsync(doctor);
    return Results.Created($"/api/doctors/{created.Id}", created);
});

doctors.MapDelete("/{id:int}", async (int id, IDoctorService service) =>
{
    var deleted = await service.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

var patients = app.MapGroup("/api/patients");

patients.MapGet("", async (CareLinkDbContext db) =>
{
    var list = await db.Patients
        .Include(p => p.DoctorPatients)
            .ThenInclude(dp => dp.Doctor)
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
        })
        .ToListAsync();
    return Results.Ok(list);
});

patients.MapGet("/{id:int}", async (int id, IPatientService service) =>
{
    var item = await service.GetByIdAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

patients.MapPost("", async (Patient patient, IPatientService service) =>
{
    if (string.IsNullOrWhiteSpace(patient.FirstName) || string.IsNullOrWhiteSpace(patient.LastName))
        return Results.BadRequest("FirstName and LastName are required.");
    var created = await service.CreateAsync(patient);
    return Results.Created($"/api/patients/{created.Id}", created);
});

patients.MapDelete("/{id:int}", async (int id, IPatientService service) =>
{
    var deleted = await service.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

// Link a patient to a doctor
doctors.MapPost("/{doctorId:int}/patients/{patientId:int}", async (int doctorId, int patientId, CareLinkDbContext db) =>
{
    var doctor = await db.Doctors.FindAsync(doctorId);
    var patient = await db.Patients.FindAsync(patientId);
    if (doctor is null || patient is null) return Results.NotFound();

    var exists = await db.Set<DoctorPatient>()
        .AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
    if (exists) return Results.NoContent();

    db.Set<DoctorPatient>().Add(new DoctorPatient { DoctorId = doctorId, PatientId = patientId });
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Unlink a patient from a doctor
doctors.MapDelete("/{doctorId:int}/patients/{patientId:int}", async (int doctorId, int patientId, CareLinkDbContext db) =>
{
    var link = await db.Set<DoctorPatient>()
        .FirstOrDefaultAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
    if (link is null) return Results.NotFound();

    db.Remove(link);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Mirror routes under patients
patients.MapPost("/{patientId:int}/doctors/{doctorId:int}", async (int patientId, int doctorId, CareLinkDbContext db) =>
{
    var doctor = await db.Doctors.FindAsync(doctorId);
    var patient = await db.Patients.FindAsync(patientId);
    if (doctor is null || patient is null) return Results.NotFound();

    var exists = await db.Set<DoctorPatient>()
        .AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
    if (exists) return Results.NoContent();

    db.Set<DoctorPatient>().Add(new DoctorPatient { DoctorId = doctorId, PatientId = patientId });
    await db.SaveChangesAsync();
    return Results.NoContent();
});

patients.MapDelete("/{patientId:int}/doctors/{doctorId:int}", async (int patientId, int doctorId, CareLinkDbContext db) =>
{
    var link = await db.Set<DoctorPatient>()
        .FirstOrDefaultAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
    if (link is null) return Results.NotFound();

    db.Remove(link);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
