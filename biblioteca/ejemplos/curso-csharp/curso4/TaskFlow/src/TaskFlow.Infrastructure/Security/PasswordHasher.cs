using System.Security.Cryptography;

namespace TaskFlow.Infrastructure.Security;

/// <summary>
/// Hash de contraseñas con PBKDF2. Cada contraseña lleva su propia sal aleatoria, por eso dos
/// usuarios con la misma contraseña tienen hashes distintos. Nunca se "descifra": se recalcula y compara.
/// </summary>
public static class PasswordHasher
{
    private const int Iterations = 210_000;   // recomendación OWASP para PBKDF2-HMAC-SHA512 (sube con los años)
    private const int SaltSize = 16, HashSize = 32;

    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashSize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string stored)
    {
        string[] parts = stored.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations)) return false;
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] expected = Convert.FromBase64String(parts[2]);
        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA512, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);   // tiempo constante: no filtra información
    }
}
