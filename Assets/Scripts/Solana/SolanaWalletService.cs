
using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

public class SolanaWalletService : MonoBehaviour, ISolanaWallet
{
    public static SolanaWalletService Instance { get; private set; }

    private const string BridgeClass =
        "com.wos.solana.SolanaBridge";

    [Header("Android")]
    [SerializeField] private string unityObjectName = "SolanaManager";

    // =========================================================
    // WALLET
    // =========================================================

    public string PublicKey { get; private set; }

    public bool IsConnected =>
        !string.IsNullOrEmpty(PublicKey);

    // =========================================================
    // SKR
    // =========================================================

    public string SkrBalance { get; private set; } = "0";

    public bool HasSkrBalance { get; private set; }

    public bool IsSkrLoading { get; private set; }

    public string SkrError { get; private set; }

    // =========================================================
    // SOL
    // =========================================================

    public string SolBalance { get; private set; } = "0";

    public bool HasSolBalance { get; private set; }

    public bool IsSolLoading { get; private set; }

    public string SolError { get; private set; }

    // =========================================================
    // TASK
    // =========================================================

    private TaskCompletionSource<string> connectTcs;

    // Legacy compatibility
    private TaskCompletionSource<string> mintTcs;

    // =========================================================
    // UNITY
    // =========================================================
public event System.Action<string> SkrDiagnosticReceived;
public event System.Action<string> SkrTransferCompleted;
public event System.Action<string> SolTransferCompleted;
public event System.Action<string> HardModePurchaseChecked;
public event System.Action<string> HardModePurchaseCheckCompleted;

public void OnHardModePurchaseCheck(string result)
{
    Debug.Log("[HardModeCheck] " + result);
    HardModePurchaseChecked?.Invoke(result);
    HardModePurchaseCheckCompleted?.Invoke(result);
}

public void CheckHardModePurchase(string treasuryAddress)
{
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        using (AndroidJavaClass bridge = new AndroidJavaClass(BridgeClass))
        {
            bridge.CallStatic("checkHardModePurchase", treasuryAddress);
        }
    }
    catch (Exception e)
    {
        Debug.LogError("[Solana] CheckHardModePurchase failed: " + e.Message);
        HardModePurchaseChecked?.Invoke("ERROR|" + e.Message);
        HardModePurchaseCheckCompleted?.Invoke("ERROR|" + e.Message);
    }
#else
    HardModePurchaseChecked?.Invoke("NOT_PURCHASED");
    HardModePurchaseCheckCompleted?.Invoke("NOT_PURCHASED");
#endif
}

public void OnSkrDiagnosticLog(string message)
{
    Debug.Log(
        "[SolanaWalletService] SKR DIAGNOSTIC:\n" +
        message
    );

    SkrDiagnosticReceived?.Invoke(message);
}

public void OnSkrTransferResult(string result)
{
    Debug.Log(
        "[SolanaWalletService] SKR transfer result: " + result
    );

    SkrTransferCompleted?.Invoke(result);
}

public void OnSolTransferResult(string result)
{
    Debug.Log(
        "[SolanaWalletService] SOL transfer result: " + result
    );

    SolTransferCompleted?.Invoke(result);
}

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        // Android uses this GameObject name for UnitySendMessage.
        name = unityObjectName;

        Debug.Log(
            "[Solana] SolanaWalletService initialized on GameObject: " +
            gameObject.name
        );
    }

    // =========================================================
    // CONNECT
    // =========================================================

    public async Task<bool> ConnectAsync()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        if (connectTcs != null &&
            !connectTcs.Task.IsCompleted)
        {
            Debug.LogWarning(
                "[Solana] Connection already in progress."
            );

            return false;
        }

        connectTcs =
            new TaskCompletionSource<string>();

        try
        {
            using (
                AndroidJavaClass bridge =
                new AndroidJavaClass(BridgeClass)
            )
            {
                Debug.Log(
                    "[Solana] Calling setUnityObjectName: " +
                    gameObject.name
                );

                bridge.CallStatic(
                    "setUnityObjectName",
                    gameObject.name
                );

                Debug.Log(
                    "[Solana] Calling connectWallet()"
                );

                bridge.CallStatic(
                    "connectWallet"
                );
            }
        }
        catch (AndroidJavaException e)
        {
            Debug.LogError(
                "[Solana] Android connect error:\n" +
                e
            );

            connectTcs.TrySetResult(
                "ERROR|" + e.Message
            );
        }
        catch (Exception e)
        {
            Debug.LogError(
                "[Solana] Connect error:\n" +
                e
            );

            connectTcs.TrySetResult(
                "ERROR|" + e.Message
            );
        }

        string result =
            await connectTcs.Task;

        if (string.IsNullOrEmpty(result))
        {
            Debug.LogError(
                "[Solana] Empty wallet response."
            );

            return false;
        }

        if (result.StartsWith(
            "ERROR|",
            StringComparison.OrdinalIgnoreCase
        ))
        {
            Debug.LogError(
                "[Solana] " + result
            );

            return false;
        }

        if (result.StartsWith(
            "FAILED:",
            StringComparison.OrdinalIgnoreCase
        ))
        {
            Debug.LogError(
                "[Solana] " + result
            );

            return false;
        }

        string[] parts =
            result.Split('|');

        if (parts.Length == 0 ||
            string.IsNullOrWhiteSpace(parts[0]))
        {
            Debug.LogError(
                "[Solana] Invalid wallet response: " +
                result
            );

            return false;
        }

        PublicKey =
            parts[0].Trim();

        Debug.Log(
            "[Solana] Wallet connected: " +
            PublicKey
        );

        // SKR balance is requested by Android automatically
        // after authorization.

        return true;

#else

        Debug.Log(
            "[Solana] Wallet connection is only available on Android."
        );

        return false;

#endif
    }

    // =========================================================
    // SKR BALANCE
    // =========================================================

    public void RefreshSkrBalance()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        if (!IsConnected)
        {
            HasSkrBalance = false;
            IsSkrLoading = false;
            SkrError = "Wallet not connected";
            SkrBalance = "0";

            Debug.LogWarning(
                "[SKR] Cannot refresh: wallet not connected."
            );

            return;
        }

        if (IsSkrLoading)
        {
            Debug.Log(
                "[SKR] Balance request already running."
            );

            return;
        }

        IsSkrLoading = true;
        HasSkrBalance = false;
        SkrError = null;

        try
        {
            using (
                AndroidJavaClass bridge =
                new AndroidJavaClass(BridgeClass)
            )
            {
                Debug.Log(
                    "[SKR] Calling getSkrBalance()"
                );

                bridge.CallStatic(
                    "getSkrBalance"
                );
            }
        }
        catch (AndroidJavaException e)
        {
            IsSkrLoading = false;
            SkrError = e.Message;

            Debug.LogError(
                "[SKR] Android error:\n" +
                e
            );
        }
        catch (Exception e)
        {
            IsSkrLoading = false;
            SkrError = e.Message;

            Debug.LogError(
                "[SKR] Request error:\n" +
                e
            );
        }

#else

        Debug.Log(
            "[SKR] SKR balance is only available on Android."
        );

#endif
    }

    // =========================================================
    // SOL BALANCE
    // =========================================================

    public void RefreshSolBalance()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        if (!IsConnected)
        {
            HasSolBalance = false;
            IsSolLoading = false;
            SolError = "Wallet not connected";
            SolBalance = "0";
            return;
        }

        if (IsSolLoading) return;

        IsSolLoading = true;
        HasSolBalance = false;
        SolError = null;

        try
        {
            using (AndroidJavaClass bridge = new AndroidJavaClass(BridgeClass))
            {
                bridge.CallStatic("getSolBalance");
            }
        }
        catch (Exception e)
        {
            IsSolLoading = false;
            SolError = e.Message;
            Debug.LogError("[SOL] Request error: " + e.Message);
        }
#endif
    }

    // =========================================================
    // ANDROID CALLBACK - WALLET
    // =========================================================

    public void OnWalletConnected(string result)
    {
        Debug.Log(
            "[Solana] OnWalletConnected received: " +
            result
        );

        if (string.IsNullOrEmpty(result))
        {
            connectTcs?.TrySetResult(
                "ERROR|Empty wallet response"
            );

            return;
        }

        connectTcs?.TrySetResult(result);
    }

    // =========================================================
    // ANDROID CALLBACK - SKR
    // =========================================================

    public void OnSkrBalance(string result)
    {
        Debug.Log(
            "[SKR] OnSkrBalance received: " +
            result
        );

        IsSkrLoading = false;

        if (string.IsNullOrEmpty(result))
        {
            HasSkrBalance = false;
            SkrError = "Empty SKR response";

            Debug.LogError(
                "[SKR] Empty response."
            );

            return;
        }

        if (result.StartsWith(
            "ERROR|",
            StringComparison.OrdinalIgnoreCase
        ))
        {
            HasSkrBalance = false;
            SkrError =
                result.Substring(
                    "ERROR|".Length
                );

            Debug.LogError(
                "[SKR] RPC error: " +
                SkrError
            );

            return;
        }

        decimal balance;

        bool valid =
            decimal.TryParse(
                result.Trim(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out balance
            );

        if (!valid)
        {
            HasSkrBalance = false;

            SkrError =
                "Invalid balance: " +
                result;

            Debug.LogError(
                "[SKR] " +
                SkrError
            );

            return;
        }

        if (balance < 0)
        {
            HasSkrBalance = false;

            SkrError =
                "Negative balance returned.";

            Debug.LogError(
                "[SKR] " +
                SkrError
            );

            return;
        }

        SkrBalance =
            balance.ToString(
                "0.######",
                CultureInfo.InvariantCulture
            );

        HasSkrBalance = true;
        SkrError = null;

        Debug.Log(
            "[SKR] Balance = " +
            SkrBalance
        );
    }

    public void OnSolBalance(string result)
    {
        Debug.Log("[SOL] OnSolBalance received: " + result);
        IsSolLoading = false;

        if (string.IsNullOrEmpty(result) || result.StartsWith("ERROR|"))
        {
            HasSolBalance = false;
            SolError = result?.Replace("ERROR|", "") ?? "Empty response";
            return;
        }

        decimal balance;
        if (decimal.TryParse(result.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out balance))
        {
            SolBalance = balance.ToString("0.######", CultureInfo.InvariantCulture);
            HasSolBalance = true;
            SolError = null;
        }
    }

    // =========================================================
    // LEGACY NFT COMPATIBILITY
    // =========================================================

    public async Task<string> MintWoodAxeAsync()
    {
        if (!IsConnected)
            return "ERROR|Wallet not connected";

#if UNITY_ANDROID && !UNITY_EDITOR

        mintTcs =
            new TaskCompletionSource<string>();

        try
        {
            using (
                AndroidJavaClass bridge =
                new AndroidJavaClass(BridgeClass)
            )
            {
                bridge.CallStatic(
                    "mintWoodAxe"
                );
            }
        }
        catch (Exception e)
        {
            return "ERROR|" + e.Message;
        }

        return await mintTcs.Task;

#else

        return "ERROR|NFT mint unavailable";

#endif
    }

    public void OnNftMinted(string result)
    {
        mintTcs?.TrySetResult(result);
    }

    // =========================================================
    // TRANSACTION CALLBACK
    // =========================================================

    public void OnTransactionSent(string result)
    {
        Debug.Log(
            "[Solana] Transaction callback: " +
            result
        );
    }

    // =========================================================
    // DISCONNECT
    // =========================================================

    public void Disconnect()
    {
        PublicKey = null;

        SkrBalance = "0";

        HasSkrBalance = false;

        IsSkrLoading = false;

        SkrError = null;

        connectTcs = null;
        mintTcs = null;

        Debug.Log(
            "[Solana] Wallet disconnected."
        );
    }
}
