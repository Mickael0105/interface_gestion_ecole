using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using MyApp.Interface.Models;

namespace MyApp.Interface.Services
{
    public class AppData
    {
        public List<Student> Students { get; set; } = new();
        public List<Teacher> Teachers { get; set; } = new();
        public List<SchoolClass> Classes { get; set; } = new();
        public List<Subject> Subjects { get; set; } = new();
        public List<Grade> Grades { get; set; } = new();
    }

    public static class JsonDataService
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "MyAppDashboard", 
            "data.json");

        public static void Save(AppData data)
        {
            try
            {
                var dir = Path.GetDirectoryName(FilePath);
                if (dir != null && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        public static AppData Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var json = File.ReadAllText(FilePath);
                    return JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
            return new AppData(); // Return empty if no file or error
        }
    }
}
