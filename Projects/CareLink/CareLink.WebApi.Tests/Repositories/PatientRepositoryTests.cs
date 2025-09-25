using System.Linq;
using System.Threading.Tasks;
using CareLink.WebApi.Data;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories;
using CareLink.WebApi.Tests.TestUtilities;
using Xunit;
using FluentAssertions;

namespace CareLink.WebApi.Tests.Repositories;

public class PatientRepositoryTests : RepositoryTestBase
{
    private readonly PatientRepository _repo;

    public PatientRepositoryTests()
    {
        _repo = new PatientRepository(Ctx);
    }

    [Fact]
    public async Task CreateAsync_WhenValidPatientProvided_ShouldReturnPatient()
    {
        // Arrange
        var inputPatient = new Patient { FirstName = "Jane", LastName = "Doe" };

        // Act
        var created = await _repo.AddAsync(inputPatient);

        // Assert
        created.Id.Should().BeGreaterThan(0);

        var fetched = await _repo.GetByIdAsync(created.Id);
        fetched.Should().NotBeNull();
        fetched.FirstName.Should().Be("Jane");
    }

    [Fact]
    public async Task GetAllAsync_WhenPatientsHaveDoctors_ShouldIncludeDoctors()
    {
        // Arrange
        var inputDoctor = new Doctor { FirstName = "Greg", LastName = "House", Specialty = "Brain" };
        var inputPatient = new Patient { FirstName = "Jane", LastName = "Doe" };
        Ctx.Doctors.Add(inputDoctor);
        Ctx.Patients.Add(inputPatient);
        await Ctx.SaveChangesAsync();
        Ctx.DoctorPatients.Add(new DoctorPatient { DoctorId = inputDoctor.Id, PatientId = inputPatient.Id });
        await Ctx.SaveChangesAsync();

        // Act
        var all = await _repo.GetAllAsync();

        // Assert
        var patient = Assert.Single(all);
        patient.FirstName.Should().Be("Jane");
        patient.DoctorPatients.Should().ContainSingle();
        patient.DoctorPatients.First().Doctor.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenExistingId_ShouldReturnTrue()
    {
        // Arrange
        var inputPatient = new Patient { FirstName = "John", LastName = "Smith" };
        Ctx.Patients.Add(inputPatient);
        await Ctx.SaveChangesAsync();

        // Act
        var deleted = await _repo.DeleteAsync(inputPatient.Id);
        
        // Assert
        deleted.Should().BeTrue();
        var fetched = await _repo.GetByIdAsync(inputPatient.Id);
        fetched.Should().BeNull();
    }
}

