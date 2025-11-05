using System.Text;
using custom_blockchain.Hashing;

namespace custom_blockchain.Transaction;

public class Transaction : IHashable
{
    public string Id { get; private set; } = string.Empty;
    public List<TransactionInput> Inputs { get; } = new();
    public List<TransactionOutput> Outputs { get; } = new();
    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public Transaction(IEnumerable<TransactionInput> inputs, IEnumerable<TransactionOutput> outputs)
    {
        Inputs.AddRange(inputs);
        Outputs.AddRange(outputs);
        ComputeId();
    }

    private void ComputeId()
    {
        Id = HashUtils.ComputeHashHex(GetBytes());
    }

    public byte[] GetBytes()
    {
        var sb = new StringBuilder();
        foreach (var input in Inputs)
            sb.Append($"{input.PreviousTxId}:{input.OutputIndex};");
        foreach (var output in Outputs)
            sb.Append($"{output.ReceiverPublicKey}:{output.Amount};");
        sb.Append(Timestamp.ToBinary());
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
