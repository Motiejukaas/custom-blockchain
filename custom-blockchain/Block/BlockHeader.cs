using System.Text;
using custom_blockchain.Hashing;

namespace custom_blockchain.Block;

public class BlockHeader : IHashable
{
    public string PrevBlockHash { get; }
    public DateTime Timestamp { get; }
    public string Version { get; } = "v0.2";
    public string MerkleRootHash { get; }
    public int Nonce { get; set; }
    public int Difficulty { get; }

    public BlockHeader(string prevHash, string merkleRoot, int difficulty)
    {
        PrevBlockHash = prevHash;
        MerkleRootHash = merkleRoot;
        Difficulty = difficulty;
        Timestamp = DateTime.UtcNow;
    }

    public byte[] GetBytes()
    {
        var input = $"{PrevBlockHash}{Timestamp.ToBinary()}{Version}{MerkleRootHash}{Nonce}{Difficulty}";
        return Encoding.UTF8.GetBytes(input);
    }
}