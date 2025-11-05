namespace custom_blockchain.Hashing;

public class CustomHash
{
    private const int HashLengthBytes = 32;
    
    public byte[] ComputeHash(byte[] input)
    {
        byte[] randomBytes = [105, 27, 24, 71, 113, 30, 4, 26, 144, 187, 127, 67, 211, 216, 176, 75, 212, 125, 79, 99, 46, 17, 207, 113, 67, 125, 183, 178, 14, 68, 72, 12];
        byte[] hash = new byte[HashLengthBytes];
        
        int minLength = Math.Min(randomBytes.Length, input.Length);

        // XOR the overlapping part
        for (int i = 0; i < minLength; i++)
        {
            hash[i] = (byte)(randomBytes[i] ^ input[i]);
        }

        // Copy remaining randomBytes if input is shorter
        for (int i = minLength; i < randomBytes.Length; i++)
        {
            hash[i] = randomBytes[i];
        }
        
        // Applying sponge function
        if (input.Length > HashLengthBytes)
        {
            int hashIndex = 0;
            for (int inputIndex = HashLengthBytes; inputIndex < input.Length; ++inputIndex)
            {
                hash[hashIndex] = (byte)(hash[hashIndex] ^ (input[inputIndex] << 1));
                if (++hashIndex >= hash.Length)
                {
                    hashIndex = 0;
                }
            }
        }
        
        // Final diffusion: ensure each byte influences all positions
        for (int round = 0; round < 4; round++)
        {
            byte carry = 0;
            for (int i = 0; i < hash.Length; i++)
            {
                byte h = hash[i];
                byte rot = (byte)((h << 1) | (h >> 7));
                hash[i] = (byte)(rot ^ carry ^ (byte)i);
                carry = (byte)(carry + h);
            }
        }
        return hash;
    }
}