
/*
-- Movie Management System
21127367 - Đỗ Thế Nghĩa
21127384 - Dương Hạnh Nhi
21127461 - Lê Thành Trung
*/

CREATE DATABASE DB_MovieManagement_1
GO

USE DB_MovieManagement_1
GO

-- Tao cac bang du lieu
CREATE TABLE Account (
    Username NVARCHAR(30),
    Password NVARCHAR(30),
    DOB DATETIME,
    Gender NVARCHAR(8), -- Male || Female
    Fullname NVARCHAR(30),
    IsAdmin BIT,
    AccountId INT,
    PRIMARY KEY (AccountId)
)

CREATE TABLE Bill (
    Total FLOAT,
    AccountId INT,
    BookingTime DATETIME,
    BillId INT,
    PRIMARY KEY (BillId)
)

CREATE TABLE BillVoucher (
    BillId INT,
    VoucherId INT,
    AppliedTime DATETIME,
    PRIMARY KEY (BillId, VoucherId)
)

CREATE TABLE Voucher (
    VoucherCode NVARCHAR(30),
    DiscountAmount FLOAT,
    IsExpired BIT,
    IsPercentageDiscount BIT,
    RequirementAmount FLOAT,
    VoucherId INT,
    PRIMARY KEY (VoucherId)
)

CREATE TABLE Ticket (
    TicketId INT,
    IsAvailable BIT,
    Row NVARCHAR(5),
    Col INT,
    Price FLOAT,
    BillId INT,
    ShowTimeId INT,
    PRIMARY KEY (TicketId)
)

CREATE TABLE ShowTime (
    ShowTimeId INT,
    MovieId INT,
    ShowDate DATETIME,
    MaxRow INT,
    MaxCol INT,
    PRIMARY KEY (ShowTimeId)
)

CREATE TABLE Movie (
    Title NVARCHAR(30),
    Duration INT, -- Tính theo phút
    PublishYear INT,
    IMDbScore FLOAT,
    AgeCertificateId INT,
    DirectorId INT,
    MovieId INT,
    IsGoldenHour BIT,
    IsBlockbuster BIT,
    PosterUrl NVARCHAR(100),
    TrailerUrl NVARCHAR(100),
    Description NVARCHAR(1000),
    GenreId INT,
    PRIMARY KEY (MovieId)
)

CREATE TABLE Person (
    Fullname NVARCHAR(30),
    AvatarUrl NVARCHAR(100),
    Biography NVARCHAR(1000),
    PersonId INT,
    PRIMARY KEY (PersonId)
)

CREATE TABLE MovieActor (
    MovieId INT,
    PersonId INT,
    PRIMARY KEY (MovieId, PersonId)
)

CREATE TABLE Genre (
    GenreName NVARCHAR(30),
    GenreId INT,
    PRIMARY KEY (GenreId)
)

CREATE TABLE AgeCertificate (
    DisplayContent NVARCHAR(30),
    RequireAge INT,
    AgeCertificateId INT,
    BackgroundColor NVARCHAR(30),
    ForegroundColor NVARCHAR(30),
    PRIMARY KEY (AgeCertificateId)
)


-- Rang buoc du lieu
GO
ALTER TABLE Bill ADD
    CONSTRAINT FK_Bill_Account FOREIGN KEY (AccountId) REFERENCES Account(AccountId)

GO
ALTER TABLE BillVoucher ADD
    CONSTRAINT FK_BillVoucher_Bill FOREIGN KEY (BillId) REFERENCES Bill(BillId),
    CONSTRAINT FK_BillVoucher_Voucher FOREIGN KEY (VoucherId) REFERENCES Voucher(VoucherId)

GO
ALTER TABLE Ticket ADD
    CONSTRAINT FK_Ticket_Bill FOREIGN KEY (BillId) REFERENCES Bill(BillId),
    CONSTRAINT FK_Ticket_ShowTime FOREIGN KEY (ShowTimeId) REFERENCES ShowTime(ShowTimeId)

GO
ALTER TABLE ShowTime ADD
    CONSTRAINT FK_ShowTime_Movie FOREIGN KEY (MovieId) REFERENCES Movie(MovieId)

GO
ALTER TABLE Movie ADD
    CONSTRAINT FK_Movie_Person FOREIGN KEY (DirectorId) REFERENCES Person(PersonId),
    CONSTRAINT FK_Movie_Genre FOREIGN KEY (GenreId) REFERENCES Genre(GenreId),
    CONSTRAINT FK_AgeCertificate FOREIGN KEY (AgeCertificateId) REFERENCES AgeCertificate(AgeCertificateId)

GO
ALTER TABLE MovieActor ADD
    CONSTRAINT FK_MovieActor_Movie FOREIGN KEY (MovieId) REFERENCES Movie(MovieId),
    CONSTRAINT FK_MovieActor_Person FOREIGN KEY (PersonId) REFERENCES Person(PersonId)


-- Nhap lieu cho cac bang
-- USE master;
-- GO
-- DROP DATABASE DB_MovieManagement_1;
-- GO