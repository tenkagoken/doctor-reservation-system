using System.ComponentModel.DataAnnotations;

namespace DoctorReservationSystem.Models;

public class User
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, RegularExpression(@"^[0-9]{14}$", ErrorMessage = "Enter a 14-digit national ID.")]
    [StringLength(14)] public string NationalId { get; set; } = string.Empty;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
