using System;

namespace LTA.Mobile.Crypt
{
    public interface ICryptoService : IDisposable
    {
        byte[] Encrypt(byte[] publicKey, string message);
        string Decrypt(byte[] publicKey, byte[] encryptedMessage, byte[] iv);
        void Dispose();
    }
}