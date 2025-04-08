using HumanResourcesApp.DBClasses;
using System;
using System.Text;

namespace HumanResourcesApp.Classes
{
    public static class Login
    {
        public static string HashPassword(string password) // Hashes the password
        {
            byte[] data = Encoding.ASCII.GetBytes(password);
            data = System.Security.Cryptography.SHA256.HashData(data);
            return Encoding.ASCII.GetString(data);
        }

        public static bool VerifyPassword(string password, string hashedPassword) // Verifies the password against the hashed password
        {
            string hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }

        public static bool PasswordIsValid(string password) // Validates the password based on criteria
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }
            if (password.Length < 8)
            {
                return false;
            }
            bool hasUpperCase = false;
            bool hasLowerCase = false;
            bool hasDigit = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c))
                {
                    hasUpperCase = true;
                }
                else if (char.IsLower(c))
                {
                    hasLowerCase = true;
                }
                else if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
            }
            return hasUpperCase && hasLowerCase && hasDigit;
        }

        public static bool IsLoginAvailable(string username) // Checks if the username is available
        {
            HumanResourcesDB db = new HumanResourcesDB();
            if(db.GetAllUsers().Any(u => u.Username == username))
            {
                return false; // Username is already taken
            }
            else
            {
                return true; // Username is available
            }
        }
    }
}
