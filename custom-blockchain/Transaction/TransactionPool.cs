namespace custom_blockchain.Transaction;

public class TransactionPool
{
    private readonly List<Transaction> _pending = new();

    public void Add(Transaction tx) => _pending.Add(tx);

    public List<Transaction> GetBatch(int count) =>
        _pending.Take(count).ToList();

    public void Remove(IEnumerable<Transaction> confirmed)
    {
        var ids = confirmed.Select(c => c.Id).ToHashSet();
        _pending.RemoveAll(t => ids.Contains(t.Id));
    }

    public int Count => _pending.Count;
}
