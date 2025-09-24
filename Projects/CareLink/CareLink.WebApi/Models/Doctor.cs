using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareLink.WebApi.Models;

public class Doctor
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name must be between 1 and 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name must be between 1 and 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Specialty is required.")]
    [StringLength(150, ErrorMessage = "Specialty must be between 1 and 150 characters.")]
    public string Specialty { get; set; } = string.Empty;

    public List<DoctorPatient> DoctorPatients { get; set; } = new List<DoctorPatient>();
}
