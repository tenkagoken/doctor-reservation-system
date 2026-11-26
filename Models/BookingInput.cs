using System.ComponentModel.DataAnnotations;

namespace DoctorReservationSystem.Models;

public class BookingInput
{
    public int AppointmentId { get; set; }
    [Required(ErrorMessage = "Please enter your name."), StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Please enter your national ID."), RegularExpression(@"^[0-9]{14}$", ErrorMessage = "Enter a 14-digit national ID.")]
    public string NationalId { get; set; } = string.Empty;
}
