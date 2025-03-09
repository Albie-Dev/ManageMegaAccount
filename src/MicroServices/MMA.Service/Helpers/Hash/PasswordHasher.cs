using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace MMA.Service
{
    partial class PasswordHasher
    {
        private const int SaltLength = 16;
        private const int HashLength = 32;
        private const int IterationCount = 3;

        public static string HashPassword(string password)
        {
            byte[] salt = GenerateSalt();
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            using (var argon2 = new Argon2id(passwordBytes))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 8;
                argon2.MemorySize = 1024 * 1024;
                argon2.Iterations = IterationCount;

                byte[] hash = argon2.GetBytes(HashLength);

                byte[] hashBytes = new byte[SaltLength + HashLength];
                Array.Copy(salt, 0, hashBytes, 0, SaltLength);
                Array.Copy(hash, 0, hashBytes, SaltLength, HashLength);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            byte[] salt = new byte[SaltLength];
            Array.Copy(hashBytes, 0, salt, 0, SaltLength);

            byte[] storedHashBytes = new byte[HashLength];
            Array.Copy(hashBytes, SaltLength, storedHashBytes, 0, HashLength);

            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            using (var argon2 = new Argon2id(passwordBytes))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 8;
                argon2.MemorySize = 1024 * 1024;
                argon2.Iterations = IterationCount;

                byte[] hash = argon2.GetBytes(HashLength);

                for (int i = 0; i < HashLength; i++)
                {
                    if (hash[i] != storedHashBytes[i])
                        return false;
                }
            }

            return true;
        }

        private static byte[] GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[SaltLength];
                rng.GetBytes(salt);
                return salt;
            }
        }
    }
}