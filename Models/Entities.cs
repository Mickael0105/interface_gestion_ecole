using System;

namespace MyApp.Interface.Models
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Matricule { get; set; } = string.Empty; // [NEW] Identifiant unique
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-16);
        public string Address { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string ParentsNames { get; set; } = string.Empty; // [NEW] Noms des parents
        public string PhotoPath { get; set; } = string.Empty;    // [NEW] Chemin de la photo
        public Guid? ClassId { get; set; }

        public string FullName => $"{Matricule} - {LastName} {FirstName}";
    }

    public class Teacher
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Matricule { get; set; } = string.Empty; // [NEW] Identifiant unique
        public string Name { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string Subjects { get; set; } = string.Empty; // comma separated for UI simplicity
        public string PhotoPath { get; set; } = string.Empty; // [NEW] Chemin de la photo
    }

    public class SchoolClass
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string AssignedSubjects { get; set; } = string.Empty; // comma separated
    }

    public class Subject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }

    public class Grade
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StudentId { get; set; }
        public Guid SubjectId { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
    }
}