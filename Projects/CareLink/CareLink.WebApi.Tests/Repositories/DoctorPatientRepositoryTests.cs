using System.Threading.Tasks;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories;
using CareLink.WebApi.Tests.TestUtilities;
using Xunit;
using FluentAssertions;

namespace CareLink.WebApi.Tests.Repositories;

public class DoctorPatientRepositoryTests : RepositoryTestBase
{
    private readonly DoctorPatientRepository _repo;

    public DoctorPatientRepositoryTests()
    {
        _repo = new DoctorPatientRepository(Ctx);
    }

    [Fact]
    public async Task ExistsAsync_WhenNoLink_ShouldReturnFalse()
    {
        // Arrange
        var inputDoctor = new Doctor { FirstName = "Greg", LastName = "House", Specialty = "Heart" };
        var inputPatient = new Patient { FirstName = "Jane", LastName = "Doe" };
        Ctx.Doctors.Add(inputDoctor);
        Ctx.Patients.Add(inputPatient);
        await Ctx.SaveChangesAsync();

        // Act
        var exists = await _repo.ExistsAsync(inputDoctor.Id, inputPatient.Id);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_WhenValidIds_ShouldCreateLink()
    {
        // Arrange
        var inputDoctor = new Doctor { FirstName = "Greg", LastName = "House", Specialty = "Heart" };
        var inputPatient = new Patient { FirstName = "Jane", LastName = "Doe" };
        Ctx.Doctors.Add(inputDoctor);
        Ctx.Patients.Add(inputPatient);
        await Ctx.SaveChangesAsync();

        // Act
        await _repo.AddAsync(inputDoctor.Id, inputPatient.Id);

        // Assert
        var exists = await _repo.ExistsAsync(inputDoctor.Id, inputPatient.Id);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveAsync_WhenExistingLink_ShouldReturnTrue()
    {
        // Arrange
        var inputDoctor = new Doctor { FirstName = "Greg", LastName = "House", Specialty = "Heart" };
        var inputPatient = new Patient { FirstName = "Jane", LastName = "Doe" };
        Ctx.Doctors.Add(inputDoctor);
        Ctx.Patients.Add(inputPatient);
        await Ctx.SaveChangesAsync();
        Ctx.DoctorPatients.Add(new DoctorPatient { DoctorId = inputDoctor.Id, PatientId = inputPatient.Id });
        await Ctx.SaveChangesAsync();

        // Act
        var removed = await _repo.RemoveAsync(inputDoctor.Id, inputPatient.Id);

        // Assert
        removed.Should().BeTrue();
        var exists = await _repo.ExistsAsync(inputDoctor.Id, inputPatient.Id);
        exists.Should().BeFalse();
    }
  }
