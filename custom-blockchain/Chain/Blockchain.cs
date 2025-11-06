using custom_blockchain.Block;
using custom_blockchain.Transaction;
using custom_blockchain.UTXO;

namespace custom_blockchain.Chain;

public class Blockchain
{
    public List<Block.Block> Chain { get; } = new();
    private readonly TransactionPool _pool;
    private readonly UtxoSet _utxoSet;
    private readonly int _difficulty;

    public Blockchain(TransactionPool pool, UtxoSet utxoSet, int difficulty = 3)
    {
        _pool = pool;
        _utxoSet = utxoSet;
        _difficulty = difficulty;
        CreateGenesisBlock();
    }

    private void CreateGenesisBlock()
    {
        var header = new BlockHeader("0", "GENESIS", _difficulty);
        var body = new BlockBody(new List<Transaction.Transaction>());
        var block = new Block.Block(0, header, body);
        block.ComputeHash();
        Chain.Add(block);
    }

    public Block.Block GetLastBlock() => Chain.Last();

    public void AddBlock(Block.Block block)
    {
        Chain.Add(block);

        foreach (var tx in block.Body.Transactions)
        {
            foreach (var input in tx.Inputs)
                _utxoSet.Remove(input.PreviousTxId, input.OutputIndex);

            for (int i = 0; i < tx.Outputs.Count; i++)
                _utxoSet.Add(tx.Id, i, tx.Outputs[i]);
        }

        _pool.Remove(block.Body.Transactions);
    }

    // === Lookup methods ===
    public Block.Block? FindBlockByIndex(int index)
    {
        return Chain.FirstOrDefault(b => b.Index == index);
    }

    public Block.Block? FindBlockByHash(string hash)
    {
        return Chain.FirstOrDefault(b => b.BlockHash.Equals(hash, StringComparison.OrdinalIgnoreCase));
    }

    public void PrintBlockInfo(Block.Block block)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n=== Block #{block.Index} ===");
        Console.ResetColor();

        Console.WriteLine($"Hash: {block.BlockHash}");
        Console.WriteLine($"Previous Hash: {block.Header.PrevBlockHash}");
        Console.WriteLine($"Timestamp: {block.Header.Timestamp}");
        Console.WriteLine($"Difficulty: {block.Header.Difficulty}");
        Console.WriteLine($"Nonce: {block.Header.Nonce}");
        Console.WriteLine($"Merkle Root: {block.Header.MerkleRootHash}");
        Console.WriteLine($"Transactions in block: {block.Body.Transactions.Count}");

        if (block.Body.Transactions.Count == 0)
        {
            Console.WriteLine("  (No transactions in this block)");
            return;
        }

        Console.WriteLine("\n--- Transactions ---");
        int count = 1;
        foreach (var tx in block.Body.Transactions)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{count}] Transaction ID: {tx.Id}");
            Console.ResetColor();

            if (tx.Inputs.Count > 0)
            {
                Console.WriteLine("  Inputs:");
                foreach (var input in tx.Inputs)
                    Console.WriteLine($"    From Tx: {input.PreviousTxId} (Output #{input.OutputIndex})");
            }

            if (tx.Outputs.Count > 0)
            {
                Console.WriteLine("  Outputs:");
                foreach (var output in tx.Outputs)
                    Console.WriteLine($"    Receiver: {output.ReceiverPublicKey} | Amount: {output.Amount}");
            }

            Console.WriteLine($"  Timestamp: {tx.Timestamp}");
            Console.WriteLine();
            count++;
        }
    }
}