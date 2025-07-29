using System;
using System.Security.Cryptography;
using System.Text;

namespace SuvatGatewayBackend.Helpers;

public static class CryptoUtils
{
    public static (string EncryptedData, string AesKeyBase64) EncryptWithAes(string plainJson)
    {
        using var aes = Aes.Create();
        aes.KeySize = 128;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateKey();

        var key = aes.Key;
        var encryptor = aes.CreateEncryptor();

        var inputBytes = Encoding.UTF8.GetBytes(plainJson);
        var encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

        return (Convert.ToBase64String(encrypted), Convert.ToBase64String(key));
    }

    public static string EncryptAesKeyWithRsa(string aesKeyBase64, string publicKeyPem)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem.ToCharArray());

        var encryptedKey = rsa.Encrypt(Convert.FromBase64String(aesKeyBase64), RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(encryptedKey);
    }

    public static string Sha256HexDigest(string input)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

}
