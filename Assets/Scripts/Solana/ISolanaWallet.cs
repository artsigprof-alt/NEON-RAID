
using System.Threading.Tasks;

public interface ISolanaWallet
{
    Task<bool> ConnectAsync();

    Task<string> MintWoodAxeAsync();

    void Disconnect();

    string PublicKey { get; }

    bool IsConnected { get; }
}

