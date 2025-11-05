namespace custom_blockchain.Block;

public class BlockBody
{
    public List<Transaction.Transaction> Transactions { get; }

    public BlockBody(List<Transaction.Transaction> transactions)
    {
        Transactions = transactions;
    }
}