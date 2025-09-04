using System;
using System.Security.Cryptography;
using System.Text;

namespace SuvatGatewayBackend.Helpers;

public class OneMoneyCryptoHelper
{
    public static string GenerateAESKey()
    {
        using var rng = new RNGCryptoServiceProvider();
        byte[] key = new byte[16];
        rng.GetBytes(key);
        return Convert.ToBase64String(key);
    }

    public static string EncryptWithAES(string plainText, string base64Key)
    {
        byte[] key = Convert.FromBase64String(base64Key);
        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        byte[] input = Encoding.UTF8.GetBytes(plainText);
        byte[] encrypted = encryptor.TransformFinalBlock(input, 0, input.Length);
        return Convert.ToBase64String(encrypted);
    }

    public static string EncryptWithRSA(string aesKey, string base64PublicKey)
    {
        byte[] keyBytes = Convert.FromBase64String(base64PublicKey);
        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
        var encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(aesKey), RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(encrypted);
    }

    public static string ComputeSha256Hash(string rawData)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

}
