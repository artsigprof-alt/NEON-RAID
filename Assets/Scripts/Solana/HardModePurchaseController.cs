using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HardModePurchaseController : MonoBehaviour
{
    [Header("TREASURY")]
    [SerializeField]
    private string treasuryWallet =
        "5oTxwPdy8YXbxV8Pe2bENYeEsa3YmPoXP8PBjT22vvvX";

    [Header("PRICE")]
    [SerializeField]
    private decimal priceSol = 0.1m;

    [Header("FIRST TEST")]
    [SerializeField]
    private bool testMode = true;

    [SerializeField]
    private decimal testAmountSol = 0.01m;

    [Header("HARD MODE")]
    [SerializeField]
    private string hardMenuSceneName = "HardMenu";

    [Header("UI")]
    [SerializeField]
    private TMP_Text statusText;

    [SerializeField]
    private TMP_Text priceText;

    [Header("DIAGNOSTIC LOG")]
    [SerializeField]
    private TMP_Text diagnosticText;

    private bool transactionInProgress;

    // true = только проверяем покупку, НЕ запускаем оплату,
    // если покупка не найдена.
    private bool checkOnlyMode;

    private const string HARD_MODE_UNLOCKED =
        "NEON_RAID_HARD_MODE_UNLOCKED";

    private const string HARD_MODE_TX =
        "NEON_RAID_HARD_MODE_TX";

    private void Start()
    {
        if (SolanaWalletService.Instance != null)
        {
            SolanaWalletService.Instance.HardModePurchaseCheckCompleted +=
                OnHardModePurchaseCheck;

            SolanaWalletService.Instance.SkrTransferCompleted +=
                OnTransferResult;

            SolanaWalletService.Instance.SolTransferCompleted +=
                OnTransferResult;

            SolanaWalletService.Instance.SkrDiagnosticReceived +=
                OnDiagnosticLog;
        }

        UpdatePriceText();
    }

    private void OnDestroy()
    {
        if (SolanaWalletService.Instance != null)
        {
            SolanaWalletService.Instance.HardModePurchaseCheckCompleted -=
                OnHardModePurchaseCheck;

            SolanaWalletService.Instance.SkrTransferCompleted -=
                OnTransferResult;

            SolanaWalletService.Instance.SolTransferCompleted -=
                OnTransferResult;

            SolanaWalletService.Instance.SkrDiagnosticReceived -=
                OnDiagnosticLog;
        }
    }

    private void UpdatePriceText()
    {
        if (priceText == null)
            return;

        decimal displayedPrice =
            testMode ? testAmountSol : priceSol;

        priceText.text =
            displayedPrice.ToString(
                "0.###",
                CultureInfo.InvariantCulture
            ) + " SOL";
    }

    // =========================================================
    // HARD MODE PURCHASE CHECK CALLBACK
    // =========================================================

    private void OnHardModePurchaseCheck(string result)
    {
        Debug.Log("[HardModeCheck] " + result);

        if (string.IsNullOrEmpty(result))
        {
            transactionInProgress = false;
            checkOnlyMode = false;

            SetStatus("Empty purchase check result.");
            return;
        }

        // -----------------------------------------------------
        // PURCHASE FOUND
        // -----------------------------------------------------

        if (result.StartsWith("PURCHASED|"))
        {
            string signature =
                result.Substring("PURCHASED|".Length);

            if (string.IsNullOrEmpty(signature))
            {
                transactionInProgress = false;
                checkOnlyMode = false;

                SetStatus("Purchase found, but signature is empty.");
                return;
            }

            PlayerPrefs.SetInt(
                HARD_MODE_UNLOCKED,
                1
            );

            PlayerPrefs.SetString(
                HARD_MODE_TX,
                signature
            );

            PlayerPrefs.Save();

            transactionInProgress = false;
            checkOnlyMode = false;

            SetStatus(
                "HARD MODE UNLOCKED\n" +
                signature
            );

            OpenHardMenu();

            return;
        }

        // -----------------------------------------------------
        // PURCHASE NOT FOUND
        // -----------------------------------------------------

        if (result == "NOT_PURCHASED")
        {
            transactionInProgress = false;

            // TEST BUTTON:
            // Только проверяем. Никакой оплаты.
            if (checkOnlyMode)
            {
                checkOnlyMode = false;

                SetStatus(
                    "NO PREVIOUS PURCHASE FOUND"
                );

                return;
            }

            // NORMAL BUY BUTTON:
            // Если покупки нет — начинаем оплату.
            checkOnlyMode = false;

            SetStatus(
                "No previous purchase found.\n" +
                "Opening payment..."
            );

            ExecuteSolPayment();

            return;
        }

        // -----------------------------------------------------
        // RPC / NETWORK / OTHER ERROR
        // -----------------------------------------------------

        if (result.StartsWith("ERROR|"))
        {
            string error =
                result.Substring("ERROR|".Length);

            transactionInProgress = false;
            checkOnlyMode = false;

            SetStatus(
                "Purchase check failed:\n" +
                error
            );

            return;
        }

        // -----------------------------------------------------
        // UNKNOWN RESULT
        // -----------------------------------------------------

        transactionInProgress = false;
        checkOnlyMode = false;

        SetStatus(result);
    }

    // =========================================================
    // DIAGNOSTICS
    // =========================================================

    private void OnDiagnosticLog(string message)
    {
        Debug.Log(
            "[HardMode] DIAGNOSTIC:\n" +
            message
        );

        if (diagnosticText == null)
            return;

        diagnosticText.text +=
            "\n" +
            message +
            "\n";
    }

    // =========================================================
    // BUY HARD MODE
    // =========================================================

    public void BuyHardMode()
    {
        if (transactionInProgress)
        {
            Debug.Log(
                "[HardMode] Transaction already in progress."
            );

            return;
        }

        if (SolanaWalletService.Instance == null)
        {
            SetStatus(
                "Wallet service unavailable."
            );

            return;
        }

        if (!SolanaWalletService.Instance.IsConnected)
        {
            SetStatus(
                "Connect wallet first."
            );

            return;
        }

        checkOnlyMode = false;
        transactionInProgress = true;

        SetStatus(
            "Checking previous purchase..."
        );

        SolanaWalletService.Instance.CheckHardModePurchase(
            treasuryWallet
        );
    }

    // =========================================================
    // TEST CHECK
    // =========================================================

    public void TestHardModePurchaseCheck()
    {
        if (transactionInProgress)
        {
            Debug.Log(
                "[HardMode] Transaction/check already in progress."
            );

            return;
        }

        if (SolanaWalletService.Instance == null)
        {
            SetStatus(
                "Wallet service unavailable."
            );

            return;
        }

        if (!SolanaWalletService.Instance.IsConnected)
        {
            SetStatus(
                "Connect wallet first for test."
            );

            return;
        }

        // ВАЖНО:
        // testMode здесь означает только проверку.
        // Оплата автоматически НЕ запускается.
        checkOnlyMode = true;
        transactionInProgress = true;

        SetStatus(
            "TEST CHECK STARTING..."
        );

        SolanaWalletService.Instance.CheckHardModePurchase(
            treasuryWallet
        );
    }

    // =========================================================
    // EXECUTE SOL PAYMENT
    // =========================================================

    private void ExecuteSolPayment()
    {
        if (transactionInProgress)
            return;

        decimal amount =
            testMode
                ? testAmountSol
                : priceSol;

        decimal currentBalance;

        if (!decimal.TryParse(
                SolanaWalletService.Instance.SolBalance,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out currentBalance))
        {
            SetStatus(
                "Unable to read SOL balance."
            );

            return;
        }

        if (currentBalance < amount)
        {
            SetStatus(
                "Not enough SOL. Required: " +
                amount.ToString(
                    "0.###",
                    CultureInfo.InvariantCulture
                )
            );

            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR

        transactionInProgress = true;

        SetStatus(
            "Confirm the SOL transaction in your wallet..."
        );

        try
        {
            using (
                AndroidJavaClass bridge =
                    new AndroidJavaClass(
                        "com.wos.solana.SolanaBridge"
                    )
            )
            {
                bridge.CallStatic(
                    "sendSol",
                    amount.ToString(
                        "0.#########",
                        CultureInfo.InvariantCulture
                    ),
                    treasuryWallet
                );
            }
        }
        catch (Exception e)
        {
            transactionInProgress = false;

            SetStatus(
                "Transaction error: " +
                e.Message
            );
        }

#else

        transactionInProgress = false;

        SetStatus(
            "SOL transactions require an Android build."
        );

#endif
    }

    // =========================================================
    // SOL / SKR TRANSFER CALLBACK
    // =========================================================

    private void OnTransferResult(string result)
    {
        transactionInProgress = false;

        Debug.Log(
            "[HardMode] Result: " +
            result
        );

        if (string.IsNullOrEmpty(result))
        {
            SetStatus(
                "Empty transaction result."
            );

            return;
        }

        // -----------------------------------------------------
        // SUCCESS
        // -----------------------------------------------------

        if (result.StartsWith("SUCCESS|"))
        {
            string signature =
                result.Substring(
                    "SUCCESS|".Length
                );

            if (string.IsNullOrEmpty(signature))
            {
                SetStatus(
                    "Transaction succeeded, but signature is empty."
                );

                return;
            }

            Debug.Log(
                "[HardMode] CONFIRMED TX: " +
                signature
            );

            // TEST MODE:
            // Только показываем успешный перевод.
            // Unlock делаем через on-chain checker.
            if (testMode)
            {
                SetStatus(
                    "TEST TRANSFER SUCCESS\n" +
                    signature
                );

                return;
            }

            // REAL MODE:
            // После подтвержденной транзакции
            // разблокируем Hard Mode.
            PlayerPrefs.SetInt(
                HARD_MODE_UNLOCKED,
                1
            );

            PlayerPrefs.SetString(
                HARD_MODE_TX,
                signature
            );

            PlayerPrefs.Save();

            SetStatus(
                "HARD MODE UNLOCKED"
            );

            OpenHardMenu();

            return;
        }

        // -----------------------------------------------------
        // ERROR
        // -----------------------------------------------------

        if (result.StartsWith("ERROR|"))
        {
            string error =
                result.Substring(
                    "ERROR|".Length
                );

            SetStatus(
                "Transaction failed:\n" +
                error
            );

            return;
        }

        // -----------------------------------------------------
        // UNKNOWN RESULT
        // -----------------------------------------------------

        SetStatus(result);
    }

    // =========================================================
    // OPEN HARD MENU
    // =========================================================

    private void OpenHardMenu()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(
                hardMenuSceneName))
        {
            Debug.LogError(
                "[HardMode] Hard menu scene name is empty."
            );

            SetStatus(
                "Hard Menu scene is not configured."
            );

            return;
        }

        Debug.Log(
            "[HardMode] Loading scene: " +
            hardMenuSceneName
        );

        SceneManager.LoadScene(
            hardMenuSceneName
        );
    }

    // =========================================================
    // STATUS
    // =========================================================

    private void SetStatus(string message)
    {
        Debug.Log(
            "[HardMode] " +
            message
        );

        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    // =========================================================
    // SAVE CHECK
    // =========================================================

    public bool IsHardModeUnlocked()
    {
        return PlayerPrefs.GetInt(
            HARD_MODE_UNLOCKED,
            0
        ) == 1;
    }

    // =========================================================
    // PURCHASE SIGNATURE
    // =========================================================

    public string GetPurchaseSignature()
    {
        return PlayerPrefs.GetString(
            HARD_MODE_TX,
            ""
        );
    }
}