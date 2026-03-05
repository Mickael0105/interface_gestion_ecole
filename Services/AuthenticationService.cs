using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyApp.Interface.Services
{
    public enum UserRole
    {
        Admin,
        Teacher,
        Student
    }

    public class UserSession
    {
        public string Username { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }

    // Authentification en mémoire (pas de base de données).
    public static class AuthenticationService
    {
        // Dictionary mapping username -> (Hash, Role)
        private static readonly Dictionary<string, (string Hash, UserRole Role)> _users = new(StringComparer.OrdinalIgnoreCase);

        static AuthenticationService()
        {
            // Admin: admin / admin123
            _users["admin"] = (ComputeHash("admin123"), UserRole.Admin);
            
            // Teacher: enseignant / enseignant123
            _users["enseignant"] = (ComputeHash("enseignant123"), UserRole.Teacher);

            // Student: etudiant / etudiant123
            _users["etudiant"] = (ComputeHash("etudiant123"), UserRole.Student);
        }

        public static UserSession? Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            if (!_users.TryGetValue(username.Trim(), out var userInfo))
                return null;

            var incomingHash = ComputeHash(password);
            if (string.Equals(userInfo.Hash, incomingHash, StringComparison.Ordinal))
            {
                return new UserSession { Username = username, Role = userInfo.Role };
            }

            return null;
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}