using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace pwned_shop.Utils
{
    public class PasswordHasher
    {
        private const int SALT_BYTE_SIZE = 128 / 8; // 128 bits / 8 bits per byte = 16 byte-long array
        private const int KEY_LENGTH = 256 / 8;
        private const int ITER_COUNT = 10000;
        public static string[] CreateHash(string password)
        {
            // generate a 128-bit salt using a secure PRNG (pseudo-random number generator)
            byte[] salt = new byte[SALT_BYTE_SIZE];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); //fills an array of bytes with a cryptographically strong random sequence of values
            }

            // derive 256-bit long hash using SHA256, 10,000 iterations
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: ITER_COUNT,
                numBytesRequested: KEY_LENGTH));

            return new string[] { hashed, Convert.ToBase64String(salt) };
        }

        public static string Hash(string password, string salt)
        {
            byte[] _salt = Convert.FromBase64String(salt);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: _salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: ITER_COUNT,
                numBytesRequested: KEY_LENGTH));

            return hashed;
        }
    }
}
