using System.Threading.Tasks;

namespace LTA.Mobile.Application.Interfaces;

public interface ICryptoService
{
    byte[] PublicKey { get; }
    byte[] IV { get; }

    Task<byte[]> EncryptAsync(byte[] publicKey, string message);
    Task<byte[]> EncryptAsync(byte[] publicKey, byte[] message);
    Task<string> DecryptTextAsync(byte[] publicKey, byte[] encryptedMessage, byte[] iv);
    Task<byte[]> DecryptBytesAsync(byte[] publicKey, byte[] encryptedMessage, byte[] iv);
}