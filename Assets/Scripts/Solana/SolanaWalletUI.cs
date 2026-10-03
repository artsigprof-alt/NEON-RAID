
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SolanaWalletUI : MonoBehaviour
{
    [Header("Wallet")]
    [SerializeField] private TextMeshProUGUI walletAddressText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button connectButton;

    [Header("Assets")]
    [SerializeField] private GameObject assetsPanel;
    [SerializeField] private TextMeshProUGUI assetsTitleText;
    [SerializeField] private TextMeshProUGUI skrBalanceText;

    [Header("SKR")]
    [SerializeField] private Button refreshSkrButton;

    [Header("Optional")]
    [SerializeField] private TextMeshProUGUI transactionText;

    private void Start()
    {
        if (connectButton != null)
        {
            connectButton.onClick.AddListener(
                OnConnectClicked
            );
        }

        if (refreshSkrButton != null)
        {
            refreshSkrButton.onClick.AddListener(
                OnRefreshSkrClicked
            );
        }

        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    // =========================================================
    // CONNECT
    // =========================================================

    private async void OnConnectClicked()
    {
        if (SolanaWalletService.Instance == null)
        {
            SetStatus(
                "Wallet service not found"
            );

            return;
        }

        SetStatus(
            "Connecting..."
        );

        if (connectButton != null)
            connectButton.interactable = false;

        bool success =
            await SolanaWalletService.Instance.ConnectAsync();

        if (success)
        {
            SetStatus(
                "Connected!"
            );
        }
        else
        {
            SetStatus(
                "Connection Failed"
            );

            if (connectButton != null)
                connectButton.interactable = true;
        }

        UpdateUI();
    }

    // =========================================================
    // REFRESH SKR
    // =========================================================

    private void OnRefreshSkrClicked()
    {
        if (SolanaWalletService.Instance == null)
        {
            SetStatus(
                "Wallet service not found"
            );

            return;
        }

        if (!SolanaWalletService.Instance.IsConnected)
        {
            SetStatus(
                "Wallet not connected"
            );

            return;
        }

        SetStatus(
            "Updating SKR..."
        );

        SolanaWalletService.Instance.RefreshSkrBalance();

        UpdateUI();
    }

    // =========================================================
    // UPDATE UI
    // =========================================================

    private void UpdateUI()
    {
        SolanaWalletService service =
            SolanaWalletService.Instance;

        if (service == null)
        {
            ShowDisconnected();
            return;
        }

        bool connected =
            service.IsConnected;

        // =====================================================
        // WALLET ADDRESS
        // =====================================================

        if (walletAddressText != null)
        {
            if (
                connected &&
                !string.IsNullOrEmpty(
                    service.PublicKey
                )
            )
            {
                walletAddressText.text =
                    "WALLET: " +
                    ShortenAddress(
                        service.PublicKey
                    );
            }
            else
            {
                walletAddressText.text =
                    "WALLET: NOT CONNECTED";
            }
        }

        // =====================================================
        // CONNECT BUTTON
        // =====================================================

        if (connectButton != null)
        {
            connectButton.gameObject.SetActive(
                !connected
            );

            if (!connected)
                connectButton.interactable = true;
        }

        // =====================================================
        // ASSETS PANEL
        // =====================================================

        if (assetsPanel != null)
        {
            assetsPanel.SetActive(
                connected
            );
        }

        if (assetsTitleText != null)
        {
            assetsTitleText.text =
                connected
                    ? "WALLET ASSETS"
                    : "";
        }

        // =====================================================
        // SKR
        // =====================================================

        if (skrBalanceText != null)
        {
            if (!connected)
            {
                skrBalanceText.text =
                    "SKR: --";
            }
            else if (
                service.IsSkrLoading
            )
            {
                skrBalanceText.text =
                    "SKR: LOADING...";
            }
            else if (
                service.HasSkrBalance
            )
            {
                skrBalanceText.text =
                    "SKR: " +
                    service.SkrBalance;
            }
            else if (
                !string.IsNullOrEmpty(
                    service.SkrError
                )
            )
            {
                // Показываем РЕАЛЬНУЮ ошибку
                skrBalanceText.text =
                    "SKR: " +
                    service.SkrError;
            }
            else
            {
                skrBalanceText.text =
                    "SKR: --";
            }
        }

        // =====================================================
        // REFRESH BUTTON
        // =====================================================

        if (refreshSkrButton != null)
        {
            refreshSkrButton.gameObject.SetActive(
                connected
            );

            refreshSkrButton.interactable =
                !service.IsSkrLoading;
        }

        // =====================================================
        // TRANSACTION
        // =====================================================

        if (
            transactionText != null &&
            !connected
        )
        {
            transactionText.text = "";
        }
    }

    // =========================================================
    // DISCONNECTED
    // =========================================================

    private void ShowDisconnected()
    {
        if (walletAddressText != null)
        {
            walletAddressText.text =
                "WALLET: NOT CONNECTED";
        }

        if (connectButton != null)
        {
            connectButton.gameObject.SetActive(true);
            connectButton.interactable = true;
        }

        if (assetsPanel != null)
        {
            assetsPanel.SetActive(false);
        }

        if (assetsTitleText != null)
        {
            assetsTitleText.text = "";
        }

        if (skrBalanceText != null)
        {
            skrBalanceText.text =
                "SKR: --";
        }

        if (refreshSkrButton != null)
        {
            refreshSkrButton.gameObject.SetActive(false);
        }
    }

    // =========================================================
    // ADDRESS
    // =========================================================

    private string ShortenAddress(
        string address
    )
    {
        if (string.IsNullOrEmpty(address))
            return "NOT CONNECTED";

        if (address.Length <= 12)
            return address;

        return
            address.Substring(
                0,
                6
            ) +
            "..." +
            address.Substring(
                address.Length - 4,
                4
            );
    }

    // =========================================================
    // STATUS
    // =========================================================

    private void SetStatus(
        string message
    )
    {
        if (statusText != null)
            statusText.text = message;
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (connectButton != null)
        {
            connectButton.onClick.RemoveListener(
                OnConnectClicked
            );
        }

        if (refreshSkrButton != null)
        {
            refreshSkrButton.onClick.RemoveListener(
                OnRefreshSkrClicked
            );
        }
    }
}

