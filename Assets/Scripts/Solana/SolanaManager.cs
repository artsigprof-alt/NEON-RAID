using UnityEngine;

public class SolanaManager : MonoBehaviour
{
    private const string BridgeClass =
        "com.wos.solana.SolanaBridge";

    public void MintCustomNFT(string nftName, string metadataUri)
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        Debug.Log("[Solana] Mint started");
        Debug.Log("[Solana] NFT name: " + nftName);
        Debug.Log("[Solana] Metadata URI: " + metadataUri);

        try
        {
            using (AndroidJavaClass solanaBridge =
                   new AndroidJavaClass(BridgeClass))
            {
                Debug.Log("[Solana] Bridge loaded");

                solanaBridge.CallStatic(
                    "mintNft",
                    nftName,
                    metadataUri
                );

                Debug.Log("[Solana] mintNft() called");
            }
        }
        catch (AndroidJavaException e)
        {
            Debug.LogError(
                "[Solana] AndroidJavaException:\n" + e
            );
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                "[Solana] Exception:\n" + e
            );
        }

#else

        Debug.Log(
            $"[Solana] Mint в Unity Editor недоступен: {nftName}"
        );

#endif
    }

    public void MintAxe()
    {
        MintCustomNFT(
            "axe prot",
            "https://moccasin-binding-mastodon-788.mypinata.cloud/ipfs/bafkreigpwzh3fmkcxz57iepylhtm4ugrpzabsn6d7gxanuwsev547wfeti"
        );
    }

    public void MintSword()
    {
        MintCustomNFT(
            "Golden Sword",
            "https://gateway.pinata.cloud/ipfs/ваша_ссылка_на_sword"
        );
    }
}