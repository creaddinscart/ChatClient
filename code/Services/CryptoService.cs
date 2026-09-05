using System.Security.Cryptography;
using System.Text;

namespace ChatClient.Services;

public static class CryptoService
{
    public static (string Cipher, string Nonce, string Tag) Encrypt(string text, string key)
    {
        var bytes = RandomNumberGenerator.GetBytes(12); var tag = new byte[16]; var data = Encoding.UTF8.GetBytes(text); var cipher = new byte[data.Length];
        using var aes = new AesGcm(Key(key), 16); aes.Encrypt(bytes, data, cipher, tag);
        return (Convert.ToBase64String(cipher), Convert.ToBase64String(bytes), Convert.ToBase64String(tag));
    }
    public static string Decrypt(string cipher, string key, string nonce, string tag)
    {
        var data = Convert.FromBase64String(cipher); var plain = new byte[data.Length];
        using var aes = new AesGcm(Key(key), 16); aes.Decrypt(Convert.FromBase64String(nonce), data, Convert.FromBase64String(tag), plain);
        return Encoding.UTF8.GetString(plain);
    }
    private static byte[] Key(string key) => SHA256.HashData(Encoding.UTF8.GetBytes(key));
}
