using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Common
{
    public class CommonHelper
    {
        // Create hash and salt, stored as Base64 strings
        public static void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                var saltBytes = hmac.Key;
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                passwordSalt = Convert.ToBase64String(saltBytes);
                passwordHash = Convert.ToBase64String(hashBytes);
            }
        }

        // Verify password
        public static bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var hashBytes = Convert.FromBase64String(storedHash);

            using (var hmac = new HMACSHA512(saltBytes))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(hashBytes);
            }
        }

        // Image Validator
        public static bool IsValidImageFile(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // Check extension
            if (!allowedExtensions.Contains(fileExtension))
                return false;

            // Check file size (e.g., max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return false;

            return true;
        }

    }
}
