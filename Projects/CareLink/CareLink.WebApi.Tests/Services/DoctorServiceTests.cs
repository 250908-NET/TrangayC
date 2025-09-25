using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace CareLink.WebApi.Tests.Services;

public class DoctorServiceTests
{
    private readonly Mock<IDoctorRepository> _doctorRepositoryMock;
    private readonly DoctorService _doctorService;

    public DoctorServiceTests()
    {
        _doctorRepositoryMock = new Mock<IDoctorRepository>();
        _doctorService = new DoctorService(_doctorRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenValidDoctorProvided_ShouldReturnDoctor()
    {
        // Arrange
        var inputDoctor = new Doctor { FirstName = "Greg", LastName = "House", Specialty = "Diagnostics" };
        var expectedDoctor = new Doctor { Id = 42, FirstName = "Greg", LastName = "House", Specialty = "Diagnostics" };
        _doctorRepositoryMock.Setup(repo => repo.AddAsync(inputDoctor))
            .ReturnsAsync(expectedDoctor);

        // Act
        var createdDoctor = await _doctorService.CreateAsync(inputDoctor);

        // Assert
        createdDoctor.Should().NotBeNull();
        createdDoctor.Should().BeEquivalentTo(expectedDoctor);
        _doctorRepositoryMock.Verify(repo => repo.AddAsync(inputDoctor), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenDoctorsHavePatients_ShouldProjectPatients()
    {
        // Arrange
        // Seed doctor entities that the service will project
        var expectedDoctors = new List<Doctor>
        {
            new Doctor
            {
                Id = 1,
                FirstName = "Greg",
                LastName = "House",
                Specialty = "Brain",
                DoctorPatients = new List<DoctorPatient>
                {
                    new DoctorPatient
                    {
                        Patient = new Patient { Id = 2, FirstName = "Jane", LastName = "Doe" }
                    }
                }
            }
        };

        _doctorRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedDoctors);

        // Act
        var projectedDoctors = await _doctorService.GetAllAsync();

        // Assert
        projectedDoctors.Should().NotBeNull();
        var doctor = Assert.Single(projectedDoctors);
        var jsonShape = JsonSerializer.Serialize(doctor);
        Assert.Contains("\"Id\":1", jsonShape);
        Assert.Contains("\"FirstName\":\"Greg\"", jsonShape);
        Assert.Contains("\"LastName\":\"House\"", jsonShape);
        Assert.Contains("\"Specialty\":\"Brain\"", jsonShape);
        Assert.Contains("\"patients\"", jsonShape);
        Assert.Contains("\"Id\":2", jsonShape);
        Assert.Contains("\"FirstName\":\"Jane\"", jsonShape);
        Assert.Contains("\"LastName\":\"Doe\"", jsonShape);
        _doctorRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenExistingId_ShouldReturnTrue()
    {
        // Arrange
        var doctorId = 5;
        _doctorRepositoryMock.Setup(r => r.DeleteAsync(doctorId)).ReturnsAsync(true);

        // Act
        var deleted = await _doctorService.DeleteAsync(doctorId);
        
        // Assert
        deleted.Should().BeTrue();
        _doctorRepositoryMock.Verify(r => r.DeleteAsync(doctorId), Times.Once);
    }
}

