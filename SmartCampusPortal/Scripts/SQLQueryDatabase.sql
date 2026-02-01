
-- Smart Campus Portal Database Schema and Sample Data
USE master;

USE SmartCampusDB;
GO

-- Drop tables if they exist to allow recreation
IF OBJECT_ID('Announcements', 'U') IS NOT NULL DROP TABLE Announcements;
IF OBJECT_ID('Attendance', 'U') IS NOT NULL DROP TABLE Attendance;
IF OBJECT_ID('FeeRecords', 'U') IS NOT NULL DROP TABLE FeeRecords;
IF OBJECT_ID('Submissions', 'U') IS NOT NULL DROP TABLE Submissions;
IF OBJECT_ID('Assignments', 'U') IS NOT NULL DROP TABLE Assignments;
IF OBJECT_ID('CourseRegistrations', 'U') IS NOT NULL DROP TABLE CourseRegistrations;
IF OBJECT_ID('Courses', 'U') IS NOT NULL DROP TABLE Courses;
IF OBJECT_ID('Faculty', 'U') IS NOT NULL DROP TABLE Faculty;
IF OBJECT_ID('Students', 'U') IS NOT NULL DROP TABLE Students;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO
USE SmartCampusDB;

-- Users Table
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Student', 'Faculty', 'Admin'))
);

-- Students Table
CREATE TABLE Students (
    StudentID INT PRIMARY KEY,
    Department NVARCHAR(50),
    EnrollmentDate DATE,
    FOREIGN KEY (StudentID) REFERENCES Users(UserID)
);

-- Faculty Table
CREATE TABLE Faculty (
    FacultyID INT PRIMARY KEY,
    Department NVARCHAR(50),
    HireDate DATE,
    FOREIGN KEY (FacultyID) REFERENCES Users(UserID)
);

-- Courses Table
CREATE TABLE Courses (
    CourseID INT PRIMARY KEY IDENTITY(1,1),
    CourseName NVARCHAR(100),
    Department NVARCHAR(50),
    Credits INT,
    PrerequisiteCourseID INT NULL,
    FOREIGN KEY (PrerequisiteCourseID) REFERENCES Courses(CourseID)
);

-- Course Registrations Table
CREATE TABLE CourseRegistrations (
    RegistrationID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT,
    CourseID INT,
    RegistrationDate DATE,
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);

-- Assignments Table
CREATE TABLE Assignments (
    AssignmentID INT PRIMARY KEY IDENTITY(1,1),
    CourseID INT,
    Title NVARCHAR(100),
    Description NVARCHAR(MAX),
    DueDate DATE,
    UploadedBy INT,
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
    FOREIGN KEY (UploadedBy) REFERENCES Faculty(FacultyID)
);

-- Submissions Table
CREATE TABLE Submissions (
    SubmissionID INT PRIMARY KEY IDENTITY(1,1),
    AssignmentID INT,
    StudentID INT,
    SubmissionDate DATETIME NOT NULL, -- Changed to DATETIME to include time, matching C# DateTime.Now
    FileName NVARCHAR(255) NOT NULL, -- To store the original file name
    FileContent VARBINARY(MAX) NOT NULL, -- To store the binary content of the file
    ContentType NVARCHAR(100) NOT NULL, -- To store the MIME type (e.g., application/pdf)
    Comment NVARCHAR(MAX) NULL, -- Optional comments
    Grade INT NULL,
    FOREIGN KEY (AssignmentID) REFERENCES Assignments(AssignmentID),
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
);

-- Fee Records Table
CREATE TABLE FeeRecords (
    FeeID INT PRIMARY KEY IDENTITY(1,1),
    StudentID INT,
    TotalAmount DECIMAL(10,2),
    PaidAmount DECIMAL(10,2),
    PaymentStatus NVARCHAR(20),
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
);

-- Attendance Table
CREATE TABLE Attendance (
    AttendanceID INT PRIMARY KEY IDENTITY(1,1),
    CourseID INT,
    StudentID INT,
    Date DATE,
    Status NVARCHAR(10),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID)
);

-- Announcements Table
CREATE TABLE Announcements (
    AnnouncementID INT PRIMARY KEY IDENTITY(1,1),
    FacultyID INT,
    CourseID INT,
    Message NVARCHAR(MAX),
    DatePosted DATE,
    FOREIGN KEY (FacultyID) REFERENCES Faculty(FacultyID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);

-- Sample Users
INSERT INTO Users (Email, Password, FullName, Role) VALUES
('student1@campus.edu', 'pass123', 'Student One', 'Student'),
('faculty1@campus.edu', 'pass123', 'Faculty One', 'Faculty'),
('admin1@campus.edu', 'pass123', 'Admin One', 'Admin');

-- Corresponding entries in Students and Faculty
INSERT INTO Students (StudentID, Department, EnrollmentDate) VALUES (1, 'Computer Science', '2022-08-01');
INSERT INTO Faculty (FacultyID, Department, HireDate) VALUES (2, 'Computer Science', '2020-01-15');

-- Sample Courses
INSERT INTO Courses (CourseName, Department, Credits) VALUES
('Data Structures', 'Computer Science', 3),
('Algorithms', 'Computer Science', 3);

-- Sample Fee Records
INSERT INTO FeeRecords (StudentID, TotalAmount, PaidAmount, PaymentStatus) VALUES
(1, 10000, 7000, 'Partially Paid');

-- Sample Assignment
INSERT INTO Assignments (CourseID, Title, Description, DueDate, UploadedBy) VALUES
(1, 'Assignment 1', 'Complete all questions in Chapter 2', '2024-12-01', 2);
