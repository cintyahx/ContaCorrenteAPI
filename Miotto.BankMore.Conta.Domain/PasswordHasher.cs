using Miotto.BankMore.Conta.Domain.Interfaces;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;

namespace Miotto.BankMore.Conta.Domain
{
    public static class PasswordHasher
    {
        public static (string hash, string salt) CreatePasswordHash(string password)
        {
            using var hmac = new HMACSHA256();
            var saltBytes = hmac.Key; // usa chave como salt
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);

            using var hmac = new HMACSHA256(saltBytes);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var computedHashBase64 = Convert.ToBase64String(computedHash);

            return computedHashBase64 == storedHash;
        }
    }
}
