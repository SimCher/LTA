using System;
using System.Security.Cryptography;
using LTA.Mobile.Application.Interfaces;

namespace LTA.Mobile.Application.Services;

public class CryptoService : IDisposable
{
    private static Aes Aes { get; }
    private static ECDiffieHellmanCng DiffieHellman { get; }
    public static byte[] PublicKey { get; }

    public static byte[] IV => Aes.IV;

    static CryptoService()
    {
        Aes = new AesCryptoServiceProvider();
        DiffieHellman = new ECDiffieHellmanCng()
        {
            KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
            HashAlgorithm = CngAlgorithm.Sha256
        };

        PublicKey = DiffieHellman.PublicKey.ToByteArray();
    }

    public static byte[] Encrypt(byte[] publicKey, string message)
    {
        throw new System.NotImplementedException();
    }

    public static string Decrypt(byte[] publicKey, byte[] encryptedMessage, byte[] iv)
    {
        throw new System.NotImplementedException();
    }

    public void Dispose()
    {
    }
}