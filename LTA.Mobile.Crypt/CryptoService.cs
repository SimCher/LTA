using System.Security.Cryptography;

namespace LTA.Mobile.Crypt
{
    public class CryptoService : ICryptoService
    {
        private Aes _aes;
        private ECDiffieHellman _diffieHellman;
        private readonly byte[] _publicKey;
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public byte[] Encrypt(byte[] publicKey, string message)
        {
            throw new System.NotImplementedException();
        }

        public string Decrypt(byte[] publicKey, byte[] encryptedMessage, byte[] iv)
        {
            throw new System.NotImplementedException();
        }
    }
}