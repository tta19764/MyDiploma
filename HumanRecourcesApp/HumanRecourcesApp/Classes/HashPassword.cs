using System;
using System.Text;

namespace HumanRecourcesApp.Classes
{
    public static class HashPassword
    {
        public static string Hash(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = System.Security.Cryptography.SHA256.HashData(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }
    }
}
