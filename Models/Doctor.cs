using System.ComponentModel.DataAnnotations;

namespace DoctorReservationSystem.Models;

public class Doctor
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Name { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Specialization { get; set; } = string.Empty;
    [Required, Phone, StringLength(25)] public string PhoneNumber { get; set; } = string.Empty;
    [Required, StringLength(200)] public string PhotoUrl { get; set; } = string.Empty;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
