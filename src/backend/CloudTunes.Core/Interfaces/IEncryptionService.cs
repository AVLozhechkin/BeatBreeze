namespace CloudTunes.Core.Interfaces;

public interface IEncryptionService
{
    public byte[] Encrypt(string valueToEncrypt);

    public string Decrypt(byte[] encryptedValue);
}
