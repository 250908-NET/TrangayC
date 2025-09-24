using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareLink.WebApi.Models;

public class Patient
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    public List<DoctorPatient> DoctorPatients { get; set; } = new List<DoctorPatient>();
}
