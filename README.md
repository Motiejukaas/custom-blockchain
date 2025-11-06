# Custom Blockchain (C#/.NET 9)

Blockchain prototype with a UTXO model, a simple transaction pool (mempool), Proof‑of‑Work mining, and a custom hash function.

## Architecture and components

Layout (folder `custom-blockchain/`):
- Block/
  - `Block.cs` — block container (Index, Header, Body, BlockHash), can compute the block hash.
  - `BlockHeader.cs` — header: PrevBlockHash, Timestamp, Version, MerkleRootHash, Nonce, Difficulty.
  - `BlockBody.cs` — list of transactions.
- Chain/
  - `Blockchain.cs` — chain state: list of blocks, genesis creation, adding blocks, lookup, printing.
- Transaction/
  - `Transaction.cs` — transaction (Inputs, Outputs, Timestamp, Id), serialization and Id calculation.
  - `TransactionInput.cs`, `TransactionOutput.cs` — inputs/outputs.
  - `TransactionPool.cs` — pending transactions (mempool), batch retrieval.
- UTXO/
  - `UtxoSet.cs` — unspent outputs map (UTXO), balance calculation, add/remove entries.
- User/
  - `UserAccount.cs` — user with a name, public key, and an initial balance value (balance is informational; real funds are tracked via UTXO).
- Hashing/
  - `CustomHash.cs` — custom 32‑byte hash function.
  - `HashUtils.cs` — convenience APIs to get hash as bytes/hex, wired to `CustomHash`.
  - `Converter.cs` — bytes↔hex conversion.
  - `IHashable.cs` — interface for objects that can provide a byte representation.
- Mining/
  - `Miner.cs` — mining: computes Merkle root and iterates nonce until a target prefix.
- `Program.cs` — sample program: initializes users/UTXO, generates transactions, mines, prints balances, and includes a simple REPL for block lookup.

### High‑level flow
1) Initialization: `Program.cs` creates 1000 users and assigns each a UTXO (genesis entries). Initializes `Blockchain`, `TransactionPool`, `UtxoSet`.
2) Transactions: generates N random transfers (single input → single output), adds them to `TransactionPool`, and immediately mirrors effects in `UtxoSet` to simulate a more realistic state.
3) Mining: takes batches of 100 transactions, computes the Merkle root, and iterates `Nonce` until the header hash’s hex representation starts with a number of zero nibbles equal to `difficulty`.
4) Confirmation: `Blockchain.AddBlock` removes spent inputs from UTXO and adds new outputs; confirmed transactions are removed from `TransactionPool`.
5) Display: prints final balances and opens a REPL — find a block by index or hash and print its details.

## Data structures
- Transaction (`Transaction`):
  - Inputs: list of (PreviousTxId, OutputIndex)
  - Outputs: list of (ReceiverPublicKey, Amount)
  - Timestamp
  - Id = Hash(GetBytes()), where GetBytes serializes inputs/outputs deterministically.

- UTXO entry: key "{txId}:{index}" → `TransactionOutput`.

- Block header (`BlockHeader`):
  - PrevBlockHash, Timestamp, Version, MerkleRootHash, Nonce, Difficulty
  - GetBytes() concatenates values as UTF‑8 string — the hash depends on these fields.

- Block (`Block`):
  - Index, Header, Body, BlockHash
  - BlockHash = Hash(Header.GetBytes()) (used across the project; mining uses the header hash condition).

## Mining (Proof‑of‑Work)
`Mining/Miner.cs` logic:
- Compute the Merkle root from transactions (pair and hash; if odd count, duplicate the last).
- Build `BlockHeader` with `PrevBlockHash`, `MerkleRootHash`, `Difficulty`.
- Iterate `Nonce` (0, 1, 2, …) and compute the header hash (hex). If it starts with `difficulty` zero hex nibbles (e.g., `00…`), the block is mined.

## How to run and use
Requirements:
- .NET SDK

Run (Windows, PowerShell):
1) Open the solution in VS Code or Rider/VS.
2) From the `custom-blockchain/custom-blockchain` folder run:

```powershell
dotnet build
dotnet run
```

What you’ll see:
- Periodic console output: “Mining Block #…”, transaction count, previous block hash, difficulty.
- On success: the block hash, nonce used, elapsed time, and an approximate hash rate.
- Finally: user balances and an interactive lookup (enter an index or a hash).

## Screenshots
- Mining success:
  

- Block lookup with details:
  

Additionally, an example console output (a text “screenshot”):

```
--- Mining Block #55 ---
Transactions: 100
Previous Block Hash: 05d7f3585833ed77e19132c46707ce987f79adc03588fcf971d3639524fdee7b
Difficulty: 1

Block #55 mined successfully!
Block Hash: 0efc3ed40bf3099bf4ac11f52d2d33392f4a05113d961407e6223ed9757bd3af
Nonce used: 9
Time taken: 0.00s
Approx. Hash Rate: 20340.71 H/s
Current Chain Length: 56
-----------------------------

```

## Configuration and experiments
Edit `Program.cs`:
- `int difficulty = 2;` — mining difficulty.
- Number of users: `GenerateUsers(1000)`.
- Number of transactions: `GenerateTransactions(users, 10000, utxoSet)`.
- Batch size for mining: `pool.GetBatch(100)`.
- You can switch hashing to SHA‑256 in `Hashing/HashUtils.cs`.