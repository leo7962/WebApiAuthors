using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WebApiAuthors.Dtos;

namespace WebApiAuthors.Services;

public class HashService
{
    public HashResult Hash(string plainText)
    {
        var sal = new byte[16];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(sal);
        }

        return Hash(plainText, sal);
    }

    public HashResult Hash(string plaintext, byte[] sal)
    {
        var keyDerivade = KeyDerivation.Pbkdf2(password: plaintext, salt: sal, prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000, numBytesRequested: 32);

        var hash = Convert.ToBase64String(keyDerivade);

        return new HashResult()
        {
            Hash = hash,
            Sal = sal
        };
    }
}
