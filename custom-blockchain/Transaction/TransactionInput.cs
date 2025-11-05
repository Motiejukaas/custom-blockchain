namespace custom_blockchain.Transaction;

public class TransactionInput
{
    public string PreviousTxId { get; }
    public int OutputIndex { get; }

    public TransactionInput(string prevTxId, int index)
    {
        PreviousTxId = prevTxId;
        OutputIndex = index;
    }
}
