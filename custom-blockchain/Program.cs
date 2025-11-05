using System.Diagnostics;
using custom_blockchain.Chain;
using custom_blockchain.Transaction;
using custom_blockchain.User;
using custom_blockchain.UTXO;
using custom_blockchain.Mining;

namespace custom_blockchain;

class Program
{
    static void Main(string[] args)
    {
        // === Genesis setup ===
        var users = GenerateUsers(5);
        var pool = new TransactionPool();
        var utxoSet = new UtxoSet();
        int difficulty = 1;

        // Initialize genesis UTXOs
        for (int i = 0; i < users.Count; i++)
        {
            var output = new TransactionOutput(users[i].PublicKey, 100000m);
            utxoSet.Add("genesis", i, output);
        }

        var chain = new Blockchain(pool, utxoSet, difficulty);
        var miner = new Miner();

        // === Generate and add transactions ===
        var txs = GenerateTransactions(users, 200);
        foreach (var tx in txs)
            pool.Add(tx);

        // === Mining loop ===
        int blockIndex = 1;
        while (pool.Count > 0)
        {
            var batch = pool.GetBatch(100);
            var lastBlock = chain.GetLastBlock();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n--- Mining Block #{blockIndex} ---");
            Console.ResetColor();
            Console.WriteLine($"Transactions: {batch.Count}");
            Console.WriteLine($"Previous Block Hash: {lastBlock.BlockHash}");
            Console.WriteLine($"Difficulty: {difficulty}\n");

            var stopwatch = Stopwatch.StartNew();
            var block = miner.MineBlock(blockIndex, lastBlock.BlockHash, batch, difficulty);
            stopwatch.Stop();

            chain.AddBlock(block);

            double seconds = stopwatch.Elapsed.TotalSeconds;
            double hashesTried = Math.Pow(16, difficulty); // rough estimate based on prefix probability
            double hashRate = hashesTried / seconds;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✔ Block #{blockIndex} mined successfully!");
            Console.ResetColor();
            Console.WriteLine($"Block Hash: {block.BlockHash}");
            Console.WriteLine($"Nonce used: {block.Header.Nonce}");
            Console.WriteLine($"Time taken: {seconds:F2}s");
            Console.WriteLine($"Approx. Hash Rate: {hashRate:F2} H/s");
            Console.WriteLine($"Current Chain Length: {chain.Chain.Count}");
            Console.WriteLine("-----------------------------");

            blockIndex++;
        }

        // === Display balances ===
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nFinal user balances (derived from UTXO):");
        Console.ResetColor();
        foreach (var u in users)
        {
            Console.WriteLine($"{u.Name}: {utxoSet.GetBalance(u.PublicKey)}");
        }

        Console.WriteLine("\nMining complete.");
    }

    // === Helper functions ===
    static List<UserAccount> GenerateUsers(int count)
    {
        var rnd = new Random();
        var users = new List<UserAccount>();
        for (int i = 0; i < count; i++)
            users.Add(new UserAccount($"User_{i:D4}", rnd.Next(100, 1000)));
        return users;
    }

    static List<Transaction.Transaction> GenerateTransactions(List<UserAccount> users, int count)
    {
        var rnd = new Random();
        var txs = new List<Transaction.Transaction>();
        for (int i = 0; i < count; i++)
        {
            var sender = users[rnd.Next(users.Count)];
            var receiver = users[rnd.Next(users.Count)];
            if (receiver.PublicKey == sender.PublicKey) continue;

            var amount = rnd.Next(1, 50);
            var inputs = new List<TransactionInput> { new("genesis", rnd.Next(users.Count)) };
            var outputs = new List<TransactionOutput> { new(receiver.PublicKey, amount) };

            txs.Add(new Transaction.Transaction(inputs, outputs));
        }
        return txs;
    }
}
