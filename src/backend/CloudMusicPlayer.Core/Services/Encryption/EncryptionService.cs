using System.Security.Cryptography;
using System.Text;
using CloudMusicPlayer.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace CloudMusicPlayer.Core.Services.Encryption;

internal class EncryptionService : IEncryptionService
{
    private readonly string _key;

    public EncryptionService(IOptions<EncryptionOptions> options)
    {
        _key = options.Value.Secret;
    }

    public byte[] Encrypt(string token)
    {
        using var aes = Aes.Create();

        aes.Key = Encoding.UTF8.GetBytes(_key);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();

        using MemoryStream memoryStream = new MemoryStream();

        memoryStream.Write(aes.IV);

        using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(token);
        }

        return memoryStream.ToArray();
    }

    public string Decrypt(byte[] encryptedToken)
    {
        using var aes = Aes.Create();

        aes.Key = Encoding.UTF8.GetBytes(_key);
        aes.IV = encryptedToken.AsSpan(0, 16).ToArray();

        using var decryptor = aes.CreateDecryptor();

        using MemoryStream memoryStream = new MemoryStream(encryptedToken.AsSpan(16).ToArray());
        using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }
}
