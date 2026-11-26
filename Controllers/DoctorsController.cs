using DoctorReservationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorReservationSystem.Controllers;

public class DoctorsController(ReservationDbContext db) : Controller
{
    [HttpGet("doctors/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var doctor = await db.Doctors.Include(d => d.Appointments)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (doctor is null) return NotFound();
        doctor.Appointments = doctor.Appointments.Where(a => !a.IsBooked && a.StartsAt > DateTime.Now)
            .OrderBy(a => a.StartsAt).ToList();
        return View(doctor);
    }
}
