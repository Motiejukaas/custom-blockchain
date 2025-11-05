using custom_blockchain.Transaction;

namespace custom_blockchain.UTXO;

public class UtxoSet
{
    private readonly Dictionary<string, TransactionOutput> _utxos = new();

    public void Add(string txId, int index, TransactionOutput output)
        => _utxos[$"{txId}:{index}"] = output;

    public bool Contains(string txId, int index)
        => _utxos.ContainsKey($"{txId}:{index}");

    public TransactionOutput? Get(string txId, int index)
        => _utxos.GetValueOrDefault($"{txId}:{index}");

    public void Remove(string txId, int index)
        => _utxos.Remove($"{txId}:{index}");

    public decimal GetBalance(string publicKey)
        => _utxos.Values
            .Where(o => o.ReceiverPublicKey == publicKey)
            .Sum(o => o.Amount);
}
