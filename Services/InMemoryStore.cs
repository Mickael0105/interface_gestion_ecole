using System;
using System.Collections.ObjectModel;
using System.Linq;
using MyApp.Interface.Models;

namespace MyApp.Interface.Services
{
    // Simple in-memory store pour toutes les entités (exemple pédagogique).
    public static class InMemoryStore
    {
        public static ObservableCollection<Student> Students { get; private set; } = new();
        public static ObservableCollection<Teacher> Teachers { get; private set; } = new();
        public static ObservableCollection<SchoolClass> Classes { get; private set; } = new();
        public static ObservableCollection<Subject> Subjects { get; private set; } = new();
        public static ObservableCollection<Grade> Grades { get; private set; } = new();

        static InMemoryStore()
        {
            Load();
            
            // If empty (first run), generate mock data
            if (!Students.Any() && !Teachers.Any())
            {
                GenerateMockData();
                Save();
            }
        }

        public static void Save()
        {
            var data = new AppData
            {
                Students = Students.ToList(),
                Teachers = Teachers.ToList(),
                Classes = Classes.ToList(),
                Subjects = Subjects.ToList(),
                Grades = Grades.ToList()
            };
            JsonDataService.Save(data);
        }

        public static void Load()
        {
            var data = JsonDataService.Load();
            Students = new ObservableCollection<Student>(data.Students ?? new System.Collections.Generic.List<Student>());
            Teachers = new ObservableCollection<Teacher>(data.Teachers ?? new System.Collections.Generic.List<Teacher>());
            Classes = new ObservableCollection<SchoolClass>(data.Classes ?? new System.Collections.Generic.List<SchoolClass>());
            Subjects = new ObservableCollection<Subject>(data.Subjects ?? new System.Collections.Generic.List<Subject>());
            Grades = new ObservableCollection<Grade>(data.Grades ?? new System.Collections.Generic.List<Grade>());
        }

        private static void GenerateMockData()
        {
             // données d'exemple
            var class1 = new SchoolClass { Name = "6ème A" };
            var class2 = new SchoolClass { Name = "5ème B" };
            Classes.Add(class1);
            Classes.Add(class2);

            Subjects.Add(new Subject { Name = "Mathématiques" });
            Subjects.Add(new Subject { Name = "Français" });
            Subjects.Add(new Subject { Name = "Histoire" });

            Teachers.Add(new Teacher { Matricule = "ENS-001", Name = "Rasoa", Specialty = "Maths", Subjects = "Mathématiques" });
            Teachers.Add(new Teacher { Matricule = "ENS-002", Name = "Jean", Specialty = "Lettres", Subjects = "Français,Histoire" });

            Students.Add(new Student
            {
                Matricule = "ETU-001",
                FirstName = "Micka",
                LastName = "Rakoto",
                DateOfBirth = DateTime.Today.AddYears(-15),
                Address = "Antananarivo",
                Contact = "0341234567",
                ClassId = class1.Id
            });

            Students.Add(new Student
            {
                Matricule = "ETU-002",
                FirstName = "Aina",
                LastName = "Rajaonary",
                DateOfBirth = DateTime.Today.AddYears(-16),
                Address = "Fianarantsoa",
                Contact = "0339876543",
                ClassId = class2.Id
            });

            // quelques notes d'exemple
            if (Students.Any() && Subjects.Any())
            {
                Grades.Add(new Grade { StudentId = Students.First().Id, SubjectId = Subjects.First().Id, Value = 14 });
                Grades.Add(new Grade { StudentId = Students.First().Id, SubjectId = Subjects.Skip(1).First().Id, Value = 12.5 });
            }
        }

        // helpers
        public static string ClassNameFor(Guid? classId)
        {
            if (classId == null) return string.Empty;
            var c = Classes.FirstOrDefault(x => x.Id == classId.Value);
            return c?.Name ?? string.Empty;
        }

        public static string SubjectName(Guid id) => Subjects.FirstOrDefault(s => s.Id == id)?.Name ?? string.Empty;
        public static string StudentName(Guid id) => Students.FirstOrDefault(s => s.Id == id)?.FullName ?? string.Empty;
    }
}