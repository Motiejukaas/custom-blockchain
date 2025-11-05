using System.Security.Cryptography;

namespace custom_blockchain.Hashing;

public static class HashUtils
{
    private static readonly CustomHash Hasher = new();

    public static byte[] ComputeHash(byte[] input) => Hasher.ComputeHash(input);
    
    public static string ComputeHashHex(byte[] input) =>
        Converter.BytesToHex(Hasher.ComputeHash(input));
    
    // public static byte[] ComputeHash(byte[] input) =>
    //     SHA256.HashData(input);
    //
    //
    // public static string ComputeHashHex(byte[] input) =>
    //     Converter.BytesToHex(ComputeHash(input));
}
