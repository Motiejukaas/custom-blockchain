namespace custom_blockchain.Transaction;

public class TransactionOutput
{
    public string ReceiverPublicKey { get; }
    public decimal Amount { get; }

    public TransactionOutput(string receiverPublicKey, decimal amount)
    {
        ReceiverPublicKey = receiverPublicKey;
        Amount = amount;
    }
}
