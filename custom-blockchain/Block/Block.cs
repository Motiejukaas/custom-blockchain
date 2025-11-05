using custom_blockchain.Hashing;
using System.Text;

namespace custom_blockchain.Block;

public class Block : IHashable
{
    public int Index { get; }
    public BlockHeader Header { get; }
    public BlockBody Body { get; }
    public string BlockHash { get; set; } = "";

    public Block(int index, BlockHeader header, BlockBody body)
    {
        Index = index;
        Header = header;
        Body = body;
        ComputeHash();
    }

    public void ComputeHash()
    {
        BlockHash = HashUtils.ComputeHashHex(GetBytes());
    }

    public byte[] GetBytes()
    {
        var sb = new StringBuilder();
        sb.Append(Header.PrevBlockHash);
        sb.Append(Header.Timestamp.ToBinary());
        sb.Append(Header.Version);
        sb.Append(Header.MerkleRootHash);
        sb.Append(Header.Nonce);
        sb.Append(Header.Difficulty);
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}