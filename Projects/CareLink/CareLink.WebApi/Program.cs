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
builder.Services.AddScoped<IDoctorPatientRepository, DoctorPatientRepository>();

// Services
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorPatientService, DoctorPatientService>();

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
doctors.MapPost("/{doctorId:int}/patients/{patientId:int}", async (int doctorId, int patientId, IDoctorPatientService linkService) =>
{
    var result = await linkService.LinkPatientAsync(doctorId, patientId);
    var data = new { doctor = result.doctor, patient = result.patient };
    if (!result.found)
        return Results.NotFound(ApiResponseHelper.Error("Doctor or patient not found"));

    if (result.alreadyLinked)
        return Results.Ok(ApiResponseHelper.Success(data, "Patient already linked to doctor"));

    return Results.Created($"/api/doctors/{doctorId}/patients/{patientId}", ApiResponseHelper.Success(data, "Patient linked to doctor successfully"));
});

// Unlink a patient from a doctor
doctors.MapDelete("/{doctorId:int}/patients/{patientId:int}", async (int doctorId, int patientId, IDoctorPatientService linkService) =>
{
    var result = await linkService.UnlinkPatientAsync(doctorId, patientId);
    if (!result.found) return Results.NotFound(ApiResponseHelper.Error("Doctor to patient link not found"));

    var deletedData = new { doctor = result.doctor, patient = result.patient };
    return Results.Ok(ApiResponseHelper.Success(deletedData, "Patient unlinked from doctor successfully"));
});

// Mirror routes under patients
patients.MapPost("/{patientId:int}/doctors/{doctorId:int}", async (int patientId, int doctorId, IDoctorPatientService linkService) =>
{
    var result = await linkService.LinkPatientAsync(doctorId, patientId);
    var data = new { patient = result.patient, doctor = result.doctor };
    if (!result.found)
        return Results.NotFound(ApiResponseHelper.Error("Doctor or patient not found"));

    if (result.alreadyLinked)
        return Results.Ok(ApiResponseHelper.Success(data, "Doctor already linked to patient"));

    return Results.Created($"/api/patients/{patientId}/doctors/{doctorId}", ApiResponseHelper.Success(data, "Doctor linked to patient successfully"));
});

patients.MapDelete("/{patientId:int}/doctors/{doctorId:int}", async (int patientId, int doctorId, IDoctorPatientService linkService) =>
{
    var result = await linkService.UnlinkPatientAsync(doctorId, patientId);
    if (!result.found) return Results.NotFound(ApiResponseHelper.Error("Patient to doctor link not found"));

    var deletedData = new { patient = result.patient, doctor = result.doctor };
    return Results.Ok(ApiResponseHelper.Success(deletedData, "Doctor unlinked from patient successfully"));
});

app.Run();
