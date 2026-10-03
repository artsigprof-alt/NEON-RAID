
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject armoryPanel;
    [SerializeField] private GameObject nftShopPanel;

    [Header("NFT Shop")]
    [SerializeField] private Button closeNFTShopButton;

    [Header("Wallet")]
    [SerializeField] private TMP_Text walletStatus;

    private void Awake()
    {
        Instance = this;

        ShowMain();
    }

    private void Start()
    {
        EnsureMusicController();
        EnsurePersistentManagers();

        DynamicMusicController.Instance?.SetMenu();

        RefreshWallet();

        if (closeNFTShopButton != null)
        {
            closeNFTShopButton.onClick.AddListener(
                CloseNFTShop
            );
        }
    }

    private void EnsureMusicController()
    {
        if (DynamicMusicController.Instance != null)
            return;

        GameObject musicObject =
            new GameObject(
                "DynamicMusicController"
            );

        musicObject.AddComponent<
            DynamicMusicController
        >();
    }

    private void EnsurePersistentManagers()
    {
        if (SkrReactorManager.Instance == null)
{
    GameObject reactorObject =
        new GameObject("SkrReactorManager");

    reactorObject.AddComponent<SkrReactorManager>();
}
        if (CurrencyManager.Instance == null)
        {
            GameObject currencyObject =
                new GameObject(
                    "CurrencyManager"
                );

            currencyObject.AddComponent<
                CurrencyManager
            >();
        }

        if (PlayerStats.Instance == null)
        {
            GameObject statsObject =
                new GameObject(
                    "PlayerStats"
                );

            statsObject.AddComponent<
                PlayerStats
            >();
        }
    }

    // =========================================================
    // MAIN
    // =========================================================

    public void ShowMain()
    {
        if (mainPanel != null)
            mainPanel.SetActive(true);

        if (armoryPanel != null)
            armoryPanel.SetActive(false);

        if (nftShopPanel != null)
            nftShopPanel.SetActive(false);
    }

    // =========================================================
    // ARMORY
    // =========================================================

    public void OpenArmory()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (armoryPanel != null)
            armoryPanel.SetActive(true);

        ArmoryUI armory =
            FindObjectOfType<ArmoryUI>();

        if (armory != null)
            armory.Refresh();
    }

    // =========================================================
    // NFT SHOP
    // =========================================================

    public void OpenNFTShop()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (nftShopPanel != null)
            nftShopPanel.SetActive(true);
    }

    public void CloseNFTShop()
    {
        ShowMain();
    }

    // =========================================================
    // GAME
    // =========================================================

    public void Play()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Game");
    }

    public void BackToMain()
    {
        ShowMain();
    }

    // =========================================================
    // WALLET
    // =========================================================

    private void RefreshWallet()
    {
        if (walletStatus != null)
        {
            walletStatus.text =
                "WALLET: NOT CONNECTED";
        }
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (closeNFTShopButton != null)
        {
            closeNFTShopButton.onClick.RemoveListener(
                CloseNFTShop
            );
        }
    }
}

