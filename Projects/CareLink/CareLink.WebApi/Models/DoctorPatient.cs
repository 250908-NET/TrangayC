using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareLink.WebApi.Models;

public class DoctorPatient
{
    [ForeignKey(nameof(Doctor))]
    public int DoctorId { get; set; }

    [ForeignKey(nameof(Patient))]
    public int PatientId { get; set; }

    public Doctor Doctor { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
}
