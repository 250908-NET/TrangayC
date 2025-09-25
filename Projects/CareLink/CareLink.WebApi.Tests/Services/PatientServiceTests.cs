using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace CareLink.WebApi.Tests.Services;

public class PatientServiceTests
{
    private readonly Mock<IPatientRepository> _patientRepositoryMock;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        _patientRepositoryMock = new Mock<IPatientRepository>();
        _patientService = new PatientService(_patientRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenValidPatientProvided_ShouldReturnPatient()
    {
        // Arrange
        var inputPatient = new Patient { FirstName = "Jane", LastName = "Doe" };
        var expectedPatient = new Patient { Id = 99, FirstName = "Jane", LastName = "Doe" };
        _patientRepositoryMock.Setup(repo => repo.AddAsync(inputPatient))
            .ReturnsAsync(expectedPatient);

        // Act
        var createdPatient = await _patientService.CreateAsync(inputPatient);

        // Assert: the created patient has an Id and the repository was called once with expected values
        createdPatient.Should().NotBeNull();
        createdPatient.Should().BeEquivalentTo(expectedPatient);
        _patientRepositoryMock.Verify(repo => repo.AddAsync(inputPatient), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenPatientsHaveDoctors_ShouldProjectPatientsWithRelatedDoctors()
    {
        // Arrange
        var expectedPatients = new List<Patient>
        {
            new Patient
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DoctorPatients = new List<DoctorPatient>
                {
                    new DoctorPatient
                    {
                        Doctor = new Doctor
                        {
                            Id = 4,
                            FirstName = "Kevin",
                            LastName = "Morgan",
                            Specialty = "Cardiology"
                        }
                    }
                }
            }
        };

        _patientRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedPatients);

        // Act
        var projectedPatients = await _patientService.GetAllAsync();

        // Assert
        projectedPatients.Should().NotBeNull();
        var patient = Assert.Single(projectedPatients);
        var jsonShape = JsonSerializer.Serialize(patient);
        Assert.Contains("\"Id\":1", jsonShape);
        Assert.Contains("\"FirstName\":\"John\"", jsonShape);
        Assert.Contains("\"LastName\":\"Doe\"", jsonShape);
        Assert.Contains("\"doctors\"", jsonShape);
        Assert.Contains("\"Id\":4", jsonShape);
        Assert.Contains("\"FirstName\":\"Kevin\"", jsonShape);
        Assert.Contains("\"LastName\":\"Morgan\"", jsonShape);
        Assert.Contains("\"Specialty\":\"Cardiology\"", jsonShape); 
        _patientRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenExistingId_ShouldReturnTrue()
    {
        // Arrange
        var patientId = 5;
        _patientRepositoryMock.Setup(r => r.DeleteAsync(patientId)).ReturnsAsync(true);

        // Act
        var deleted = await _patientService.DeleteAsync(patientId);
        
        // Assert
        deleted.Should().BeTrue();
        _patientRepositoryMock.Verify(r => r.DeleteAsync(patientId), Times.Once);
    }
}
