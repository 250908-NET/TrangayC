using System;
using System.Linq;
using System.Threading.Tasks;
using CareLink.WebApi.Data;
using CareLink.WebApi.Models;
using CareLink.WebApi.Repositories;
using CareLink.WebApi.Tests.TestUtilities;
using Xunit;
using FluentAssertions;

namespace CareLink.WebApi.Tests.Repositories;

public class DoctorRepositoryTests : RepositoryTestBase
{
    private readonly DoctorRepository _repo;

    public DoctorRepositoryTests()
    {
        _repo = new DoctorRepository(Ctx);
    }

    [Fact]
    public async Task CreateAsync_WhenValidDoctorProvided_ShouldReturnDoctor()
    {
        // Arrange
        var inputDoctor = new Doctor { FirstName = "Gregory", LastName = "House", Specialty = "Heart" };

        // Act
        var created = await _repo.AddAsync(inputDoctor);

        // Assert
        created.Id.Should().BeGreaterThan(0);

        // And we can fetch it back
        var fetched = await _repo.GetByIdAsync(created.Id);
        fetched.Should().NotBeNull();
        fetched.FirstName.Should().Be("Gregory");
    }

    [Fact]
    public async Task GetAllAsync_WhenDoctorsHavePatients_ShouldIncludePatients()
    {
        // Arrange
        var inputDoctor = new Doctor { FirstName = "Tom", LastName = "Hanks", Specialty = "Eye" };
        var inputPatient = new Patient { FirstName = "Jane", LastName = "Doe" };
        Ctx.Doctors.Add(inputDoctor);
        Ctx.Patients.Add(inputPatient);
        await Ctx.SaveChangesAsync();
        Ctx.DoctorPatients.Add(new DoctorPatient { DoctorId = inputDoctor.Id, PatientId = inputPatient.Id });
        await Ctx.SaveChangesAsync();

        // Act
        var all = await _repo.GetAllAsync();

        // Assert
        var d = Assert.Single(all);
        d.FirstName.Should().Be("Tom");
        d.DoctorPatients.Should().ContainSingle();
        d.DoctorPatients.First().Patient.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenExistingId_ShouldReturnTrue()
    {
        // Arrange
        var inputDoctor = new Doctor { FirstName = "Meredith", LastName = "Grey", Specialty = "Kidney" };
        Ctx.Doctors.Add(inputDoctor);
        await Ctx.SaveChangesAsync();

        // Act
        var deleted = await _repo.DeleteAsync(inputDoctor.Id);
        
        // Assert
        deleted.Should().BeTrue();
        var fetched = await _repo.GetByIdAsync(inputDoctor.Id);
        fetched.Should().BeNull();
    }
}

