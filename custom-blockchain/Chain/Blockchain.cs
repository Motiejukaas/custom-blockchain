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

        // Remove confirmed txs and update UTXOs
        foreach (var tx in block.Body.Transactions)
        {
            foreach (var input in tx.Inputs)
                _utxoSet.Remove(input.PreviousTxId, input.OutputIndex);

            for (int i = 0; i < tx.Outputs.Count; i++)
                _utxoSet.Add(tx.Id, i, tx.Outputs[i]);
        }

        _pool.Remove(block.Body.Transactions);
    }
}