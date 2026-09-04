--CREATING THE RACEDAY DATABASE

CREATE DATABASE RaceDay;


USE RaceDay;


-- 1. CREATING THE RELEVANT TABLES

CREATE TABLE Users
(
    UserID INT IDENTITY(1,1) PRIMARY KEY,

    FirstName NVARCHAR(50) NOT NULL,

    LastName NVARCHAR(50) NOT NULL,

    Email NVARCHAR(100) NOT NULL UNIQUE,

    PasswordHash NVARCHAR(255) NOT NULL,

    Role NVARCHAR(20) NOT NULL
        CONSTRAINT CK_Users_Role
        CHECK (Role IN ('Organiser', 'Participant')),

    PhoneNumber NVARCHAR(20) NULL,

    CreatedAt DATETIME2 NOT NULL
        DEFAULT GETDATE()
);


CREATE TABLE Events
(
    EventID INT IDENTITY(1,1) PRIMARY KEY,

    OrganiserID INT NOT NULL,

    EventName NVARCHAR(100) NOT NULL,

    Description NVARCHAR(500) NULL,

    Location NVARCHAR(150) NOT NULL,

    EventDate DATE NOT NULL,

    RegistrationDeadline DATE NOT NULL,

    Status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Events_Status
        DEFAULT 'Upcoming',

    CreatedAt DATETIME2 NOT NULL
        DEFAULT GETDATE(),

    CONSTRAINT CK_Events_Status
        CHECK (Status IN ('Upcoming', 'Open', 'Closed', 'Completed', 'Cancelled')),

    CONSTRAINT FK_Events_Organiser
        FOREIGN KEY (OrganiserID)
        REFERENCES Users(UserID)
);



CREATE TABLE Categories
(
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,

    EventID INT NOT NULL,

    CategoryName NVARCHAR(100) NOT NULL,

    DistanceKm DECIMAL(6,2) NOT NULL,

    EntryFee DECIMAL(10,2) NOT NULL,

    MaximumParticipants INT NOT NULL,

    CONSTRAINT FK_Categories_Event
        FOREIGN KEY (EventID)
        REFERENCES Events(EventID),

    CONSTRAINT CK_Categories_Distance
        CHECK (DistanceKm > 0),

    CONSTRAINT CK_Categories_EntryFee
        CHECK (EntryFee >= 0),

    CONSTRAINT CK_Categories_MaxParticipants
        CHECK (MaximumParticipants > 0),

    CONSTRAINT UQ_Category_Event_Name
        UNIQUE (EventID, CategoryName)
);
 

CREATE TABLE Enrolments
(
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,

    ParticipantID INT NOT NULL,

    CategoryID INT NOT NULL,

    EnrolmentDate DATETIME2 NOT NULL
        DEFAULT GETDATE(),

    EmergencyContact NVARCHAR(100) NOT NULL,

    Status NVARCHAR(20) NOT NULL
        DEFAULT 'Active',

    CONSTRAINT FK_Enrolments_Participant
        FOREIGN KEY (ParticipantID)
        REFERENCES Users(UserID),

    CONSTRAINT FK_Enrolments_Category
        FOREIGN KEY (CategoryID)
        REFERENCES Categories(CategoryID),

    CONSTRAINT CK_Enrolments_Status
        CHECK (Status IN ('Active', 'Cancelled', 'Completed')),

    CONSTRAINT UQ_Participant_Category
        UNIQUE (ParticipantID, CategoryID)
);




CREATE TABLE Results
(
    ResultID INT IDENTITY(1,1) PRIMARY KEY,

    EnrolmentID INT NOT NULL UNIQUE,

    FinishTime TIME NULL,

    Position INT NULL,

    ResultStatus NVARCHAR(20) NOT NULL
        DEFAULT 'Finished',

    RecordedAt DATETIME2 NOT NULL
        DEFAULT GETDATE(),

    CONSTRAINT FK_Results_Enrolment
        FOREIGN KEY (EnrolmentID)
        REFERENCES Enrolments(EnrolmentID),

    CONSTRAINT CK_Results_Position
        CHECK (Position IS NULL OR Position > 0),

    CONSTRAINT CK_Results_Status
        CHECK (ResultStatus IN ('Finished', 'DNF', 'DNS', 'Disqualified'))
);



CREATE TABLE Weather
(
    WeatherID INT IDENTITY(1,1) PRIMARY KEY,

    EventID INT NOT NULL,

    Temperature DECIMAL(5,2) NULL,

    WeatherCondition NVARCHAR(100) NULL,

    WindSpeed DECIMAL(5,2) NULL,

    RainProbability DECIMAL(5,2) NULL,

    RecordedAt DATETIME2 NOT NULL
        DEFAULT GETDATE(),

    CONSTRAINT FK_Weather_Event
        FOREIGN KEY (EventID)
        REFERENCES Events(EventID),

    CONSTRAINT CK_Weather_RainProbability
        CHECK (
            RainProbability IS NULL
            OR RainProbability BETWEEN 0 AND 100
        ),

    CONSTRAINT CK_Weather_WindSpeed
        CHECK (
            WindSpeed IS NULL
            OR WindSpeed >= 0
        )
);
GO


--INSERTING THE VALUES

INSERT INTO Users ( FirstName,LastName,Email,PasswordHash,Role,PhoneNumber)
VALUES
(
    'Thabo',
    'Mokoena',
    'thabo.mokoena@raceday.co.za',
    'HASHED_PASSWORD_1',
    'Organiser',
    '0821112233'
),
(
    'Lerato',
    'Dlamini',
    'lerato.dlamini@raceday.co.za',
    'HASHED_PASSWORD_2',
    'Organiser',
    '0832223344'
),
(
    'Sipho',
    'Nkosi',
    'sipho.nkosi@email.com',
    'HASHED_PASSWORD_3',
    'Participant',
    '0843334455'
),
(
    'Anele',
    'Khumalo',
    'anele.khumalo@email.com',
    'HASHED_PASSWORD_4',
    'Participant',
    '0854445566'
);




INSERT INTO Events (OrganiserID,EventName,Description,Location,EventDate,RegistrationDeadline,Status)
VALUES
(
    1,
    'Johannesburg City Run',
    'A road running event through Johannesburg.',
    'Johannesburg',
    '2026-10-18',
    '2026-10-10',
    'Open'
),
(
    2,
    'Cape Town Coastal Marathon',
    'A coastal running event for recreational and competitive runners.',
    'Cape Town',
    '2026-11-08',
    '2026-10-31',
    'Upcoming'
),
(
    1,
    'Pretoria Cycle Challenge',
    'A cycling event featuring multiple distance categories.',
    'Pretoria',
    '2026-11-22',
    '2026-11-14',
    'Upcoming'
);




INSERT INTO Categories (EventID,CategoryName,DistanceKm,EntryFee,MaximumParticipants)
VALUES
-- Johannesburg City Run
(
    1,
    '5 KM Fun Run',
    5.00,
    100.00,
    500
),
(
    1,
    '10 KM Road Race',
    10.00,
    180.00,
    750
),
(
    1,
    '21 KM Half Marathon',
    21.10,
    300.00,
    1000
),

-- Cape Town Coastal Marathon
(
    2,
    '10 KM Coastal Run',
    10.00,
    200.00,
    750
),
(
    2,
    '21 KM Half Marathon',
    21.10,
    350.00,
    1000
),
(
    2,
    '42.2 KM Marathon',
    42.20,
    500.00,
    1500
),

-- Pretoria Cycle Challenge
(
    3,
    '30 KM Cycle',
    30.00,
    250.00,
    500
),
(
    3,
    '60 KM Cycle',
    60.00,
    400.00,
    750
),
(
    3,
    '100 KM Cycle',
    100.00,
    550.00,
    1000
);





INSERT INTO Enrolments (ParticipantID,CategoryID,EmergencyContact,Status)
VALUES
(
    3,
    1,
    'Nomsa Nkosi - 0815556677',
    'Active'
),
(
    3,
    5,
    'Nomsa Nkosi - 0815556677',
    'Active'
),
(
    4,
    2,
    'Sibusiso Khumalo - 0826667788',
    'Active'
),
(
    4,
    7,
    'Sibusiso Khumalo - 0826667788',
    'Active'
);





INSERT INTO Results (EnrolmentID,FinishTime,Position,ResultStatus)
VALUES
(
    1,
    '00:32:45',
    37,
    'Finished'
);




INSERT INTO Weather (EventID,Temperature,WeatherCondition,WindSpeed,RainProbability)
VALUES
(
    1,
    22.50,
    'Partly Cloudy',
    14.20,
    20.00
),
(
    2,
    19.80,
    'Clear',
    11.50,
    10.00
),
(
    3,
    24.10,
    'Sunny',
    18.30,
    5.00
);


SELECT * FROM Users;

     SELECT * FROM Events;

SELECT * FROM Categories;

     SELECT * FROM Enrolments;

SELECT * FROM Results;

      SELECT * FROM Weather;