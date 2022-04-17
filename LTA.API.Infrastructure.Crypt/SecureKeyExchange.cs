using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LTA.API.Infrastructure.Crypt
{
    public class SecureKeyExchange : IDisposable
    {
        private Aes _aes;
        private ECDiffieHellmanCng _diffieHellman;
        private readonly byte[] _publicKey;

        public SecureKeyExchange()
        {
            _aes = new AesCryptoServiceProvider();

            _diffieHellman = new ECDiffieHellmanCng
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                HashAlgorithm = CngAlgorithm.Sha256
            };

            _publicKey = _diffieHellman.PublicKey.ToByteArray();
        }

        public byte[] PublicKey => _publicKey;
        public byte[] IV => _aes.IV;

        public byte[] Encrypt(byte[] publicKey, string secretMessage)
        {
            var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
            var derivedKey = _diffieHellman.DeriveKeyMaterial(key);

            _aes.Key = derivedKey;

            using var cipherText = new MemoryStream();
            using var encryptor = _aes.CreateEncryptor();
            using var cryptoStream = new CryptoStream(cipherText, encryptor, CryptoStreamMode.Write);

            var cipherTextMessage = Encoding.UTF8.GetBytes(secretMessage);
            cryptoStream.Write(cipherTextMessage, 0, cipherTextMessage.Length);

            return cipherText.ToArray();
        }

        public string Decrypt(byte[] publicKey, byte[] encryptedMessage, byte[] iv)
        {
            var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
            var derivedKey = _diffieHellman.DeriveKeyMaterial(key);

            _aes.Key = derivedKey;
            _aes.IV = iv;

            using var plainText = new MemoryStream();
            using var decryptor = _aes.CreateDecryptor();
            using var cryptoStream = new CryptoStream(plainText, decryptor, CryptoStreamMode.Write);

            cryptoStream.Write(encryptedMessage, 0, encryptedMessage.Length);

            return Encoding.UTF8.GetString(plainText.ToArray());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing) return;
            _aes?.Dispose();

            _diffieHellman?.Dispose();
        }
    }
}
