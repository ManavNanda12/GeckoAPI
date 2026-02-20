using System;
using System.Security.Cryptography;

namespace GeckoAPI.Common
{
    public static class PasswordGenerator
    {
        private const string ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string GenerateRandomPassword(int length = 6)
        {
            if (length <= 0)
                throw new ArgumentException("Length must be greater than 0.");

            var password = new char[length];
            var randomBytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            for (int i = 0; i < length; i++)
            {
                password[i] = ValidChars[randomBytes[i] % ValidChars.Length];
            }

            return new string(password);
        }
    }
}