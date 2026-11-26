using DoctorReservationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorReservationSystem.Controllers;

public class HomeController(ReservationDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.Doctors.OrderBy(d => d.Id).ToListAsync());

    [Route("error")]
    public IActionResult Error() => View();
}
