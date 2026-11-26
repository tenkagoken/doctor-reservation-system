namespace DoctorReservationSystem.Models;

public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public bool IsBooked { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
}
