using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LTA.Mobile.Application.Interfaces;

namespace LTA.Mobile.Application.Services;

public class CryptoService : IDisposable, ICryptoService
{
    public CryptoService()
    {
        _aes = Aes.Create();
        _diffieHellman = new ECDiffieHellmanCng
        {
            KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
            HashAlgorithm = CngAlgorithm.Sha256
        };

        _publicKey = _diffieHellman.PublicKey.ToByteArray();
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public byte[] PublicKey => _publicKey;
    public byte[] IV => _aes.IV;
    public async Task<byte[]> EncryptAsync(byte[] publicKey, string message)
    {
        byte[] encrpytedMessage;
        var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
        var derivedKey = _diffieHellman.DeriveKeyMaterial(key);

        _aes.Key = derivedKey;

        using (var cipherText = new MemoryStream())
        {
            using (var encryptor = _aes.CreateEncryptor())
            {
                using (var cryptoStream = new CryptoStream(cipherText, encryptor, CryptoStreamMode.Write))
                {
                    var cipherTextMessage = Encoding.UTF8.GetBytes(message);
                    await cryptoStream.WriteAsync(cipherTextMessage, 0, cipherTextMessage.Length);
                }
            }

            encrpytedMessage = cipherText.ToArray();
        }

        return encrpytedMessage;
    }

    public async Task<byte[]> EncryptAsync(byte[] publicKey, byte[] message)
    {
        throw new NotImplementedException();
    }

    public async Task<string> DecryptTextAsync(byte[] publicKey, byte[] encryptedMessage, byte[] iv)
    {
        string decryptedMessage;
        var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
        var derivedKey = _diffieHellman.DeriveKeyMaterial(key);

        _aes.Key = derivedKey;
        _aes.IV = iv;

        using (var plainText = new MemoryStream())
        {
            using (var decryptor = _aes.CreateDecryptor())
            {
                using (var cryptoStream = new CryptoStream(plainText, decryptor, CryptoStreamMode.Write))
                {
                    await cryptoStream.WriteAsync(encryptedMessage, 0, encryptedMessage.Length);
                }
            }

            decryptedMessage = Encoding.UTF8.GetString(plainText.ToArray());
        }

        return decryptedMessage;
    }

    public async Task<byte[]> DecryptBytesAsync(byte[] publicKey, byte[] encryptedMessage, byte[] iv)
    {
        throw new NotImplementedException();
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _aes?.Dispose();
            _diffieHellman?.Dispose();
        }
    }

    private Aes _aes;
    private ECDiffieHellmanCng _diffieHellman;
    private readonly byte[] _publicKey;
}