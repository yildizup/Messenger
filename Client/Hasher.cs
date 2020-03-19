using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class Hasher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="algo">Hash Algorithmus (andere Hash Algorithmen können implementiert werden)</param>
        /// <param name="password">der zu "hashende" string</param>
        /// <returns></returns>
        private static string GenerateHashString(HashAlgorithm algo, string password)
        {
            algo.ComputeHash(Encoding.UTF8.GetBytes(password));

            // berechnetes Hash speichern
            var result = algo.Hash;

            // Hash zurückgeben
            return string.Join(
                string.Empty,
                result.Select(x => x.ToString("x2")));
        }

        public static string SHA1(string password)
        {
            var result = default(string);

            using (var algo = new SHA1Managed())
            {
                result = GenerateHashString(algo, password);
            }

            return result;
        }

        /// <summary>
        /// vergleicht, ob das eingegebene Passwort mit dem gehashten Passwort übereinstimmt
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public static bool Verify(string password, string hashedPassword)
        {
            if (SHA1(password) == hashedPassword)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

    }
}
