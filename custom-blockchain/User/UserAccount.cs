using System.Text;
using custom_blockchain.Hashing;

namespace custom_blockchain.User;

public class UserAccount : IHashable
{
    public string Name { get; }
    public string PublicKey { get; }
    public decimal Balance { get; set; }

    public UserAccount(string name, decimal balance)
    {
        Name = name;
        Balance = balance;
        PublicKey = GeneratePublicKey(name);
    }

    private static string GeneratePublicKey(string seed)
    {
        var data = Encoding.UTF8.GetBytes(seed + Guid.NewGuid());
        return HashUtils.ComputeHashHex(data);
    }

    public byte[] GetBytes() =>
        Encoding.UTF8.GetBytes($"{Name}:{PublicKey}:{Balance}");
}