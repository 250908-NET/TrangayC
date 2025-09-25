using System.Threading.Tasks;
using CareLink.WebApi.Dtos;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories.Interfaces;
using CareLink.WebApi.Services;
using Moq;
using Xunit;
using FluentAssertions;

namespace CareLink.WebApi.Tests.Services;

public class DoctorPatientServiceTests
{
    private readonly Mock<IDoctorRepository> _doctorRepo;
    private readonly Mock<IPatientRepository> _patientRepo;
    private readonly Mock<IDoctorPatientRepository> _linkRepo;
    private readonly DoctorPatientService _service;

    public DoctorPatientServiceTests()
    {
        _doctorRepo = new Mock<IDoctorRepository>();
        _patientRepo = new Mock<IPatientRepository>();
        _linkRepo = new Mock<IDoctorPatientRepository>();
        _service = new DoctorPatientService(_doctorRepo.Object, _patientRepo.Object, _linkRepo.Object);
    }

    [Fact]
    public async Task LinkPatientAsync_WhenDoctorOrPatientMissing_ShouldReturnFalse()
    {
        // Arrange
        _doctorRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Doctor?)null);
        _patientRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Patient { Id = 2, FirstName = "Jane", LastName = "Doe" });

        // Act
        var result = await _service.LinkPatientAsync(1, 2);

        // Assert
        result.found.Should().BeFalse();
        result.alreadyLinked.Should().BeFalse();
        result.doctor.Should().BeNull();
        result.patient.Should().NotBeNull();
        _linkRepo.Verify(r => r.AddAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task LinkPatientAsync_WhenLinkExists_ShouldReturnAlreadyLinked()
    {
        // Arrange
        _doctorRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Doctor { Id = 1, FirstName = "Greg", LastName = "House", Specialty = "Diagnostics" });
        _patientRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Patient { Id = 2, FirstName = "Jane", LastName = "Doe" });
        _linkRepo.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(true);

        // Act
        var result = await _service.LinkPatientAsync(1, 2);

        // Assert
        result.found.Should().BeTrue();
        result.alreadyLinked.Should().BeTrue();
        result.doctor!.Id.Should().Be(1);
        result.patient!.Id.Should().Be(2);
        _linkRepo.Verify(r => r.AddAsync(1, 2), Times.Never);
    }

    [Fact]
    public async Task LinkPatientAsync_WhenLinkNotExisting_ShouldAddLink()
    {
        // Arrange
        _doctorRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Doctor { Id = 1, FirstName = "Greg", LastName = "House", Specialty = "Diagnostics" });
        _patientRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Patient { Id = 2, FirstName = "Jane", LastName = "Doe" });
        _linkRepo.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(false);

        // Act
        var result = await _service.LinkPatientAsync(1, 2);

        // Assert
        result.found.Should().BeTrue();
        result.alreadyLinked.Should().BeFalse();
        result.doctor!.Id.Should().Be(1);
        result.patient!.Id.Should().Be(2);
        _linkRepo.Verify(r => r.AddAsync(1, 2), Times.Once);
    }

    [Fact]
    public async Task UnlinkPatientAsync_WhenRemovalSucceeds_ShouldReturnTrue()
    {
        // Arrange
        _doctorRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Doctor { Id = 1, FirstName = "Greg", LastName = "House", Specialty = "Diagnostics" });
        _patientRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Patient { Id = 2, FirstName = "Jane", LastName = "Doe" });
        _linkRepo.Setup(r => r.RemoveAsync(1, 2)).ReturnsAsync(true);

        // Act
        var result = await _service.UnlinkPatientAsync(1, 2);

        // Assert
        result.found.Should().BeTrue();
        result.doctor!.Id.Should().Be(1);
        result.patient!.Id.Should().Be(2);
        _linkRepo.Verify(r => r.RemoveAsync(1, 2), Times.Once);
    }

    [Fact]
    public async Task UnlinkPatientAsync_WhenNoLink_ShouldReturnFalse()
    {
        // Arrange
        _doctorRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Doctor { Id = 1, FirstName = "Greg", LastName = "House", Specialty = "Diagnostics" });
        _patientRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Patient { Id = 2, FirstName = "Jane", LastName = "Doe" });
        _linkRepo.Setup(r => r.RemoveAsync(1, 2)).ReturnsAsync(false);

        // Act
        var result = await _service.UnlinkPatientAsync(1, 2);

        // Assert
        result.found.Should().BeFalse();
        result.doctor!.Id.Should().Be(1);
        result.patient!.Id.Should().Be(2);
        _linkRepo.Verify(r => r.RemoveAsync(1, 2), Times.Once);
    }
}

