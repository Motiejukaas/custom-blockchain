using custom_blockchain.Block;
using custom_blockchain.Hashing;

namespace custom_blockchain.Mining;

public class Miner
{
    public Block.Block MineBlock(int index, string prevHash, List<Transaction.Transaction> transactions, int difficulty)
    {
        string merkleRoot = ComputeMerkleRoot(transactions);
        var header = new BlockHeader(prevHash, merkleRoot, difficulty);
        var body = new BlockBody(transactions);
        var block = new Block.Block(index, header, body);

        string targetPrefix = new string('0', difficulty);
        int nonce = 0;

        while (true)
        {
            header.Nonce = nonce;
            string hash = HashUtils.ComputeHashHex(header.GetBytes());
            if (hash.StartsWith(targetPrefix))
            {
                block.BlockHash = hash;
                break;
            }
            nonce++;
        }

        return block;
    }

    private static string ComputeMerkleRoot(List<Transaction.Transaction> txs)
    {
        if (txs.Count == 0)
            return "0";

        var hashes = txs.Select(t => HashUtils.ComputeHash(t.GetBytes())).ToList();

        while (hashes.Count > 1)
        {
            List<byte[]> newLevel = new();
            for (int i = 0; i < hashes.Count; i += 2)
            {
                var left = hashes[i];
                var right = (i + 1 < hashes.Count) ? hashes[i + 1] : left;
                var combined = left.Concat(right).ToArray();
                newLevel.Add(HashUtils.ComputeHash(combined));
            }
            hashes = newLevel;
        }

        return Converter.BytesToHex(hashes[0]);
    }
}