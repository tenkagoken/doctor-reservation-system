# Doctor Reservation System

An ASP.NET Core MVC application for selecting a doctor, viewing that doctor's available appointments, and reserving a time with a name and national ID. SQL Server stores doctors, appointment slots, and customers.

## Requirements

- Visual Studio 2022 with the ASP.NET and web development workload (or .NET 8 SDK)
- SQL Server and SQL Server Management Studio (SSMS)

## Set up

1. Open SSMS and connect to your SQL Server instance. Execute `database/setup.sql`. It creates the `DoctorReservationSystem` database and sample data. Running the script again does not erase bookings.
2. Open `DoctorReservationSystem.csproj` in Visual Studio. Edit `appsettings.json` if your SQL Server instance is not `localhost` (for SQL Express, use `Server=.\\SQLEXPRESS`). Windows authentication is configured by default.
3. Restore NuGet packages, then press F5. The site launches at `https://localhost:7168`.

The sample doctors and portraits are fictional. Appointment times are seeded in local server time for the next seven days. SSMS can be used to add more rows to `Doctors` and `Appointments`.

## Data model

- `Doctor` has many `Appointment` records; an appointment belongs to exactly one doctor.
- `User` has a name and a 14-digit national ID and may hold multiple appointments.
- `Appointment.IsBooked` and `Appointment.UserId` change together when a reservation succeeds. A conditional update prevents double booking.

The confirmation page shows only the booking reference and visit details, not the national ID. In a real deployment, add authentication and data retention rules for customer records.
