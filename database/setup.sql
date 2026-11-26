-- Run this entire script in SQL Server Management Studio.
IF DB_ID(N'DoctorReservationSystem') IS NULL
    CREATE DATABASE DoctorReservationSystem;
GO
USE DoctorReservationSystem;
GO
IF OBJECT_ID(N'dbo.Doctors', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Doctors (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Doctors PRIMARY KEY,
        Name nvarchar(100) NOT NULL,
        Specialization nvarchar(100) NOT NULL,
        PhoneNumber nvarchar(25) NOT NULL,
        PhotoUrl nvarchar(200) NOT NULL
    );
END;
GO
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Name nvarchar(100) NOT NULL,
        NationalId varchar(14) NOT NULL,
        CONSTRAINT UQ_Users_NationalId UNIQUE (NationalId),
        CONSTRAINT CK_Users_NationalId CHECK (LEN(NationalId) = 14 AND NationalId NOT LIKE '%[^0-9]%')
    );
END;
GO
IF OBJECT_ID(N'dbo.Appointments', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Appointments (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Appointments PRIMARY KEY,
        DoctorId int NOT NULL,
        StartsAt datetime2 NOT NULL,
        EndsAt datetime2 NOT NULL,
        IsBooked bit NOT NULL CONSTRAINT DF_Appointments_IsBooked DEFAULT 0,
        UserId int NULL,
        CONSTRAINT FK_Appointments_Doctors_DoctorId FOREIGN KEY (DoctorId) REFERENCES dbo.Doctors(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Appointments_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT CK_Appointments_Time CHECK (EndsAt > StartsAt),
        CONSTRAINT CK_Appointments_Booking CHECK ((IsBooked = 0 AND UserId IS NULL) OR (IsBooked = 1 AND UserId IS NOT NULL)),
        CONSTRAINT UQ_Appointments_Doctor_Start UNIQUE (DoctorId, StartsAt)
    );
    CREATE INDEX IX_Appointments_UserId ON dbo.Appointments(UserId);
END;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Doctors)
BEGIN
    INSERT INTO dbo.Doctors (Name, Specialization, PhoneNumber, PhotoUrl) VALUES
        (N'Dr. Omar Hassan', N'Cardiology', N'+20 100 123 4567', N'/images/doctors.jpg'),
        (N'Dr. Layla Ahmed', N'Dermatology', N'+20 101 234 5678', N'/images/doctors.jpg'),
        (N'Dr. Daniel Mensah', N'Neurology', N'+20 102 345 6789', N'/images/doctors.jpg'),
        (N'Dr. Priya Shah', N'Pediatrics', N'+20 103 456 7890', N'/images/doctors.jpg');
END;
GO
-- Add a rolling week of example times only when the next seven days have no slots.
IF NOT EXISTS (SELECT 1 FROM dbo.Appointments WHERE StartsAt >= GETDATE() AND StartsAt < DATEADD(day, 8, GETDATE()))
BEGIN
    DECLARE @Day int = 1;
    WHILE @Day <= 7
    BEGIN
        INSERT INTO dbo.Appointments (DoctorId, StartsAt, EndsAt, IsBooked)
        SELECT d.Id,
               DATEADD(hour, t.HourOfDay, CAST(DATEADD(day, @Day, CAST(GETDATE() AS date)) AS datetime2)),
               DATEADD(minute, 30, DATEADD(hour, t.HourOfDay, CAST(DATEADD(day, @Day, CAST(GETDATE() AS date)) AS datetime2))),
               0
        FROM dbo.Doctors d
        CROSS JOIN (VALUES (9), (11), (14)) AS t(HourOfDay)
        WHERE NOT EXISTS (
            SELECT 1 FROM dbo.Appointments a
            WHERE a.DoctorId = d.Id
              AND a.StartsAt = DATEADD(hour, t.HourOfDay, CAST(DATEADD(day, @Day, CAST(GETDATE() AS date)) AS datetime2))
        );
        SET @Day += 1;
    END;
END;
GO
SELECT d.Name, a.StartsAt, a.EndsAt, a.IsBooked
FROM dbo.Appointments a JOIN dbo.Doctors d ON d.Id = a.DoctorId
ORDER BY a.StartsAt, d.Name;
