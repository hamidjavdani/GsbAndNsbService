using System.Security.Cryptography;
using System.Text;
namespace GSB.Test.Api.Services;
public class EncryptionService
{
    public static string Encrypt(string combinedString, string keyString, string ivString)
    {
        if (string.IsNullOrEmpty(keyString))
        {
            throw new ArgumentException("Key string cannot be null or empty", nameof(keyString));
        }

        if (string.IsNullOrEmpty(ivString))
        {
            throw new ArgumentException("IV string cannot be null or empty", nameof(ivString));
        }

        byte[] PlainBytes = Encoding.UTF8.GetBytes(combinedString);

        var crypt = new SHA256Managed();

        byte[] key = crypt.ComputeHash(Encoding.UTF8.GetBytes(keyString));
        byte[] iv = Encoding.UTF8.GetBytes(ivString);
        byte[] encrypted;
        // Create an RijndaelManaged object
        // with the specified key and IV.
        using (RijndaelManaged rijAlg = new())
        {
            rijAlg.Key = key;
            rijAlg.IV = iv;

            // Create a decrytor to perform the stream transform.
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new())
            {
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(PlainBytes, 0, PlainBytes.Length);
                    csEncrypt.FlushFinalBlock();
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string data, string keyString, string ivString)
    {
        if (string.IsNullOrEmpty(keyString))
        {
            throw new ArgumentException("Key string cannot be null or empty", nameof(keyString));
        }

        if (string.IsNullOrEmpty(ivString))
        {
            throw new ArgumentException("IV string cannot be null or empty", nameof(ivString));
        }

        var crypt = new SHA256Managed();

        byte[] key = crypt.ComputeHash(Encoding.UTF8.GetBytes(keyString));
        byte[] iv = Encoding.UTF8.GetBytes(ivString);

        using (var rijndaelManaged =
                new RijndaelManaged { Key = key, IV = iv, Mode = CipherMode.CBC })
        {
            rijndaelManaged.BlockSize = 128;
            rijndaelManaged.KeySize = 256;
            using (var memoryStream =
                   new MemoryStream(Convert.FromBase64String(data)))
            using (var cryptoStream =
                   new CryptoStream(memoryStream,
                       rijndaelManaged.CreateDecryptor(key, iv),
                       CryptoStreamMode.Read))
            {
                return new StreamReader(cryptoStream).ReadToEnd();
            }
        }
    }


}



