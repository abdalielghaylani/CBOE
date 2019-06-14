using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace PerkinElmer.COE.Inventory.API.Code
{
    /// <summary>
    /// Security Helper
    /// </summary>
    public static class SecurityHelper
    {
        private const int SaltByteSize = 24;
        private const int HashByteSize = 20; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
        private const int Pbkdf2Iterations = 1000;
        private const int IterationIndex = 0;
        private const int SaltIndex = 1;
        private const int Pbkdf2Index = 2;

        /// <summary>
        /// Get an api key string based on the secret phrase
        /// </summary>
        /// <param name="secret">Known secret phrase</param>
        /// <returns>String that represent a encrypted token</returns>
        public static string GenerateKey(string secret)
        {
            var cryptoProvider = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SaltByteSize];
            cryptoProvider.GetBytes(salt);

            var hash = GetPbkdf2Bytes(secret, salt, Pbkdf2Iterations, HashByteSize);
            return Pbkdf2Iterations + ":" +
               Convert.ToBase64String(salt) + ":" +
               Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verify if the token is valid based on the secret phrase
        /// </summary>
        /// <param name="secret">Known secret phrase</param>
        /// <param name="token">Encrypted token to verify</param>
        /// <returns>True if it is a valid token</returns>
        public static bool IsValidToken(string secret, string token)
        {
            try
            {
                char[] delimiter = { ':' };
                var split = token.Split(delimiter);
                var iterations = Int32.Parse(split[IterationIndex]);
                var salt = Convert.FromBase64String(split[SaltIndex]);
                var hash = Convert.FromBase64String(split[Pbkdf2Index]);

                var testHash = GetPbkdf2Bytes(secret, salt, iterations, hash.Length);
                return SlowEquals(hash, testHash);
            }
            catch
            {
                return false;
            }
        }

        private static byte[] GetPbkdf2Bytes(string secret, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(secret, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }
    }
}