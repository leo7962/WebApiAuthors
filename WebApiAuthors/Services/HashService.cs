using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
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

    private static HashResult Hash(string plaintext, byte[] sal)
    {
        var keyDerivade = KeyDerivation.Pbkdf2(plaintext, sal, KeyDerivationPrf.HMACSHA512,
            10000, 32);

        var hash = Convert.ToBase64String(keyDerivade);

        return new HashResult
        {
            Hash = hash,
            Sal = sal
        };
    }
}