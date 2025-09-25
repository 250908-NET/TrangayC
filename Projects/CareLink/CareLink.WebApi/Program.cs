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

/// <summary>
/// Doctor endpoints (CRUD and relationship links under doctors).
/// Base route: /api/doctors
/// </summary>
var doctorRoutes = app.MapGroup("/api/doctors");

/// <summary>
/// Retrieves all doctors with related patients (list).
/// </summary>
doctorRoutes.MapGet("", async (IDoctorService doctorService) =>
{
    var doctorList = await doctorService.GetAllAsync();
    return Results.Ok(ApiResponseHelper.Success(doctorList, "Doctors retrieved successfully"));
});

/// <summary>
/// Retrieves a specific doctor by identifier.
/// </summary>
doctorRoutes.MapGet("/{id:int}", async (int id, IDoctorService doctorService) =>
{
    var doctorItem = await doctorService.GetByIdAsync(id);
    return doctorItem is null
        ? Results.NotFound(ApiResponseHelper.Error("Doctor not found"))
        : Results.Ok(ApiResponseHelper.Success(doctorItem, "Doctor retrieved successfully"));
});

/// <summary>
/// Creates a new doctor.
/// </summary>
doctorRoutes.MapPost("", async (Doctor doctor, IDoctorService doctorService) =>
{
    var (isValid, errors) = BasicModelValidator.Validate(doctor);
    if (!isValid)
        return Results.BadRequest(ApiResponseHelper.Error(errors));

    var createdDoctor = await doctorService.CreateAsync(doctor);
    return Results.Created($"/api/doctors/{createdDoctor.Id}", ApiResponseHelper.Success(createdDoctor, "Doctor created successfully"));
});

/// <summary>
/// Deletes a doctor by identifier.
/// </summary>
doctorRoutes.MapDelete("/{id:int}", async (int id, IDoctorService doctorService) =>
{
    var existingDoctor = await doctorService.GetByIdAsync(id);
    if (existingDoctor is null)
        return Results.NotFound(ApiResponseHelper.Error("Doctor not found"));

    var doctorDeleted = await doctorService.DeleteAsync(id);
    return doctorDeleted
        ? Results.Ok(ApiResponseHelper.Success(existingDoctor, "Doctor deleted successfully"))
        : Results.BadRequest(ApiResponseHelper.Error("Failed to delete doctor"));
});

/// <summary>
/// Patient endpoints (CRUD and relationship links under patients).
/// Base route: /api/patients
/// </summary>
var patientRoutes = app.MapGroup("/api/patients");

/// <summary>
/// Retrieves all patients with related doctors (list).     
/// </summary>
patientRoutes.MapGet("", async (IPatientService patientService) =>
{
    var patientList = await patientService.GetAllAsync();
    return Results.Ok(ApiResponseHelper.Success(patientList, "Patients retrieved successfully"));
});

/// <summary>
/// Retrieves a specific patient by identifier.
/// </summary>
patientRoutes.MapGet("/{id:int}", async (int id, IPatientService patientService) =>
{
    var patientItem = await patientService.GetByIdAsync(id);
    return patientItem is null
        ? Results.NotFound(ApiResponseHelper.Error("Patient not found"))
        : Results.Ok(ApiResponseHelper.Success(patientItem, "Patient retrieved successfully"));
});

/// <summary>
/// Creates a new patient.
/// </summary>
patientRoutes.MapPost("", async (Patient patient, IPatientService patientService) =>
{
    var (isValid, errors) = BasicModelValidator.Validate(patient);
    if (!isValid)
        return Results.BadRequest(ApiResponseHelper.Error(errors));

    var createdPatient = await patientService.CreateAsync(patient);
    return Results.Created($"/api/patients/{createdPatient.Id}", ApiResponseHelper.Success(createdPatient, "Patient created successfully"));
});

/// <summary>
/// Deletes a patient by identifier.
/// </summary>
patientRoutes.MapDelete("/{id:int}", async (int id, IPatientService patientService) =>
{
    var existingPatient = await patientService.GetByIdAsync(id);
    if (existingPatient is null)
        return Results.NotFound(ApiResponseHelper.Error("Patient not found"));

    var patientDeleted = await patientService.DeleteAsync(id);
    return patientDeleted
        ? Results.Ok(ApiResponseHelper.Success(existingPatient, "Patient deleted successfully"))
        : Results.BadRequest(ApiResponseHelper.Error("Failed to delete patient"));
});

// Link a patient to a doctor (under doctors)
doctorRoutes.MapPost("/{doctorId:int}/patients/{patientId:int}", async (int doctorId, int patientId, IDoctorPatientService linkService) =>
{
    var result = await linkService.LinkPatientAsync(doctorId, patientId);
    var data = new { doctor = result.doctor, patient = result.patient };
    if (!result.found)
        return Results.NotFound(ApiResponseHelper.Error("Doctor or patient not found"));

    if (result.alreadyLinked)
        return Results.Ok(ApiResponseHelper.Success(data, "Patient already linked to doctor"));

    return Results.Created($"/api/doctors/{doctorId}/patients/{patientId}", ApiResponseHelper.Success(data, "Patient linked to doctor successfully"));
});

// Unlink a patient from a doctor (under doctors)
doctorRoutes.MapDelete("/{doctorId:int}/patients/{patientId:int}", async (int doctorId, int patientId, IDoctorPatientService linkService) =>
{
    var result = await linkService.UnlinkPatientAsync(doctorId, patientId);
    if (!result.found) return Results.NotFound(ApiResponseHelper.Error("Doctor to patient link not found"));

    var deletedData = new { doctor = result.doctor, patient = result.patient };
    return Results.Ok(ApiResponseHelper.Success(deletedData, "Patient unlinked from doctor successfully"));
});

// Mirror routes under patients
patientRoutes.MapPost("/{patientId:int}/doctors/{doctorId:int}", async (int patientId, int doctorId, IDoctorPatientService linkService) =>
{
    var result = await linkService.LinkPatientAsync(doctorId, patientId);
    var data = new { patient = result.patient, doctor = result.doctor };
    if (!result.found)
        return Results.NotFound(ApiResponseHelper.Error("Doctor or patient not found"));

    if (result.alreadyLinked)
        return Results.Ok(ApiResponseHelper.Success(data, "Doctor already linked to patient"));

    return Results.Created($"/api/patients/{patientId}/doctors/{doctorId}", ApiResponseHelper.Success(data, "Doctor linked to patient successfully"));
});

// Mirror unlink route under patients
patientRoutes.MapDelete("/{patientId:int}/doctors/{doctorId:int}", async (int patientId, int doctorId, IDoctorPatientService linkService) =>
{
    var result = await linkService.UnlinkPatientAsync(doctorId, patientId);
    if (!result.found) return Results.NotFound(ApiResponseHelper.Error("Patient to doctor link not found"));

    var deletedData = new { patient = result.patient, doctor = result.doctor };
    return Results.Ok(ApiResponseHelper.Success(deletedData, "Doctor unlinked from patient successfully"));
});

app.Run();
