using DoctorReservationSystem.Data;
using DoctorReservationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DoctorReservationSystem.Controllers;

public class BookingsController(ReservationDbContext db) : Controller
{
    [HttpGet("book/{id:int}")]
    public async Task<IActionResult> Create(int id)
    {
        var slot = await db.Appointments.Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsBooked && a.StartsAt > DateTime.Now);
        if (slot is null) return NotFound();
        ViewBag.Appointment = slot;
        return View(new BookingInput { AppointmentId = id });
    }

    [HttpPost("book/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int id, BookingInput input)
    {
        if (id != input.AppointmentId) return BadRequest();
        var slot = await db.Appointments.Include(a => a.Doctor).FirstOrDefaultAsync(a => a.Id == id);
        if (slot is null) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.Appointment = slot;
            return View(input);
        }

        if (slot.IsBooked || slot.StartsAt <= DateTime.Now) return RedirectToAction("Unavailable", new { doctorId = slot.DoctorId });

        // A serializable transaction and conditional update prevent two visitors taking the same slot.
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        try
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.NationalId == input.NationalId);
            if (user is null)
            {
                user = new User { Name = input.Name.Trim(), NationalId = input.NationalId };
                db.Users.Add(user);
                await db.SaveChangesAsync();
            }
            else
            {
                user.Name = input.Name.Trim();
                await db.SaveChangesAsync();
            }

            var changed = await db.Appointments.Where(a => a.Id == id && !a.IsBooked && a.StartsAt > DateTime.Now)
                .ExecuteUpdateAsync(setters => setters.SetProperty(a => a.IsBooked, true)
                    .SetProperty(a => a.UserId, user.Id));
            if (changed == 0)
            {
                await transaction.RollbackAsync();
                return RedirectToAction("Unavailable", new { doctorId = slot.DoctorId });
            }
            await transaction.CommitAsync();
            return RedirectToAction("Confirmation", new { id });
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(nameof(input.NationalId), "This ID was updated at the same time. Please try again.");
            ViewBag.Appointment = slot;
            return View(input);
        }
    }

    [HttpGet("booked/{id:int}")]
    public async Task<IActionResult> Confirmation(int id)
    {
        var slot = await db.Appointments.Include(a => a.Doctor).FirstOrDefaultAsync(a => a.Id == id && a.IsBooked);
        if (slot is null) return NotFound();
        return View(slot);
    }

    [HttpGet("unavailable")]
    public IActionResult Unavailable(int doctorId) { ViewBag.DoctorId = doctorId; return View(); }
}
