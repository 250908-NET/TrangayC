using CareLink.WebApi.Data;
using CareLink.WebApi.Helpers;
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

doctors.MapGet("", async (IDoctorService service) =>
{
    var list = await service.GetAllAsync();
    return Results.Ok(ApiResponseHelper.Success(list, "Doctors retrieved successfully"));
});

doctors.MapGet("/{id:int}", async (int id, IDoctorService service) =>
{
    var item = await service.GetByIdAsync(id);
    return item is null
        ? Results.NotFound(ApiResponseHelper.Error("Doctor not found"))
        : Results.Ok(ApiResponseHelper.Success(item, "Doctor retrieved successfully"));
});

doctors.MapPost("", async (Doctor doctor, IDoctorService service) =>
{
    var (isValid, errors) = BasicModelValidator.Validate(doctor);
    if (!isValid)
        return Results.BadRequest(ApiResponseHelper.Error(errors));

    var created = await service.CreateAsync(doctor);
    return Results.Created($"/api/doctors/{created.Id}", ApiResponseHelper.Success(created, "Doctor created successfully"));
});

doctors.MapDelete("/{id:int}", async (int id, IDoctorService service) =>
{
    var existing = await service.GetByIdAsync(id);
    if (existing is null)
        return Results.NotFound(ApiResponseHelper.Error("Doctor not found"));

    var deleted = await service.DeleteAsync(id);
    return deleted
        ? Results.Ok(ApiResponseHelper.Success(existing, "Doctor deleted successfully"))
        : Results.BadRequest(ApiResponseHelper.Error("Failed to delete doctor"));
});

var patients = app.MapGroup("/api/patients");

patients.MapGet("", async (IPatientService service) =>
{
    var list = await service.GetAllAsync();
    return Results.Ok(ApiResponseHelper.Success(list, "Patients retrieved successfully"));
});

patients.MapGet("/{id:int}", async (int id, IPatientService service) =>
{
    var item = await service.GetByIdAsync(id);
    return item is null
        ? Results.NotFound(ApiResponseHelper.Error("Patient not found"))
        : Results.Ok(ApiResponseHelper.Success(item, "Patient retrieved successfully"));
});

patients.MapPost("", async (Patient patient, IPatientService service) =>
{
    var (isValid, errors) = BasicModelValidator.Validate(patient);
    if (!isValid)
        return Results.BadRequest(ApiResponseHelper.Error(errors));

    var created = await service.CreateAsync(patient);
    return Results.Created($"/api/patients/{created.Id}", ApiResponseHelper.Success(created, "Patient created successfully"));
});

patients.MapDelete("/{id:int}", async (int id, IPatientService service) =>
{
    var existing = await service.GetByIdAsync(id);
    if (existing is null)
        return Results.NotFound(ApiResponseHelper.Error("Patient not found"));

    var deleted = await service.DeleteAsync(id);
    return deleted
        ? Results.Ok(ApiResponseHelper.Success(existing, "Patient deleted successfully"))
        : Results.BadRequest(ApiResponseHelper.Error("Failed to delete patient"));
});

// Link a patient to a doctor
doctors.MapPost("/{doctorId:int}/patients/{patientId:int}", async (int doctorId, int patientId, CareLinkDbContext db) =>
{
    var doctor = await db.Doctors.FindAsync(doctorId);
    var patient = await db.Patients.FindAsync(patientId);
    if (doctor is null || patient is null) return Results.NotFound(ApiResponseHelper.Error("Doctor or patient not found"));

    var exists = await db.Set<DoctorPatient>()
        .AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
    if (exists)
    {
        var linkData = new { doctorId, patientId };
        return Results.Ok(ApiResponseHelper.Success(linkData, "Patient already linked to doctor"));
    }

    var link = new DoctorPatient { DoctorId = doctorId, PatientId = patientId };
    db.Set<DoctorPatient>().Add(link);
    await db.SaveChangesAsync();
    var createdData = new { doctorId, patientId };
    return Results.Created($"/api/doctors/{doctorId}/patients/{patientId}", ApiResponseHelper.Success(createdData, "Patient linked to doctor successfully"));
});

// Unlink a patient from a doctor
doctors.MapDelete("/{doctorId:int}/patients/{patientId:int}", async (int doctorId, int patientId, CareLinkDbContext db) =>
{
    var link = await db.Set<DoctorPatient>()
        .FirstOrDefaultAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
    if (link is null) return Results.NotFound(ApiResponseHelper.Error("Doctor to patient link not found"));

    db.Remove(link);
    await db.SaveChangesAsync();
    var deletedData = new { doctorId, patientId };
    return Results.Ok(ApiResponseHelper.Success(deletedData, "Patient unlinked from doctor successfully"));
});

// Mirror routes under patients
patients.MapPost("/{patientId:int}/doctors/{doctorId:int}", async (int patientId, int doctorId, CareLinkDbContext db) =>
{
    var doctor = await db.Doctors.FindAsync(doctorId);
    var patient = await db.Patients.FindAsync(patientId);
    if (doctor is null || patient is null) return Results.NotFound(ApiResponseHelper.Error("Doctor or patient not found"));

    var exists = await db.Set<DoctorPatient>()
        .AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
    if (exists)
    {
        var linkData = new { doctorId, patientId };
        return Results.Ok(ApiResponseHelper.Success(linkData, "Doctor already linked to patient"));
    }

    var link = new DoctorPatient { DoctorId = doctorId, PatientId = patientId };
    db.Set<DoctorPatient>().Add(link);
    await db.SaveChangesAsync();
    var createdData = new { doctorId, patientId };
    return Results.Created($"/api/patients/{patientId}/doctors/{doctorId}", ApiResponseHelper.Success(createdData, "Doctor linked to patient successfully"));
});

patients.MapDelete("/{patientId:int}/doctors/{doctorId:int}", async (int patientId, int doctorId, CareLinkDbContext db) =>
{
    var link = await db.Set<DoctorPatient>()
        .FirstOrDefaultAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
    if (link is null) return Results.NotFound(ApiResponseHelper.Error("Patient to doctor link not found"));

    db.Remove(link);
    await db.SaveChangesAsync();
    var deletedData = new { doctorId, patientId };
    return Results.Ok(ApiResponseHelper.Success(deletedData, "Doctor unlinked from patient successfully"));
});

app.Run();
